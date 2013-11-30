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
    public partial class ViewAlbum : System.Web.UI.Page
    {
        private const int NumThumbnailsPerRow = 5;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                int albumId;
                if (!int.TryParse(Page.Request.QueryString["AlbumId"], out albumId)) return;

                UploadPictureButton.PostBackUrl = string.Format("~/UploadPicture.aspx?AlbumId={0}", albumId);

                rptThumbnailGrid.DataSource = GetThumbnailRowSet(albumId);
                rptThumbnailGrid.DataBind();
            }
        }

        protected class Thumbnail
        {
            public int PictureId { get; set; }
            public string RelativePath { get; set; }
            public string Title { get; set; }
            public bool Empty { get; set; }
            public string AlphaOmega { get; set; }
        }

        protected IEnumerable<Thumbnail[]> GetThumbnailRowSet(int albumId)
        {
            List<Thumbnail[]> rows = new List<Thumbnail[]>();

            using (var db = new NietoYostenDbDataContext())
            {
                // Get all pictures in album
                var pics = from c in db.Pictures
                           where c.AlbumId == albumId
                           select c;
                
                // Get album folder name
                string folderName = db.Albums.FirstOrDefault(a => a.Id == albumId).FolderName;

                // Iterate through the album pictures to create datasource for the thumbnail grid (a list of Thumbnail arrays)
                int currentRow = 0;
                int currentCol = 0;
                Thumbnail[] workingRow = null;

                foreach (var pic in pics)
                {
                    if (workingRow == null)
                    {
                        // Start new row
                        workingRow = new Thumbnail[NumThumbnailsPerRow];
                        for (int i = 0; i < NumThumbnailsPerRow; i++)
                        {
                            workingRow[i] = new Thumbnail { Empty = true };
                        }
                        workingRow[0].AlphaOmega = "alpha";
                        workingRow[NumThumbnailsPerRow - 1].AlphaOmega = "omega";
                    }

                    // Fill data for current picture
                    workingRow[currentCol].PictureId = pic.Id;
                    workingRow[currentCol].RelativePath = string.Format(
                            "pictures/thumb/{0}/{1}", folderName, pic.FileName);
                    workingRow[currentCol].Title = pic.Title;
                    workingRow[currentCol].Empty = false;

                    // Move to column, row cursors for next picture
                    currentCol++;
                    if (currentCol >= NumThumbnailsPerRow)
                    {
                        // Move to next row
                        currentCol = 0;
                        currentRow++;
                        
                        // Add working row and clear for next row
                        rows.Add(workingRow);
                        workingRow = null;
                    }
                }
                if (workingRow != null)
                {
                    rows.Add(workingRow);
                }
            }
            return rows;
        }

        protected void DeletePicture_Click(object sender, EventArgs e)
        {
            if (!Roles.IsUserInRole("family"))
            {
                ErrorMessage.Text = "<p class=\"errormsg\">Error: You do not have enough permissions to delete pictures.</p>";
                ErrorMessage.Visible = true;
                return;
            }

            if (DeleteConfirmed.Value == "true")
            {
                var selPics = GetSelectedPictures();

                using (var db = new NietoYostenDbDataContext())
                {
                    foreach (int pictureId in selPics)
                    {
                        NyUtil.DeletePicture(db, Server, pictureId);
                    }
                    db.SubmitChanges();
                }
                Response.Redirect(Request.RawUrl);
            }           
        }

        /// <summary>
        /// Check which pictures has been selected.
        /// </summary>
        /// <returns>A list with the ids of the selected pictures.</returns>
        protected IEnumerable<int> GetSelectedPictures()
        {
            int albumId;
            if (!int.TryParse(Page.Request.QueryString["AlbumId"], out albumId)) return null;
            var thumbnails = GetThumbnailRowSet(albumId);

            var selectedPictures = new List<int>();

            int currentRow = 0;
            foreach (Thumbnail[] row in thumbnails)
            {
                int currentCol = 0;
                foreach (var thumbnail in row)
                {
                    var checkBox = (CheckBox)rptThumbnailGrid.Controls[currentRow].Controls[1].Controls[currentCol].Controls[1].Controls[3];
                    if (checkBox.Checked)
                    {
                        selectedPictures.Add(thumbnail.PictureId);
                    }
                    currentCol++;
                }
                currentRow++;
            }

            return selectedPictures;
        }
    }
}