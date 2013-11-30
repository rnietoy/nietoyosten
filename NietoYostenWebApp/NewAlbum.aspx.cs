using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NietoYostenWebApp
{
    public partial class NewAlbum : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void NewAlbumButton_Click(object sender, EventArgs e)
        {
            if (!Roles.IsUserInRole("family"))
            {
                ErrorMessage.Text = "<p class=\"errormsg\">Error: You do not have enough permissions to create an album.</p>";
                ErrorMessage.Visible = true;
                return;
            }

            // Add album to database
            using (var db = new NietoYostenDbDataContext())
            {
                var newAlbum = new Album()
                    {
                        Title = AlbumTitle.Text,
                        FolderName = AlbumFolder.Text
                    };
                db.Albums.InsertOnSubmit(newAlbum);
                db.SubmitChanges();
            }
            // Create folders for album
            //string originalDir = Server.MapPath(string.Format("~/pictures/original/{0}", AlbumFolder.Text));
            Directory.CreateDirectory(Server.MapPath(string.Format("~/pictures/original/{0}", AlbumFolder.Text)));
            Directory.CreateDirectory(Server.MapPath(string.Format("~/pictures/web/{0}", AlbumFolder.Text)));
            Directory.CreateDirectory(Server.MapPath(string.Format("~/pictures/thumb/{0}", AlbumFolder.Text)));

            Response.Redirect("~/Pictures.aspx");
        }
    }
}