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
            if (!int.TryParse(this.Request.QueryString["page"], out page))
            {
                page = 1;
            }

            var picturesPaged = _pictures.Paged(
                sql: "SELECT P.ID, P.Title, P.FileName, A.FolderName FROM Pictures P " +
                     "INNER JOIN Albums A ON A.ID = P.AlbumID " +
                     "WHERE A.Title = @0",
                primaryKey: "ID",
                currentPage: page,
                pageSize: 40,
                args: album);

            var pictures = picturesPaged.Items;

            ViewBag.Title = album;

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
            model.ThumbArray = thumbArray;
            model.TotalPages = picturesPaged.TotalPages;
            model.CurrentPage = page;

            return View((object)model);
        }

        [RequireLogin]
        public ActionResult ShowPicture(string album, int picture)
        {
            var model = _pictures.Query(
                "SELECT P.Title, P.FileName, A.FolderName FROM Pictures P " +
                "INNER JOIN Albums A ON A.ID = P.AlbumID " +
                "WHERE A.Title = @0 AND P.ID = @1",
                album, picture).First();

            return View((object)model);
        }
    }
}
