using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NietoYostenWebApp.admin
{
    public partial class AddLink : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string catId = Page.Request.QueryString["catId"];
                ddlLinkCategories.SelectedValue = catId;
            }
        }

        protected void LinkDetails_ItemInserted(object sender, DetailsViewInsertedEventArgs e)
        {
            Response.Redirect(string.Format("~/admin/AdminLinks.aspx?catId={0}",
                HttpUtility.UrlEncode(ddlLinkCategories.SelectedValue)));
        }

        protected void dsLink_Inserting(object sender, LinqDataSourceInsertEventArgs e)
        {
            Weblink link = (Weblink)e.NewObject;
            link.CategoryId = int.Parse(ddlLinkCategories.SelectedValue);
        }
    }
}