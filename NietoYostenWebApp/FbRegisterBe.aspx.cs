using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using Newtonsoft.Json.Linq;
using NietoYostenWebApp.Code;

namespace NietoYostenWebApp
{
    public partial class FbRegisterBe : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (!string.IsNullOrEmpty(Request.Form["signed_request"]))
                {
                    string signedRequest = Request.Form["signed_request"];

                    if (!FacebookUtil.ValidateSignedRequest(signedRequest)) return;

                    var regInfo = FacebookUtil.GetRegistrationInfo(signedRequest);

                    // Check if user is already registered on the site (just re-connecting to the app)
                    bool alreadyRegistered = false;
                    string userName = null;

                    using (var db = new NietoYostenDbDataContext())
                    {
                        if (db.FacebookUserIds.Any(x => x.FbUid == regInfo.UserId))
                        {
                            // Nothing to do here other than log the user in
                            alreadyRegistered = true;
                            userName = FacebookUtil.GetUserNameFromFbUid(db, regInfo.UserId);
                        }
                    }
                    if (alreadyRegistered)
                    {
                        FormsAuthentication.RedirectFromLoginPage(userName, true);
                        return;
                    }
                    
                    // Merge with site user if necessary
                    var existingUser = Membership.GetUserNameByEmail(regInfo.Email);
                    if (!string.IsNullOrEmpty(existingUser))
                    {
                        var aspnetUser = Membership.GetUser(existingUser);
                        var userId = new FacebookUserId()
                            {
                                UserId = (Guid) aspnetUser.ProviderUserKey,
                                FbUid = regInfo.UserId
                            };
                        using (var db = new NietoYostenDbDataContext())
                        {
                            db.FacebookUserIds.InsertOnSubmit(userId);
                            db.SubmitChanges();
                        }

                        // Login user after doing the merge
                        FormsAuthentication.RedirectFromLoginPage(aspnetUser.UserName, true);
                        return;
                    }
                    else
                    {
                        // Create new user
                        MembershipCreateStatus createStatus;
                        var password = "Magneto1";
                        var newUser = Membership.CreateUser(regInfo.UserName, password, regInfo.Email, null, null, true, Guid.NewGuid(), out createStatus);

                        // Grant role to user: default to friend for now
                        // TODO: Grant role based on membership to NY family Facebook group
                        Roles.AddUserToRole(newUser.UserName, "friend");

                        switch (createStatus)
                        {
                            case MembershipCreateStatus.Success:
                                var userId = new FacebookUserId()
                                    {
                                        UserId = (Guid) newUser.ProviderUserKey,
                                        FbUid = regInfo.UserId
                                    };
                                using (var db = new NietoYostenDbDataContext())
                                {
                                    db.FacebookUserIds.InsertOnSubmit(userId);
                                    db.SubmitChanges();
                                }
                                FormsAuthentication.RedirectFromLoginPage(newUser.UserName, true);
                                break;

                            default:
                                Response.Redirect("FbRegister.aspx?facebook_result=success");
                                break;
                        }    
                    }
                }
            }
        }
    }
}