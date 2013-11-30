using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Text;
// Lucene
using Lucene.Net.Store;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Index;
using Lucene.Net.Documents;

namespace NietoYostenWebApp.admin
{
    public partial class AdminSiteContent : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Populate list of sections
            if (!IsPostBack)
            {
                NietoYostenDbDataContext db = new NietoYostenDbDataContext();
                db.Log = new DebugTextWriter();
                PopulateSectionsList(db, ddlSection, 0, null);
            }
            rptArticles.DataSource = articlesDataSource;
            rptArticles.DataBind();
        }

        public static void PopulateSectionsList(NietoYostenDbDataContext db, DropDownList ddl, int level, int? parent)
        {
            List<Section> sectionList;
            if (parent == null)
            {
                sectionList = db.Sections.Where(section => section.ParentSectionId == null).ToList();
            }
            else
            {
                sectionList = db.Sections.Where(section => section.ParentSectionId == parent).ToList();
            }
            foreach (Section section in sectionList)
            {
                string sectionName = new StringBuilder().Insert(0, "&nbsp; ", level * 2).ToString() + 
                    HttpUtility.HtmlEncode(section.Name);
                ListItem item = new ListItem(HttpUtility.HtmlDecode(sectionName), section.SectionId.ToString());
                ddl.Items.Add(item);
                PopulateSectionsList(db, ddl, level + 1, section.SectionId);
            }
        }

        protected void chkArticle_DataBinding(object sender, EventArgs e)
        {
            CheckBox chk = (CheckBox)(sender);
            chk.ID = string.Format("chkArticle{0}", Eval("ArticleId"));
        }

        protected void btnAddArticle_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/admin/AddArticle.aspx");
        }

        protected void btnPublishAndUnpublish_Click(object sender, EventArgs e)
        {
            NietoYostenDbDataContext db = new NietoYostenDbDataContext();
            int sectionId = int.Parse(ddlSection.SelectedValue);

            var q =
                from a in db.Articles
                where a.SectionId == sectionId
                select a;

            int i = 0;
            foreach (Article a in q)
            {
                CheckBox chk = (CheckBox)rptArticles.Items[i].FindControl(String.Format("chkArticle{0}", a.ArticleId));
                if (chk != null && chk.Checked)
                {
                    a.Published = ((Button)sender).ID == "btnPublish" ? true : false;
                    // Update search index
                    MyLucene.UpdateArticle(a);
                }
                i++;
            }
            db.SubmitChanges();
            rptArticles.DataBind();
        }
    }
}