using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Dynamic;
using System.Linq;
using System.Web;
using Elmah;
using Massive;

namespace NietoYostenMvc.Models
{
    public class Pictures : DynamicModel
    {

        public Pictures() : base("NietoYostenDb", "Pictures", "ID")
        {
        }

        public dynamic Add(string folderName, string fileName, int userId)
        {
            dynamic result = new ExpandoObject();
            result.Success = false;

            int albumId = (int)this.Scalar("SELECT ID FROM Albums WHERE FolderName = @0", folderName);

            try
            {
                this.Insert(new
                {
                    AlbumID = albumId,
                    Title = fileName,
                    FileName = fileName,
                    UploadedBy = userId
                });

                result.Success = true;
            }
            catch (SqlException ex)
            {
                result.Message = "Ocurrió un error al agregar la foto.";
                ErrorSignal.FromCurrentContext().Raise(ex);
            }

            return result;
        }
    }
}