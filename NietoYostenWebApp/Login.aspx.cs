using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using NietoYostenWebApp.Code;

namespace NietoYostenWebApp
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Set facebook app id on form
                fbAppId.Value = ConfigurationManager.AppSettings["FacebookAppId"];

                if (!string.IsNullOrEmpty(Request.Form["signed_request"]))
                {
                    var signedRequest = Request.Form["signed_request"];

                    bool isValid = FacebookUtil.ValidateSignedRequest(signedRequest);
                    if (!isValid)
                    {
                        return;
                    }

                    string fbUserId = FacebookUtil.GetFacebookUserId(signedRequest);

                    using (var db = new NietoYostenDbDataContext())
                    {
                        var userNameQuery = from u in db.aspnet_Users
                                       join fbu in db.FacebookUserIds on u.UserId equals fbu.UserId
                                            where fbu.FbUid == fbUserId
                                       select u.UserName;

                        var userName = userNameQuery.FirstOrDefault();

                        // If FB login was successful but user is not in DB, then redirect to Registration page
                        if (null == userName)
                        {
                            Response.Redirect("~/FbRegister.aspx");
                            return;
                        }

                        // Otherwise log in user
                        FormsAuthentication.RedirectFromLoginPage(userName, true);
                    }
                }
            }
            else
            {
                
            }
        }
    }
}