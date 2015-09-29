using System;
using System.Data.SqlClient;
using System.Linq;
using System.Web.DynamicData;
using Massive;
using NietoYostenMvc.Code;

namespace NietoYostenMvc.Models
{
    public class PicturesModel
    {
        private const int DefaultPageSize = 20;

        private readonly DynamicModel dynamicModel;
        private readonly int pageSize;

        private PicturesModel(int pageSize)
        {
            this.dynamicModel = new DynamicModel("NietoYostenDb", "Pictures", "ID");
            this.pageSize = pageSize;
        }

        public static PicturesModel GetInstance()
        {
            return new PicturesModel(DefaultPageSize);
        }

        public static PicturesModel GetInstance(int pageSize)
        {
            return new PicturesModel(pageSize);
        }

        public dynamic Add(string folderName, string fileName, int userId)
        {
            dynamic result = null;

            int albumId = (int) dynamicModel.Scalar("SELECT ID FROM Albums WHERE FolderName = @0", folderName);

            try
            {
                result = dynamicModel.Insert(new
                {
                    AlbumID = albumId,
                    Title = fileName,
                    FileName = fileName,
                    UploadedBy = userId,
                });
            }
            catch (SqlException ex)
            {
                if (ex.Message.Contains("UQ_Pictures_AlbumID_FileName"))
                {
                    throw UserFriendlyException.GetInstance("An image with this file name already exists in this album.", ex);
                }
            }

            return result;
        }

        public dynamic Get(int pictureId)
        {
            object albumIdObj = this.dynamicModel.Scalar(
                "SELECT AlbumID FROM Pictures WHERE ID = @0", pictureId);

            if (albumIdObj == null)
            {
                return null;
            }

            int albumId = (int) albumIdObj;

            // Get picture details, including row number (to be used to calculate the album page)
            var query = this.dynamicModel.Query(
                "SELECT * FROM " +
                "  (SELECT ROW_NUMBER() OVER (ORDER BY P.UploadedAt, P.Title) AS Row, P.ID, P.Title, P.FileName, A.FolderName " +
                "  FROM Pictures P" +
                "  INNER JOIN Albums A ON A.ID = P.AlbumID" +
                "  WHERE A.ID = @0" +
                "  ) T " +
                "WHERE ID = @1",
                albumId, pictureId);

            dynamic picture = query.FirstOrDefault();

            if (picture == null)
            {
                return null;
            }

            int row = (int)picture.Row;

            picture.PreviousID = this.PictureIdAtPosition(albumId, row - 1);
            picture.NextID = this.PictureIdAtPosition(albumId, row + 1);
            picture.AlbumPage = ((picture.Row - 1)/this.pageSize) + 1;
            picture.FullName = string.Format("{0}/{1}", picture.FolderName, picture.FileName);

            return picture;
        }

        public dynamic GetByFileName(int albumId, string filename)
        {
            dynamic picture = this.dynamicModel.Single(
                where: "AlbumID = @0 AND FileName = @1",
                args: new object[] { albumId, filename });

            return this.Get(picture.ID);
        }

        private int? PictureIdAtPosition(int albumId, int position)
        {
            return (int?)this.dynamicModel.Scalar(
                "SELECT T.ID FROM " +
                "  (SELECT ROW_NUMBER() OVER (ORDER BY P.UploadedAt, P.Title) AS Row, P.ID, P.Title, P.FileName, A.FolderName " +
                "  FROM Pictures P" +
                "  INNER JOIN Albums A ON A.ID = P.AlbumID" +
                "  WHERE A.ID = @0" +
                "  ) T " +
                "WHERE Row = @1",
                albumId, position);
        }

        public PageResult GetPage(string albumFolderName, int pageNumber)
        {
            dynamic result = this.dynamicModel.Paged(
                sql: "SELECT P.ID, P.Title, P.FileName, A.FolderName, P.UploadedAt FROM Pictures P " +
                     "INNER JOIN Albums A ON A.ID = P.AlbumID " +
                     "WHERE A.FolderName = @0",
                primaryKey: "ID",
                currentPage: pageNumber,
                pageSize: this.pageSize,
                orderBy: "UploadedAt",
                args: albumFolderName);

            return PageResult.GetInstance(result.Items, result.TotalPages);
        }

        public void Delete(int pictureId)
        {
            this.dynamicModel.Delete(pictureId);
        }

        public void DeleteAllInAlbum(int albumId)
        {
            this.dynamicModel.Delete(null, "AlbumID = @0", albumId);
        }
    }
}