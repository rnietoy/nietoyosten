using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NietoYostenWebApp
{
    public partial class Links : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                NietoYostenDbDataContext db = new NietoYostenDbDataContext();
                WeblinkCategory wcat = db.WeblinkCategories.Single(w => w.Id.ToString() == Request.QueryString["catId"]);
                litHeading.Text = "Links - " + wcat.Name;
            }
        }
    }
}