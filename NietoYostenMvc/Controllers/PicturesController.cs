using System;
using System.Dynamic;
using System.IO;
using System.Threading;
using System.Web.Mvc;
using Elmah;
using NietoYostenMvc.Code;
using NietoYostenMvc.Models;

namespace NietoYostenMvc.Controllers
{
    public class PicturesController : ApplicationController
    {
        private readonly PicturesModel picturesModel;
        private readonly AlbumsModel albumsModel;
        private readonly ImageStorage imageStorage;

        public PicturesController()
        {
            this.picturesModel = PicturesModel.GetInstance();
            this.albumsModel = AlbumsModel.GetInstance();
            this.imageStorage = ImageStorage.GetInstance();
        }

        [RequireLogin]
        public ActionResult Index()
        {
            var model = albumsModel.GetAll();
            return View(model);
        }

        [RequireLogin]
        public ActionResult ShowAlbum(string album)
        {
            int currentPage = this.GetCurrentPage();
            PageResult pageResult = this.picturesModel.GetPage(album, currentPage);

            dynamic model = new ExpandoObject();
            model.FolderName = album;   // Album folder name
            model.ThumbArray = NyUtil.ConvertListToTable<dynamic>(pageResult.Items, 4);
            model.TotalPages = pageResult.TotalPages;
            model.CurrentPage = currentPage;

            return this.View((object)model);
        }

        [RequireLogin]
        public ActionResult ShowPicture(int pictureid)
        {
            dynamic picture = this.picturesModel.Get(pictureid);

            if (picture == null)
            {
                return HttpNotFound();
            }

            return View((object)picture);
        }

        /// <summary>
        /// Upload pictures to an album
        /// </summary>
        /// <param name="id">Album folder name</param>
        /// <returns></returns>
        [RequireRole(Role = "family")]
        public ActionResult Upload(string id)
        {
            return View((object)id);
        }

        private class UploadPictureDto
        {
            public string fileName;
            public string folderName;
            public long position;
            public string errorMsg = null;
        }

        private string GetTempFilePath(string fileName)
        {
            return Path.Combine(
                HttpContext.Server.MapPath("~/content/pictures/upload"),
                fileName + ".incomplete");
        }

        [HttpPost]
        [RequireRole(Role = "family")]
        public ActionResult BeginFileUpload(string folderName, string fileName, string base64Data)
        {
            if (!Request.IsAjaxRequest())
            {
                return HttpNotFound();
            }

            long position;
            using (FileStream fs = System.IO.File.Create(GetTempFilePath(fileName)))
            {
                byte[] data = Convert.FromBase64String(base64Data);
                fs.Write(data, 0, data.Length);
                position = fs.Position;
            }

            return Json(new UploadPictureDto
            {
                fileName = fileName,
                folderName = folderName,
                position = position
            });
        }

        [HttpPost]
        [RequireRole(Role = "family")]
        public ActionResult UploadFileChunk(string folderName, string fileName, string base64Data)
        {
            if (!Request.IsAjaxRequest())
            {
                return HttpNotFound();
            }

            long position = 0;
            // Try several times, sometimes the file has not been completely closed since the
            // last call to this method, so we get a "file is opened by another process" exception.
            // We wait for a little bit and then try again.
            for (int i = 0; i < 5; i++)
            {
                try
                {
                    using (FileStream fs = System.IO.File.Open(GetTempFilePath(fileName), FileMode.Append))
                    {
                        byte[] data = Convert.FromBase64String(base64Data);
                        fs.Write(data, 0, data.Length);
                        position = fs.Position;
                    }
                    break;
                }
                catch (IOException ex)
                {
                    ErrorSignal.FromCurrentContext().Raise(ex);
                    Thread.Sleep(200);
                }
            }

            return Json(
                new UploadPictureDto
                {
                    fileName = fileName,
                    folderName = folderName,
                    position = position
                });
        }

        [HttpPost]
        [RequireRole(Role = "family")]
        public ActionResult EndFileUpload(string folderName, string fileName, string base64Data)
        {
            if (!Request.IsAjaxRequest())
            {
                return HttpNotFound();
            }

            dynamic addResult = picturesModel.Add(folderName, fileName, CurrentUserID);
            dynamic picture = picturesModel.Get(addResult.ID);
            this.imageStorage.Upload(this.GetTempFilePath(fileName), picture.FullName);
            
            if (System.IO.File.Exists(GetTempFilePath(fileName)))
            {
                System.IO.File.Delete(GetTempFilePath(fileName));
            }

            return Json("La imagen se subió con éxito");
        }

        [RequireRole(Role = "family")]
        public ActionResult AddAlbum()
        {
            return View();
        }

        [HttpPost]
        [RequireRole(Role = "family")]
        [HandleUserFriendlyError(View = "AddAlbum")]
        public ActionResult AddAlbum(string title, string folder)
        {
            albumsModel.Add(title, folder, CurrentUserID);
            return RedirectToAction("Index");
        }

        [HttpPost]
        [RequireRole(Role = "family")]
        public ActionResult DeletePictures(string[] pictureIds)
        {
            if (!Request.IsAjaxRequest())
            {
                return HttpNotFound();
            }

            // Foreach picture id, delete it from storage and the database
            foreach (string pictureId in pictureIds)
            {
                int id = int.Parse(pictureId);
                dynamic picture = this.picturesModel.Get(id);
                this.imageStorage.TryDelete(picture.FullName);
                this.picturesModel.Delete(id);
            }

            string message = string.Format("Deleted {0} picture(s).", pictureIds.Length);
            this.SetAlertMessage(message, AlertClass.AlertSuccess);

            return Json(message);
        }
    }
}
