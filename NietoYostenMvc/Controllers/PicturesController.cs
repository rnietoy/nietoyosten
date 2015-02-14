using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web.Mvc;
using Elmah;
using Massive;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
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
            var model = _albums.All(orderBy: "CreatedAt desc");
            return View(model);
        }

        [RequireLogin]
        public ActionResult ShowAlbum(string album)
        {
            var page = this.GetPage();

            var picturesPaged = _pictures.Paged(
                sql: "SELECT P.ID, P.Title, P.FileName, A.FolderName, P.UploadedAt FROM Pictures P " +
                     "INNER JOIN Albums A ON A.ID = P.AlbumID " +
                     "WHERE A.FolderName = @0",
                primaryKey: "ID",
                currentPage: page,
                pageSize: AlbumPageSize,
                orderBy: "UploadedAt",
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
            int albumId = (int)_albums.Scalar(
                "SELECT AlbumID FROM Pictures WHERE ID = @0", pictureid);

            // Get picture details, including row number (to be used to calculate the album page)
            var query = _pictures.Query(
                "SELECT * FROM " +
                "  (SELECT ROW_NUMBER() OVER (ORDER BY P.ID) AS Row, P.ID, P.Title, P.FileName, A.FolderName " +
                "  FROM Pictures P" +
                "  INNER JOIN Albums A ON A.ID = P.AlbumID" +
                "  WHERE A.id = @0" +
                "  ) T " +
                "WHERE ID = @1",
                albumId, pictureid);

            model.Picture = query.FirstOrDefault();

            if (null == model.Picture)
            {
                return HttpNotFound();
            }

            // Get ID of previous and next picture in album
            model.PreviousID = _pictures.Scalar(
                "SELECT TOP 1 P.ID FROM Pictures P " +
                "INNER JOIN Albums A ON A.ID = P.AlbumID " +
                "WHERE A.ID = @0 " +
                "AND P.ID < @1 ORDER BY P.ID DESC",
                albumId, pictureid);

            model.NextID = _pictures.Scalar(
                "SELECT TOP 1 P.ID FROM Pictures P " +
                "INNER JOIN Albums A ON A.ID = P.AlbumID " +
                "WHERE A.ID = @0 " +
                "AND P.ID > @1 ORDER BY P.ID",
                albumId, pictureid);

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
            return View((object)id);
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

            string azureFileName = string.Format("{0}/{1}", folderName, fileName);

            try
            {
                UploadToAzure(GetTempFilePath(fileName), azureFileName);
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
            _pictures.Add(folderName, fileName, CurrentUserID);

            return Json("La imagen se subió con éxito");
        }

        [RequireRole(Role = "family")]
        public ActionResult AddAlbum()
        {
            return View();
        }

        [HttpPost]
        [RequireRole(Role = "family")]
        public ActionResult AddAlbum(string title, string folder)
        {
            try
            {
                Directory.CreateDirectory(HttpContext.Server.MapPath("~/content/pictures/original/" + folder));

                _albums.Insert(new
                {
                    Title = title,
                    FolderName = folder,
                    CreatedBy = CurrentUserID,
                    ModifiedBy = CurrentUserID
                });
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);

                this.SetAlertMessage("Ocurrió un error al crear el álbum.", AlertClass.AlertDanger);
                return View();
            }
            return RedirectToAction("Index");
        }

        private void UploadToAzure(string sourceFileName, string destFileName)
        {
            CloudStorageAccount storageAccount = NyUtil.StorageAccount;
            
            // Create the blob client and reference the container
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference("pictures");

            // Upload image to Blob Storage
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(destFileName);
            blockBlob.Properties.ContentType = "image/jpeg";
            blockBlob.UploadFromFile(sourceFileName, FileMode.Open);
        }
    }
}
