using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NietoYostenWebApp.admin
{
    public partial class LinkCategories : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            
        }

        protected void btnAddCategory_Click(object sender, EventArgs e)
        {
            WeblinkCategory cat = new WeblinkCategory();
            cat.Name = HttpUtility.HtmlEncode(txtNewCategory.Text);
            NietoYostenDbDataContext db = new NietoYostenDbDataContext();
            db.WeblinkCategories.InsertOnSubmit(cat);
            db.SubmitChanges();
            txtNewCategory.Text = "";
            db.Dispose();
            gvCategories.DataBind();
        }

    }
}