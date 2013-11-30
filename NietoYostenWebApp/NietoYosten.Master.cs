using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;

namespace NietoYostenWebApp
{
    public partial class NietoYosten : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            Control loginView = FindControl("LoginViewSearch");
            TextBox txtSearch = (TextBox)loginView.FindControl("txtSearch");
            if (txtSearch != null)
            {
                string url = string.Format("~/Search.aspx?query={0}", HttpUtility.UrlEncode(txtSearch.Text));
                Response.Redirect(url);
            }
        }

        protected void Menu1_MenuItemDataBound(object sender, MenuEventArgs e)
        {
            SiteMapNode node = e.Item.DataItem as SiteMapNode;

            // If the siteMapNode has a "target" attribute, use the value as the Target of the menu item
            if (!string.IsNullOrEmpty(node["target"]))
            {
                e.Item.Target = node["target"];
            }
        }
    }
}