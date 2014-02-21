using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Security;
using Elmah;
using Massive;
using NietoYostenMvc.Code;
using NietoYostenMvc.Models;

namespace NietoYostenMvc.Controllers
{
    public class AccountController : ApplicationController
    {
        private Users _users;

        public AccountController()
        {
            _users = new Users();
        }

        public ActionResult Login()
        {
            ViewBag.AlertMessage = TempData["AlertMessage"];
            ViewBag.AlertClass = TempData["AlertClass"];
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
            var user = _users.Single(where:"Email = @0", args:email);

            // Check if user exists
            if (null == user)
            {
                ViewBag.AlertMessage = "Este usuario no existe.";
                ViewBag.AlertClass = "alert-danger";
                return View();
            }

            // Regular credentials check
            if (null != user.HashedPassword && !Crypto.VerifyHashedPassword(user.HashedPassword, password))
            {
                ViewBag.AlertMessage = "El usuario y/o la contraseña no son correctos.";
                ViewBag.AlertClass = "alert-danger";
                return View();
            }

            // Check credentials against old aspnet membership password
            if (null == user.HashedPassword && !AspNetMembershipLogin(user, password))
            {
                ViewBag.AlertMessage = "El usuario y/o la contraseña no son correctos.";
                ViewBag.AlertClass = "alert-danger";
                return View();
            }

            // Check if user is approved
            if (!user.IsApproved)
            {
                ViewBag.AlertMessage = "Este usuario no ha sido aprovado aún.";
                ViewBag.AlertClass = "alert-danger";
                return View();
            }

            _users.Update(new {LastLogin = DateTime.Now}, user.ID);
            FormsAuthentication.SetAuthCookie(email, false);
            return LoginAndRedirectToReturnUrl();
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
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

            // Set password in Users.HashedPassword (if password is correct)
            _users.SetPassword(user.ID, password);

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
            var result = _users.Register(email, password, confirm);

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

                    ViewBag.AlertMessage = "Ocurrió un error al solicitar la aprovación del usuario.";
                    ViewBag.AlertClass = "alert-danger";
                    return View();
                }

                return RedirectToAction("RegistrationDone", "Account");
            }
            else
            {
                ViewBag.AlertMessage = "Ocurrió un error al registrar el usuario.";
                ViewBag.AlertClass = "alert-danger";
                return View();
            }
        }

        public ActionResult RegistrationDone()
        {
            return View();
        }

        /// <summary>
        /// Send an email notification to admins telling them that a new user
        /// has registered and needs to be approved/rejected.
        /// </summary>
        private void SendNewUserRegistrationNotificationToAdmins(string userEmail, string reason)
        {
            var message = new MailMessage();
            var fromAddress = new MailAddress("noreply@nietoyosten.com");
            message.From = fromAddress;

            var admins = _users.GetUsersInRole("admin");
            foreach (var admin in admins)
            {
                message.To.Add(admin);
            }

            message.Subject = string.Format("User {0} has requested approval for the nietoyosten.com site", userEmail);
            message.Body = "A new user has been registered on the Nieto Yosten web site. " +
                           "Please log in to http://nietoyosten.com, go to the Admin section, and then " +
                           "go to Users -> Approval Requests.\n\n" +
                           "The reason given is:\n" + reason;

            var smtpClient = new SmtpClient();
            smtpClient.Send(message);
        }

        [RequireRole(Role = "admin")]
        public ActionResult ApprovalRequests()
        {
            IEnumerable<dynamic> requests = _users.Query("SELECT U.ID, U.Email, AR.Reason FROM Users U " +
                         "INNER JOIN ApprovalRequests AR ON U.ID = AR.UserID");

            ViewBag.AlertMessage = TempData["AlertMessage"];
            ViewBag.AlertClass = TempData["AlertClass"];
            return View(requests);
        }

        [RequireRole(Role = "admin")]
        public ActionResult Approve(int id)
        {
            dynamic user = _users.Single(id);
            user.IsApproved = true;
            _users.Update(user, id);

            var db = new DynamicModel("NietoYostenDb", "ApprovalRequests");
            db.Delete(where: "UserID = @0", args: id);

            TempData["AlertMessage"] = string.Format("El usuario {0} ha sido approvado.", user.Email);
            return RedirectToAction("ApprovalRequests", "Account");
        }

        [RequireRole(Role = "admin")]
        public ActionResult Reject(int id)
        {
            dynamic user = _users.Single(id);

            var db = new DynamicModel("NietoYostenDb", "ApprovalRequests");
            db.Delete(where: "UserID = @0", args: id);
            _users.Delete(id);

            TempData["AlertMessage"] = string.Format("El usuario {0} ha sido rechazado.", user.Email);
            TempData["AlertClass"] = "alert-info";
            return RedirectToAction("ApprovalRequests", "Account");
        }
    }
}
