using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Configuration;
using System.Collections;

namespace NietoYostenWebApp
{
    public partial class Content : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string sectionName = Page.RouteData.Values["SectionName"].ToString();
            
            int pageNum = GetPageNum();
            int articlesPerPage = int.Parse(ConfigurationManager.AppSettings["articlesPerPage"]);
            int startAtArticle = (pageNum - 1) * articlesPerPage + 1;

            // Create datasource for repeater based on page number
            NietoYostenDbDataContext db = new NietoYostenDbDataContext();
            Section section = db.Sections.Single(s => s.Name == sectionName);
            int sectionId = section.SectionId;
            litPageName.Text = section.Name;

            var q =
                from c in db.Articles
                join user in db.aspnet_Users on c.CreatedBy equals user.UserId
                where c.SectionId == sectionId
                where c.Published == true
                orderby c.DateCreated descending
                select new { c.ArticleId, c.Title, c.IntroText, c.DateCreated, user.UserName, 
                     HasNoContent = String.IsNullOrEmpty(c.Content) };

            rptArticles.DataSource = q.Skip(startAtArticle - 1).Take(articlesPerPage);
            rptArticles.DataBind();

            int totalPages = (int)System.Math.Ceiling((double)q.Count() / (double)articlesPerPage);
            rptPaginationLinks.DataSource = GetPaginationLinks(sectionName, pageNum, articlesPerPage, totalPages);
            rptPaginationLinks.DataBind();
        }

        int GetPageNum()
        {
            int n = 1;
            if (!int.TryParse(Request.QueryString["page"], out n))
                n = 1;
            return n;
        }

        class PaginationLink
        {
            string text;
            string href;

            public PaginationLink(string text, string href)
            {
                this.text = text;
                this.href = href;
            }

            public string Text
            {
                get { return text; }
            }

            public string Href
            {
                get { return href; }
            }
        }

        ArrayList GetPaginationLinks(string sectionName, int curPage, int articlesPerPage, int totalPages)
        {
            ArrayList paginationLinks = new ArrayList();
            int nPageLinks = int.Parse(ConfigurationManager.AppSettings["nPaginationLinks"]);
            int nPageLinksHalf = nPageLinks / 2;
            int pageNavFirst = curPage - nPageLinksHalf;
            int pageNavLast = nPageLinks;

            if (totalPages <= nPageLinks)
            {
                pageNavFirst = 1;
                pageNavLast = totalPages;
            }
            else
            {
                if (curPage + nPageLinksHalf > totalPages)
                    pageNavFirst = totalPages - nPageLinks + 1;
                if (curPage - nPageLinksHalf < 1)
                    pageNavFirst = 1;
                pageNavLast = pageNavFirst + nPageLinks - 1;
                if (pageNavLast > totalPages) pageNavLast = totalPages;
            }

            int nextPage = curPage + 1;
            if (nextPage > totalPages) nextPage = totalPages;
            int prevPage = curPage - 1;
            if (prevPage < 1) prevPage = 1;

            // Encode section name, just in case
            sectionName = HttpUtility.UrlEncode(sectionName);

            if (totalPages > 1)
            {
                paginationLinks.Add(new PaginationLink("Prev", string.Format("~/Content/{0}?page={1}", sectionName, prevPage)));
                for (int i = pageNavFirst; i <= pageNavLast; i++)
                {
                    if (i == curPage)
                    {
                        paginationLinks.Add(new PaginationLink(string.Format("<strong>{0}</strong>", i), 
                            string.Format("~/Content/{0}?page={1}", sectionName, i)));
                    }
                    else
                    {
                        paginationLinks.Add(new PaginationLink(i.ToString(), string.Format("~/Content/{0}?page={1}", sectionName, i)));
                    }
                }
                paginationLinks.Add(new PaginationLink("Next", string.Format("~/Content/{0}?page={1}", sectionName, nextPage)));
            }
            
            return paginationLinks;
        }
        
        protected void litReadMoreLink_DataBinding(object sender, EventArgs e)
        {
            string hasNoContent = Eval("HasNoContent").ToString();
            if (hasNoContent == "False")
            {
                Literal litReadMore = (Literal)sender;
                litReadMore.Text = string.Format("<p><a href=\"{0}?id={1}\">Read More...</a></p>",
                    ResolveUrl("~/ShowArticle.aspx"), Eval("ArticleId"));
            }
        }
    }
}