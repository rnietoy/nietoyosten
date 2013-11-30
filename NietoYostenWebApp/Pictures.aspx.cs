using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using NietoYostenWebApp.Code;

namespace NietoYostenWebApp
{
    public partial class Pictures : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                using (var db = new NietoYostenDbDataContext())
                {
                    rptAlbums.DataSource = db.Albums;
                    rptAlbums.DataBind();
                }
            }
        }

        protected void DeleteAlbumButton_Click(object sender, EventArgs e)
        {
            if (!Roles.IsUserInRole("family"))
            {
                ErrorMessage.Text = "<p class=\"errormsg\">Error: You do not have enough permissions to delete an album.</p>";
                ErrorMessage.Visible = true;
                return;
            }

            if (DeleteAlbumConfirmed.Value == "true")
            {
                var selAlbums = GetSelectedAlbums();
                using (var db = new NietoYostenDbDataContext())
                {
                    foreach (int albumId in selAlbums)
                    {
                        DeleteAlbum(db, albumId);
                    }
                    db.SubmitChanges();
                }
                Response.Redirect(Request.RawUrl);
            }
        }

        /// <summary>
        /// Get the selected albums
        /// </summary>
        /// <returns>List of the ids of the selected albums</returns>
        protected IEnumerable<int> GetSelectedAlbums()
        {
            var selectedAlbums = new List<int>();

            foreach (Control albumItem in rptAlbums.Controls)
            {
                var albumCheckbox = (CheckBox) albumItem.Controls[1];

                if (albumCheckbox.Checked)
                {
                    var albumIdField = (HiddenField)albumItem.Controls[5];
                    selectedAlbums.Add(int.Parse(albumIdField.Value));
                }
            }

            return selectedAlbums;
        }

        protected void DeleteAlbum(NietoYostenDbDataContext db, int albumId)
        {
            // Delete pictures in album, if any
            var album = db.Albums.SingleOrDefault(a => a.Id == albumId);
            foreach (var picture in album.Pictures)
            {
                NyUtil.DeletePicture(db, Server, picture.Id);
            }

            // Delete album folders (thumb, web, original)
            string[] folders = {"thumb", "web", "original"};

            foreach (var folder in folders)
            {
                string pictureFolder = Server.MapPath(string.Format(
                    "~/pictures/{0}/{1}", folder, album.FolderName));
                if (System.IO.Directory.Exists(pictureFolder))
                {
                    System.IO.Directory.Delete(pictureFolder);    
                }
            }

            // Remove album from DB
            db.Albums.DeleteOnSubmit(album);
        }
    }
}