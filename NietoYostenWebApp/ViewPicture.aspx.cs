using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NietoYostenWebApp
{
    public partial class ViewPicture : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            int pictureId = 0;
            if (!int.TryParse(Page.Request.QueryString["PictureId"], out pictureId)) return;

            using (var db = new NietoYostenDbDataContext())
            {
                var pic = db.Pictures.FirstOrDefault(a => a.Id == pictureId);
                if (pic == null) return;

                string albumFolderName = pic.Album.FolderName;

                PageImage.ImageUrl = string.Format(
                    "~/pictures/web/{0}/{1}",
                    albumFolderName,
                    pic.FileName);

                PicTitle.Text = pic.Title;

                DownloadOriginalLink.NavigateUrl = string.Format(
                    "~/pictures/original/{0}/{1}",
                    albumFolderName,
                    pic.FileName);
            }
        }
    }
}