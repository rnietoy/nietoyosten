using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Web.Helpers;
using System.Web.Mvc;
using CodeBits;
using Elmah;
using Massive;
using NietoYostenMvc.Code;
using NietoYostenMvc.Code.FormsAuth;
using NietoYostenMvc.Models;

namespace NietoYostenMvc.Controllers
{
    public class AccountController : ApplicationController
    {
        private readonly Users users;
        private readonly PasswordResetTokens pwdResetTokens;
        private readonly IFormsAuth formsAuth;
        private readonly IMailer mailer;
        private readonly IFacebookApi facebookApi;

        public AccountController() : this(new FormsAuthWrapper(), new Mailer(), new FacebookApi())
        {
        }

        public AccountController(IFormsAuth formsAuth, IMailer mailer, IFacebookApi facebookApi)
        {
            this.users = new Users();
            this.pwdResetTokens = new PasswordResetTokens();
            this.formsAuth = formsAuth;
            this.mailer = mailer;
            this.facebookApi = facebookApi;
        }

        public ActionResult Login()
        {
            ViewBag.ReturnUrl = TempData["ReturnUrl"];
            return View();
        }

        private ActionResult LoginAndRedirectToReturnUrl()
        {
            string returnUrl = (HttpContext.Request).QueryString.Get("ReturnUrl");
            if (null != returnUrl)
            {
                return Redirect(string.Format("~/{0}", returnUrl));
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string email, string password)
        {
            var user = this.users.Single(where:"Email = @0", args:email);

            // Check if user exists
            if (null == user)
            {
                this.SetAlertMessage("Este usuario no existe.", AlertClass.AlertDanger);
                return View();
            }

            // Regular credentials check
            if (null != user.HashedPassword && !Crypto.VerifyHashedPassword(user.HashedPassword, password))
            {
                this.SetAlertMessage("El usuario y/o la contraseña no son correctos.", AlertClass.AlertDanger);
                return View();
            }

            // Check credentials against old aspnet membership password
            if (null == user.HashedPassword && !AspNetMembershipLogin(user, password))
            {
                this.SetAlertMessage("El usuario y/o la contraseña no son correctos.", AlertClass.AlertDanger);
                return View();
            }

            // Check if user is approved
            if (!user.IsApproved)
            {
                this.SetAlertMessage("Este usuario no ha sido aprovado aún.", AlertClass.AlertDanger);
                return View();
            }
            
            this.LoginUser(user);
            return LoginAndRedirectToReturnUrl();
        }

        [HttpPost]
        public ActionResult FbLogin(string signedRequest, string accessToken, string returnUrl)
        {
            if (!Request.IsAjaxRequest())
            {
                return HttpNotFound();
            }

            if (string.IsNullOrWhiteSpace(signedRequest))
            {
                return Json(
                    new NyResult
                    {
                        Success = false,
                        RedirectUrl = "/Account/Login",
                        Message = "An error occured during facebook login. Signed request is empty."
                    });
            }

            if (!FacebookUtil.ValidateSignedRequest(signedRequest))
            {
                return Json(
                    new NyResult
                    {
                        Success = false,
                        RedirectUrl = "/Account/Login",
                        Message = "An error occured during facebook login. Signed request is not valid."
                    });
            }

            long fbUserId = FacebookUtil.GetFacebookUserId(signedRequest);

            // Check if FB user id is associated with a user in our DB
            var user = this.users.Single(where: "FacebookUserID = @0", args: fbUserId);
            if (user != null)
            {
                if (user.IsApproved)
                {
                    this.LoginUser(user);
                    return Json(new NyResult
                        {
                            Success = true,
                            RedirectUrl = returnUrl
                        });
                }

                return Json(new NyResult
                    {
                        Success = false,
                        RedirectUrl = "/Account/Login",
                        Message = "This user account has not been approved yet."
                    });
            }

            string email = this.facebookApi.GetUserEmail(accessToken);

            // Merge with existing user if necessary
            if (email == null)
            {
                return Json(new NyResult
                {
                    Success = false,
                    RedirectUrl = "/Account/Login",
                    Message = "User email could not be retreived from facebook account. Check that your facebook account is connected to the NietoYosten website Facebook app " +
                              "and that you've granted email access permission to the app."
                });
            }

            var mergeUser = this.users.Single(where: "Email = @0", args: email);
            if (mergeUser != null)
            {
                this.users.Update(new { FacebookUserID = fbUserId }, mergeUser.ID);
                this.LoginUser(mergeUser);
                return Json(new NyResult
                {
                    Success = true,
                    RedirectUrl = "/",
                });
            }

            // Create new user
            string password = PasswordGenerator.Generate(12);
            NyResult registerResult = this.Register(email, password, "Facebook registration");
            return Json(registerResult);
        }

        public ActionResult Logout()
        {
            this.formsAuth.SignOut();
            return RedirectToAction("Index", "Home");
        }

        public ActionResult RecoverPassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult RecoverPassword(string email)
        {
            // First check that user exists in our database is approved
            var user = this.users.Single(where: "Email = @0", args: email);
            if (null == user || !user.IsApproved)
            {
                this.SetAlertMessage("Este usuario no existe en el sitio.", AlertClass.AlertDanger);
                return View();
            }

            string token = this.pwdResetTokens.AddToken(user.ID);

            // Send out email with new password
            var message = new MailMessage();
            var fromAddress = new MailAddress("noreply@nietoyosten.com", "NietoYosten");
            message.From = fromAddress;
            message.To.Add(email);

            message.Subject = "Password reset for nietoyosten.com";
            message.Body = string.Format("Dear user,\n\n" +
                                         "A password reset from the nietoyosten.com website has been requested for this email address. " +
                                         "Please click on the following link to reset your password: \n\n" +
                                         "http://nietoyosten.com/Account/ResetPassword?token={0}\n\n" +
                                         "If you did not request a password reset, please ignore this message.\n\n" +
                                         "Cheers,\n" +
                                         "nietoyosten.com webmaster.\n",
                                         token);

            // Set encoding to ISO-8859-1 since we are using non-English characters. If we don't
            // do this, SmtpClient will incorrectly encode the message in Base64.
            message.SubjectEncoding = Encoding.GetEncoding("ISO-8859-1");
            message.BodyEncoding = Encoding.GetEncoding("ISO-8859-1");

            this.mailer.SendMail(message);

            this.SetAlertMessage("We have sent instructions to your email on how to reset your password.", AlertClass.AlertInfo);
            return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// Verifies a password against the aspnet_Membership table.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        /// <remarks>
        /// Thanks to Paul Brown for the article that helped me write this code:
        /// http://pretzelsteelersfan.blogspot.com/2012/11/migrating-legacy-apps-to-new.html
        /// And many thanks to Malcolm Swaine for the hashing code:
        /// http://www.codeproject.com/Articles/32600/Manually-validating-an-ASP-NET-user-account-with-a
        /// </remarks>
        private bool AspNetMembershipLogin(dynamic user, string password)
        {
            var db = new DynamicModel("NietoYostenDb", "aspnet_Membership", "Email");
            var membership = db.Single((object)user.Email);

            if (!membership.IsApproved || membership.IsLockedOut) return false;

            // Compute aspnet hashed password
            byte[] bIn = Encoding.Unicode.GetBytes(password);
            byte[] bSalt = Convert.FromBase64String(membership.PasswordSalt);
            byte[] bAll = new byte[bSalt.Length + bIn.Length];
            byte[] bRet = null;
            Buffer.BlockCopy(bSalt, 0, bAll, 0, bSalt.Length);
            Buffer.BlockCopy(bIn, 0, bAll, bSalt.Length, bIn.Length);

            HashAlgorithm s = HashAlgorithm.Create("SHA1");
            bRet = s.ComputeHash(bAll);
            string sha1HashedPassword = Convert.ToBase64String(bRet);
            
            // Check if password is correct
            if (sha1HashedPassword != membership.Password) return false;

            // Set password in this.users.HashedPassword (if password is correct)
            this.users.SetPassword(user.ID, password);

            return true;
        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(string email, string password, string confirm, string reason)
        {
            var result = this.users.Register(email, password, confirm);

            if (result.Success)
            {
                // Add approval request and send a notification email
                try
                {
                    var db = new DynamicModel("NietoYostenDb", "ApprovalRequests");
                    db.Insert(new {UserID = result.User.ID, Reason = reason});

                    SendNewUserRegistrationNotificationToAdmins(email, reason);
                }
                catch (Exception ex)
                {
                    ErrorSignal.FromCurrentContext().Raise(ex);

                    this.SetAlertMessage("Ocurrió un error al solicitar la aprovación del usuario.", AlertClass.AlertDanger);
                    return View();
                }

                NyUtil.SetAlertMessage(this, result.Message, AlertClass.AlertSuccess);
                return RedirectToAction("Index", "Home");
            }

            NyUtil.SetAlertMessage(this, result.Message, AlertClass.AlertDanger);
            return View();
        }

        /// <summary>
        /// Send an email notification to admins telling them that a new user
        /// has registered and needs to be approved/rejected.
        /// </summary>
        private void SendNewUserRegistrationNotificationToAdmins(string userEmail, string reason)
        {
            var message = new MailMessage();
            var fromAddress = new MailAddress("noreply@nietoyosten.com", "NietoYosten");
            message.From = fromAddress;

            var admins = this.users.GetUsersInRole("admin");
            foreach (var admin in admins)
            {
                message.To.Add(admin);
            }

            message.Subject = string.Format("User approval request for {0}", userEmail);
            message.Body = string.Format("User {0} has requested approval to the nietoyosten.com site. " +
                                         "Please go to http://nietoyosten.com/Account/ApprovalRequests " +
                                         "to approve or reject this user.\n\n" +
                                         "The reason given by the user is:\n{1}", userEmail, reason);

            this.mailer.SendMail(message);
        }

        [RequireRole(Role = "admin")]
        public ActionResult ApprovalRequests()
        {
            IEnumerable<dynamic> requests = this.users.Query("SELECT U.ID, U.Email, AR.Reason FROM this.users U " +
                         "INNER JOIN ApprovalRequests AR ON U.ID = AR.UserID");

            return View(requests);
        }

        [RequireRole(Role = "admin")]
        public ActionResult Approve(int id)
        {
            dynamic user = this.users.Single(id);
            user.IsApproved = true;
            this.users.Update(user, id);

            var db = new DynamicModel("NietoYostenDb", "ApprovalRequests");
            db.Delete(where: "UserID = @0", args: id);

            NyUtil.SetAlertMessage(this, string.Format("El usuario {0} ha sido approvado.", user.Email));
            return RedirectToAction("ApprovalRequests", "Account");
        }

        [RequireRole(Role = "admin")]
        public ActionResult Reject(int id)
        {
            dynamic user = this.users.Single(id);

            var db = new DynamicModel("NietoYostenDb", "ApprovalRequests");
            db.Delete(where: "UserID = @0", args: id);
            this.users.Delete(id);

            NyUtil.SetAlertMessage(this, string.Format("El usuario {0} ha sido rechazado.", user.Email), AlertClass.AlertInfo);
            return RedirectToAction("ApprovalRequests", "Account");
        }

        /// <summary>
        /// Reset a user's password
        /// </summary>
        /// <param name="id">ID of user to reset password for</param>
        /// <returns></returns>
        public ActionResult ResetPassword()
        {
            string token = (HttpContext.Request).QueryString.Get("token");

            int userId;
            if (!ValidatePasswordResetToken(token, out userId))
            {
                this.SetAlertMessage("The password reset link is not valid. Please request another password reset.", AlertClass.AlertDanger);
                return RedirectToAction("RecoverPassword", "Account");
            }

            dynamic model = new ExpandoObject();
            model.UserID = userId;
            model.Token = token;

            return View(model);
        }

        [HttpPost]
        public ActionResult ResetPassword(string token, string password, string confirm)
        {
            int userId;
            if (!ValidatePasswordResetToken(token, out userId))
            {
                this.SetAlertMessage("Could not validate password reset link while trying to reset password. Please request another password reset.", AlertClass.AlertDanger);
                return RedirectToAction("RecoverPassword", "Account");
            }

            dynamic result = this.users.CheckPasswordStrength(password, confirm);
            if (!result.Success)
            {
                NyUtil.SetAlertMessage(this, result.Message, AlertClass.AlertDanger);
                return View();
            }

            this.users.SetPassword(userId, password);

            // Delete the token(s) for this user from the DB so it can no longer be used again
            this.pwdResetTokens.Delete(where: "UserID = " + userId);

            this.SetAlertMessage("Your password has been updated. Please log in with your new password.", AlertClass.AlertInfo);
            return RedirectToAction("Login", "Account");
        }

        private bool ValidatePasswordResetToken(string token, out int userId)
        {
            userId = 0;

            if (string.IsNullOrWhiteSpace(token))
            {
                return false;
            }

            dynamic tokenEntry = this.pwdResetTokens.Single(where: string.Format("HashedToken='{0}'", Crypto.Hash(token)));
            if (null == tokenEntry)
            {
                return false;
            }

            userId = tokenEntry.UserID;

            // Return false if all matching tokens are expired
            if ((DateTime.UtcNow - tokenEntry.CreatedAt) > TimeSpan.FromMinutes(60))
            {
                return false;
            }
            
            return true;
        }

        /// <summary>
        /// Login user, record login in DB
        /// </summary>
        /// <param name="user">User to login</param>
        private void LoginUser(dynamic user)
        {
            this.users.Update(new { LastLogin = DateTime.Now }, user.ID);
            this.formsAuth.SetAuthCookie(user.Email, false);
        }

        private NyResult Register(string email, string password, string reason)
        {
            var result = this.users.Register(email, password, password);
            
            if (result.Success)
            {
                // Add approval request and send a notification email
                try
                {
                    var db = new DynamicModel("NietoYostenDb", "ApprovalRequests");
                    db.Insert(new { UserID = result.User.ID, Reason = reason });
                    SendNewUserRegistrationNotificationToAdmins(email, reason);
                }
                catch (Exception ex)
                {
                    ErrorSignal.FromCurrentContext().Raise(ex);
                    return new NyResult
                    {
                        Success = false,
                        Message = "Ocurrió un error al solicitar la aprovación del usuario."
                    };
                }

                return new NyResult
                {
                    Success = true,
                    Message = result.Message
                };
            }

            return new NyResult
            {
                Success = false,
                Message = result.Message
            };
        }
    }
}
