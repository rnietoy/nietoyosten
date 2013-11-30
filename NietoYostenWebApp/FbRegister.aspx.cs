using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NietoYostenWebApp
{
    public partial class FbRegister : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                fbRegisterFrame.Attributes["src"] = string.Format("{0}?client_id={1}&redirect_uri={2}&fields={3}",
                    "https://www.facebook.com/plugins/registration",
                    ConfigurationManager.AppSettings["FacebookAppId"],
                    Server.UrlEncode(ConfigurationManager.AppSettings["BaseUrl"] + "/FbRegisterBe.aspx"),
                    "name,email");
            }
        }
    }
}