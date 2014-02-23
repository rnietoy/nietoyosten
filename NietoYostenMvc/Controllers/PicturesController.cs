using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using Elmah;
using Massive;
using NietoYostenMvc.Code;
using NietoYostenMvc.Models;

namespace NietoYostenMvc.Controllers
{
    public class PicturesController : ApplicationController
    {
        public const int AlbumPageSize = 20;

        private Pictures _pictures;
        private DynamicModel _albums;

        public PicturesController()
        {
            _pictures = new Pictures();
            _albums = new DynamicModel("NietoYostenDb", "Albums");
        }

        [RequireLogin]
        public ActionResult Index()
        {
            var model = _albums.All();
            return View(model);
        }

        [RequireLogin]
        public ActionResult ShowAlbum(string album)
        {
            int page;

            // If TryParse fails, it sets page to 0, so we set it to 1 (since there is no zeroth page)
            if (!int.TryParse(this.Request.QueryString["page"], out page))
            {
                page = 1;
            }

            var picturesPaged = _pictures.Paged(
                sql: "SELECT P.ID, P.Title, P.FileName, A.FolderName FROM Pictures P " +
                     "INNER JOIN Albums A ON A.ID = P.AlbumID " +
                     "WHERE A.FolderName = @0",
                primaryKey: "ID",
                currentPage: page,
                pageSize: AlbumPageSize,
                args: album);

            var pictures = picturesPaged.Items;

            ViewBag.Title = _pictures.Scalar("SELECT Title FROM Albums WHERE FolderName = @0", album);

            var thumbArray = new List<List<dynamic>>();
            int colPos = 0;
            List<dynamic> currentRow = new List<dynamic>();

            foreach (var picture in pictures)
            {
                currentRow.Add(picture);
                colPos++;
                if (colPos > 3)
                {
                    thumbArray.Add(currentRow);
                    colPos = 0;
                    currentRow = new List<dynamic>();
                }
            }
            if (currentRow.Any())
            {
                thumbArray.Add(currentRow);    
            }

            dynamic model = new ExpandoObject();
            model.FolderName = album;   // Album folder name
            model.ThumbArray = thumbArray;
            model.TotalPages = picturesPaged.TotalPages;
            model.CurrentPage = page;

            return View((object)model);
        }

        [RequireLogin]
        public ActionResult ShowPicture(int pictureid)
        {
            dynamic model = new ExpandoObject();

            // Get picture details, including row number (to be used to calculate the album page)
            var query = _pictures.Query(
                "SELECT * FROM " +
                "  (SELECT ROW_NUMBER() OVER (ORDER BY P.ID) AS Row, P.ID, P.Title, P.FileName, A.FolderName " +
                "  FROM Pictures P " +
                "  INNER JOIN Albums A ON A.ID = P.AlbumID " +
                "  WHERE A.FolderName = (SELECT FolderName FROM Albums WHERE ID = P.AlbumID) " +
                "  ) T " +
                "WHERE ID = @0",
                pictureid);

            model.Picture = query.FirstOrDefault();

            if (null == model.Picture)
            {
                return HttpNotFound();
            }

            // Get ID of previous picture in album
            model.PreviousID = _pictures.Scalar(
                "SELECT TOP 1 P.ID FROM Pictures P " +
                "INNER JOIN Albums A ON A.ID = P.AlbumID " +
                "WHERE P.ID < @0 ORDER BY P.ID DESC",
                pictureid);

            // Get ID of next picture in album
            model.NextID = _pictures.Scalar(
                "SELECT TOP 1 P.ID FROM Pictures P " +
                "INNER JOIN Albums A ON A.ID = P.AlbumID " +
                "WHERE P.ID > @0 ORDER BY P.ID",
                pictureid);

            // Calculate page of requested picture in album
            long page = ((model.Picture.Row - 1)/AlbumPageSize) + 1;
            model.AlbumPage = page;

            return View((object)model);
        }

        /// <summary>
        /// Upload pictures to an album
        /// </summary>
        /// <param name="id">Album folder name</param>
        /// <returns></returns>
        [RequireRole(Role = "family")]
        public ActionResult Upload(string id)
        {
            return View();
        }

        private class UploadPictureDto
        {
            public string fileName;
            public string folderName;
            public long position;
            public string errorMsg = null;
        }

        public string GetTempFilePath(string fileName)
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
                catch (System.IO.IOException ex)
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

            string originalPicFile = Path.Combine(
                HttpContext.Server.MapPath("~/content/pictures/original/" + folderName),
                fileName);

            try
            {
                System.IO.File.Copy(GetTempFilePath(fileName), originalPicFile);
            }
            catch (IOException)
            {
                return Json("Error: Un archivo con este nombre ya existe en este álbum.");
            }
            finally
            {
                if (System.IO.File.Exists(GetTempFilePath(fileName)))
                {
                    System.IO.File.Delete(GetTempFilePath(fileName));    
                }
            }

            // Add picture to database
            _pictures.Add(folderName, fileName);

            return Json("La imagen se subió con éxito");
        }
    }
}
