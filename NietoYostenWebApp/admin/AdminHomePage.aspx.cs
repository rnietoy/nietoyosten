using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NietoYostenWebApp.admin
{
    public partial class AdminHomePage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            NietoYostenDbDataContext db = new NietoYostenDbDataContext();
            if (!IsPostBack)
            {
                // Populate Section lists
                admin.AdminSiteContent.PopulateSectionsList(db, ddlSection1, 0, null);
                admin.AdminSiteContent.PopulateSectionsList(db, ddlSection2, 0, null);
                admin.AdminSiteContent.PopulateSectionsList(db, ddlSection3, 0, null);
                admin.AdminSiteContent.PopulateSectionsList(db, ddlSection4, 0, null);

                LoadDataFromDb(db);
            }
        }

        /// <summary>
        /// Loads data from the DB into the controls
        /// </summary>
        /// <param name="db"></param>
        void LoadDataFromDb(NietoYostenDbDataContext db)
        {
            for (int i = 1; i <= 4; i++)
            {
                PopulateRow(db, i);
            }
        }

        void PopulateRow(NietoYostenDbDataContext db, int row)
        {
            Table<HomePageArticle> HomePageArticles = db.GetTable<HomePageArticle>();
            HomePageArticle h = HomePageArticles.Single(q => q.Position == row);
            Control mainContent = Page.Master.FindControl("MainContent");
            ((CheckBox)mainContent.FindControl("chkShow" + row)).Checked = h.Enabled;
            int sectionId1 = h.Article.SectionId;
            ((DropDownList)mainContent.FindControl("ddlSection" + row)).SelectedValue = sectionId1.ToString();
            DropDownList ddlArticle = (DropDownList)mainContent.FindControl("ddlArticle" + row);
            PopulateArticleList(db, ddlArticle, sectionId1);
            ddlArticle.SelectedValue = h.ArticleId.ToString();
        }

        void PopulateArticleList(NietoYostenDbDataContext db, int row)
        {
            Control mainContent = Page.Master.FindControl("MainContent");
            DropDownList ddl = (DropDownList)mainContent.FindControl("ddlArticle" + row);
            DropDownList ddlSection = (DropDownList)mainContent.FindControl("ddlSection" + row);
            int sectionId = int.Parse(ddlSection.SelectedValue);
            PopulateArticleList(db, ddl, sectionId);
        }

        void PopulateArticleList(NietoYostenDbDataContext db, DropDownList ddl, int sectionId)
        {
            Table<Article> Article = db.GetTable<Article>();
            var q =
                from a in Article
                where a.SectionId == sectionId
                orderby a.DateCreated descending
                select a;

            ddl.Items.Clear();

            foreach (var article in q)
            {
                string publishedStatus = article.Published ? "" : "(Unpublished)";
                ListItem item = new ListItem(article.Title + " " + publishedStatus, 
                    article.ArticleId.ToString());
                ddl.Items.Add(item);
            }
        }

        protected void ddlSection1_SelectedIndexChanged(object sender, EventArgs e)
        {
            NietoYostenDbDataContext db = new NietoYostenDbDataContext();
            PopulateArticleList(db, 1);
        }

        protected void ddlSection2_SelectedIndexChanged(object sender, EventArgs e)
        {
            NietoYostenDbDataContext db = new NietoYostenDbDataContext();
            PopulateArticleList(db, 2);
        }

        protected void ddlSection3_SelectedIndexChanged(object sender, EventArgs e)
        {
            NietoYostenDbDataContext db = new NietoYostenDbDataContext();
            PopulateArticleList(db, 3);
        }

        protected void ddlSection4_SelectedIndexChanged(object sender, EventArgs e)
        {
            NietoYostenDbDataContext db = new NietoYostenDbDataContext();
            PopulateArticleList(db, 4);
        }

        void GetDataFromRow(HomePageArticle hpa, int row)
        {
            Control mainContent = Page.Master.FindControl("MainContent");
            CheckBox chkShow = (CheckBox)mainContent.FindControl("chkShow" + row);
            hpa.Enabled = chkShow.Checked;
            DropDownList ddlArticle = (DropDownList)mainContent.FindControl("ddlArticle" + row);
            hpa.ArticleId = int.Parse(ddlArticle.SelectedValue);
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            NietoYostenDbDataContext db = new NietoYostenDbDataContext();

            var q =
                from h in db.HomePageArticles
                orderby h.Position
                where h.Position >= 1 && h.Position <=4
                select h;

            int i = 1;
            foreach (var hpa in q)
            {
                GetDataFromRow((HomePageArticle)hpa, i);
                i++;
            }
            db.SubmitChanges();
            Response.Redirect("~/admin/AdminSiteContent.aspx");
        }
    }
}