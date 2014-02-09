using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Massive;

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

        public ActionResult Index()
        {
            var model = _albums.All();
            return View(model);
        }

        public ActionResult ShowAlbum(string album)
        {
            var pictures = _pictures.Query(
                "SELECT P.Title, P.FileName, A.FolderName FROM Pictures P " +
                "INNER JOIN Albums A ON A.ID = P.AlbumID " +
                "WHERE A.Title = @0",
                album);

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

            return View(thumbArray);
        }

        public ActionResult ShowPicture(string album, string picture)
        {
            var model = _pictures.Query(
                "SELECT P.Title, P.FileName, A.FolderName FROM Pictures P " +
                "INNER JOIN Albums A ON A.ID = P.AlbumID " +
                "WHERE A.Title = @0 AND P.Title = @1",
                album, picture).First();

            return View((object)model);
        }

    }
}
