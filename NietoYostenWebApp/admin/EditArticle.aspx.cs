using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;
// Lucene
using Lucene.Net.Store;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Index;
using Lucene.Net.Documents;

namespace NietoYostenWebApp.admin
{
    public partial class EditArticle : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                NietoYostenDbDataContext db = new NietoYostenDbDataContext();
                int articleId = int.Parse(Page.RouteData.Values["ArticleId"].ToString());
                Article article = db.Articles.Single(a => a.ArticleId == articleId);
                // Populate section list
                admin.AdminSiteContent.PopulateSectionsList(db, ddlSection, 0, null);
                ddlSection.SelectedValue = article.SectionId.ToString();
                txtTitle.Text = article.Title;
                introEditor.Text = article.IntroText;
                contentEditor.Text = article.Content;
            }
            
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            NietoYostenDbDataContext db = new NietoYostenDbDataContext();
            MembershipUser user = Membership.GetUser();
            int articleId = int.Parse(Page.RouteData.Values["ArticleId"].ToString());
            Article article = db.Articles.Single(a => a.ArticleId == articleId);
            article.Title = txtTitle.Text;
            article.IntroText = introEditor.Text;
            article.Content = contentEditor.Text;
            article.SectionId = int.Parse(ddlSection.SelectedValue);
            article.ModifiedBy = (Guid)user.ProviderUserKey;
            article.DateModified = DateTime.Now;
            db.SubmitChanges();
            // Update index
            MyLucene.UpdateArticle(article);

            Response.Redirect("~/admin/AdminSiteContent.aspx");
        }
    }
}