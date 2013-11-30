using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NietoYostenWebApp.Code
{
    public class NyUtil
    {
        public static void DeletePicture(NietoYostenDbDataContext db, HttpServerUtility server, int pictureId)
        {
            var picture = db.Pictures.SingleOrDefault(p => p.Id == pictureId);
            string folderName = picture.Album.FolderName;
            string fileName = picture.FileName;

            // Delete pictures from file system
            string originalPicFile = server.MapPath(string.Format(
                       "~/pictures/original/{0}/{1}", folderName, fileName));
            System.IO.File.Delete(originalPicFile);

            string webPicFile = server.MapPath(string.Format(
                "~/pictures/web/{0}/{1}", folderName, fileName));
            System.IO.File.Delete(webPicFile);

            string thumbPicFile = server.MapPath(string.Format(
                "~/pictures/thumb/{0}/{1}", folderName, fileName));
            System.IO.File.Delete(thumbPicFile);

            // Delete picture from DB
            db.Pictures.DeleteOnSubmit(picture);
        }
    }
}