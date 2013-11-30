using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NietoYostenWebApp.admin
{
    public partial class AdminLinks : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string catId = Page.Request.QueryString["catId"];
                ddlLinkCategories.SelectedValue = catId;
            }
        }

        protected void ddlLinkCategories_SelectedIndexChanged(object sender, EventArgs e)
        {
            gvLinks.SelectedIndex = 0;
        }

        protected void btnAddLink_Click(object sender, EventArgs e)
        {
            Response.Redirect(string.Format("~/admin/AddLink.aspx?catId={0}", ddlLinkCategories.SelectedValue));
        }
    }
}