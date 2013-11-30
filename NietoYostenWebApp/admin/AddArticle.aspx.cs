using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.Security;
using System.Data.SqlTypes;
// Lucene
using Lucene.Net.Store;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Index;
using Lucene.Net.Documents;

namespace NietoYostenWebApp.admin
{
    public partial class AddArticle : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            NietoYostenDbDataContext db = new NietoYostenDbDataContext();
            admin.AdminSiteContent.PopulateSectionsList(db, ddlSection, 0, null);
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            // Insert the article into the DB
            NietoYostenDbDataContext db = new NietoYostenDbDataContext();
            db.Log = new DebugTextWriter();
            MembershipUser user = Membership.GetUser();
            Article article = new Article
            {
                Title = HttpUtility.HtmlEncode(txtTitle.Text),
                IntroText = introEditor.Text,
                Content = contentEditor.Text,
                SectionId = int.Parse(ddlSection.SelectedValue),
                CreatedBy = (Guid)user.ProviderUserKey,
                DateCreated = DateTime.Now
            };
            db.Articles.InsertOnSubmit(article);
            db.SubmitChanges();
            MyLucene.UpdateArticle(article);
            Response.Redirect("~/Admin/AdminSiteContent.aspx");
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Admin/AdminSiteContent.aspx");
        }
    }
}