using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using Massive;
using NietoYostenMvc.Code;

namespace NietoYostenMvc.Models
{
    public class AlbumsModel
    {
        private readonly DynamicModel dynamicModel;

        private AlbumsModel()
        {
            this.dynamicModel = new DynamicModel("NietoYostenDb", "Albums", "ID");
        }

        public static AlbumsModel GetInstance()
        {
            return new AlbumsModel();
        }

        public IEnumerable<dynamic> GetAll()
        {
            return this.dynamicModel.All(orderBy: "CreatedAt DESC,Title");
        }

        public dynamic GetByFolderName(string folderName)
        {
            return this.dynamicModel.Single(where: "FolderName = @0", args: folderName);
        }

        public dynamic Add(string title, string folderName, int userId)
        {
            dynamic result = null;

            try
            {
                result = this.dynamicModel.Insert(new
                {
                    Title = title,
                    FolderName = folderName,
                    CreatedBy = userId,
                    ModifiedBy = userId
                });
            }
            catch (SqlException ex)
            {
                if (ex.Message.Contains("UQ_Albums_FolderName"))
                {
                    throw UserFriendlyException.GetInstance("An album with this folder name already exists.", ex);
                }
            }

            return result;
        }

        public string GetTitle(string folderName)
        {
            return (string)this.dynamicModel.Scalar("SELECT Title FROM Albums WHERE FolderName = @0", folderName);
        }
    }
}