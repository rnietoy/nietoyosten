using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NietoYostenWebApp
{
    public partial class ShowArticle : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                NietoYostenDbDataContext db = new NietoYostenDbDataContext();
                int articleId = 0;
                if (!int.TryParse(Page.Request.QueryString["id"], out articleId)) return;

                Article article;
                if (db.Articles.Count(a => a.ArticleId == articleId) == 1)
                {
                    article = db.Articles.SingleOrDefault(a => a.ArticleId == articleId);
                }
                else
                {
                    return;
                }
                aspnet_User user = db.aspnet_Users.Single(u => u.UserId == article.CreatedBy);

                litArticleDate.Text = string.Format("{0:D}<br />Created by {1}", article.DateCreated, user.UserName);
                litTitle.Text = article.Title;
                litContent.Text = article.IntroText + article.Content;
            }
        }
    }
}