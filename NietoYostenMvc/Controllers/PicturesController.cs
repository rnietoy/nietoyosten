using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Massive;
using NietoYostenMvc.Code;

namespace NietoYostenMvc.Controllers
{
    public class PicturesController : ApplicationController
    {
        public const int AlbumPageSize = 20;

        private DynamicModel _pictures;
        private DynamicModel _albums;

        public PicturesController()
        {
            _pictures = new DynamicModel("NietoYostenDb", "Pictures");
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
        public ActionResult ShowPicture(string album, int picture)
        {
            dynamic model = new ExpandoObject();

            // Get picture details, including row number (to be used to calculate the album page)
            var query = _pictures.Query(
                "SELECT * FROM " +
                "  (SELECT ROW_NUMBER() OVER (ORDER BY P.ID) AS Row, P.ID, P.Title, P.FileName, A.FolderName FROM Pictures P " +
                "  INNER JOIN Albums A ON A.ID = P.AlbumID " +
                "  WHERE A.FolderName = @0) T " +
                "WHERE ID = @1",
                album, picture);

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
                picture);

            // Get ID of next picture in album
            model.NextID = _pictures.Scalar(
                "SELECT TOP 1 P.ID FROM Pictures P " +
                "INNER JOIN Albums A ON A.ID = P.AlbumID " +
                "WHERE P.ID > @0 ORDER BY P.ID",
                picture);

            // Calculate page of requested picture in album
            long page = ((model.Picture.Row - 1)/AlbumPageSize) + 1;
            model.AlbumPage = page;

            return View((object)model);
        }
    }
}
