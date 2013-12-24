using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Security;
using Massive;
using NietoYostenMvc.Models;

namespace NietoYostenMvc.Controllers
{
    public class AccountController : ApplicationController
    {
        private Users _users;
        private string _returnUrl = null;

        public AccountController()
        {
            _users = new Users();
        }

        public ActionResult Login()
        {
            ViewBag.ReturnUrl = HttpContext.Request.QueryString.Get("ReturnUrl");
            return View();
        }

        private ActionResult GetLoginRedirectAction()
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
            var user = _users.Single("Email = @0", (object)email);

            // Regular credentials check
            if (null != user && null != user.HashedPassword && Crypto.VerifyHashedPassword(user.HashedPassword, password))
            {
                FormsAuthentication.SetAuthCookie(email, false);
                return GetLoginRedirectAction();
            }

            // Check credentials against old aspnet membership password
            if (AspNetMembershipLogin(user, password))
            {
                FormsAuthentication.SetAuthCookie(email, false);
                return GetLoginRedirectAction();
            }

            return View();
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
    }
}
