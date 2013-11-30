using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NietoYostenWebApp
{
    public partial class UploadPicture : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            
        }

        /// <summary>
        /// Resize an image preserving the aspec ration
        /// </summary>
        /// <param name="originalFile">File name of original picture, including full path</param>
        /// <param name="maxDimension">How long the maximum dimention should measure (either the height, or the width)</param>
        public static void ResizeImage(string originalFile, string destinationFile, int maxDimension)
        {
            System.Drawing.Image originalImg = System.Drawing.Image.FromFile(originalFile);
            int effectiveWidth;
            int effectiveHeight;

            // Calculate effective width and height
            if (originalImg.Width > originalImg.Height)
            {
                effectiveWidth = maxDimension;
                effectiveHeight = (originalImg.Height*maxDimension)/originalImg.Width;
            }
            else
            {
                effectiveHeight = maxDimension;
                effectiveWidth = (originalImg.Width*maxDimension)/originalImg.Height;
            }

            // Resize image
            System.Drawing.Image thumbImg = new Bitmap(effectiveWidth, effectiveHeight, originalImg.PixelFormat);
            Graphics oGraphic = Graphics.FromImage(thumbImg);
            oGraphic.CompositingQuality = CompositingQuality.HighQuality;
            oGraphic.SmoothingMode = SmoothingMode.HighQuality;
            oGraphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
            oGraphic.DrawImage(originalImg, new Rectangle(0, 0, effectiveWidth, effectiveHeight));

            thumbImg.Save(destinationFile);

            originalImg.Dispose();
        }

        public class UploadPictureDto
        {
            public string fileName;
            public string folderName;
            public long position;
            public string errorMsg;
        }

        public static string GetTempFilePath(string fileName)
        {
            return Path.Combine(
                HttpContext.Current.Server.MapPath("~/pictures/upload"),
                fileName + ".incomplete");
        }

        [WebMethod]
        public static UploadPictureDto BeginFileUpload(int albumId, string fileName, string base64Data)
        {
            if (!Roles.IsUserInRole("family"))
            {
                return new UploadPictureDto { fileName = null, folderName = null, position = 0,
                    errorMsg = "Current user is not allowed to upload pictures." };
            }

            string folderName = null;
            using (var db = new NietoYostenDbDataContext())
            {
                var album = db.Albums.FirstOrDefault(x => x.Id == albumId);
                folderName = album.FolderName;
            }

            long position = 0;
            using (var fs = File.Create(GetTempFilePath(fileName)))
            {
                byte[] data = Convert.FromBase64String(base64Data);
                fs.Write(data, 0, data.Length);
                position = fs.Position;
            }

            return new UploadPictureDto {fileName = fileName, folderName = folderName, position = position };
        }

        [WebMethod]
        public static UploadPictureDto UploadFileChunk(string folderName, string fileName, string base64Data)
        {
            if (!Roles.IsUserInRole("family"))
            {
                return new UploadPictureDto
                {
                    fileName = null,
                    folderName = null,
                    position = 0,
                    errorMsg = "Current user is not allowed to upload pictures."
                };
            }

            long position = 0;
            using (var fs = File.Open(GetTempFilePath(fileName), FileMode.Append))
            {
                byte[] data = Convert.FromBase64String(base64Data);
                fs.Write(data, 0, data.Length);
                position = fs.Position;
            }

            return new UploadPictureDto() { fileName = fileName, folderName = folderName, position = position };
        }

        [WebMethod]
        public static string EndFileUpload(string folderName, string fileName)
        {
            if (!Roles.IsUserInRole("family"))
            {
                return "Error: Current user is not allowed to upload pictures.";
            }

            string originalPicFile = Path.Combine(
                HttpContext.Current.Server.MapPath("~/pictures/original/" + folderName),
                fileName);

            try
            {
                File.Copy(GetTempFilePath(fileName), originalPicFile);
            }
            catch (IOException)
            {
                return "Error: file with same name already exists in this album.";
            }
            finally
            {
                File.Delete(GetTempFilePath(fileName));    
            }
            
            // Create and save web-sized image
            string webPicFile = HttpContext.Current.Server.MapPath(string.Format(
                "~/pictures/web/{0}/{1}", folderName, fileName));
            ResizeImage(originalPicFile, webPicFile, 640);

            // Create and save thumbnail-sized image
            string thumbPicFile = HttpContext.Current.Server.MapPath(string.Format(
                "~/pictures/thumb/{0}/{1}", folderName, fileName));
            ResizeImage(originalPicFile, thumbPicFile, 120);

            // Add picture to the database
            using (var db = new NietoYostenDbDataContext())
            {
                var album = db.Albums.FirstOrDefault(x => x.FolderName == folderName);

                var picture = new Picture();
                picture.AlbumId = album.Id;

                picture.FileName = fileName;
                picture.Title = fileName;
                
                db.Pictures.InsertOnSubmit(picture);
                db.SubmitChanges();
            }
            return "Upload successful";
        }
    }
}