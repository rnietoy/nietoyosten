using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Elmah;
using NietoYostenMvc.Models;

namespace NietoYostenMvc.Controllers
{
    public class ApplicationController : Controller
    {
        public dynamic CurrentUser
        {
            get
            {
                HttpCookie authCookie = HttpContext.Request.Cookies[FormsAuthentication.FormsCookieName];
                if (null == authCookie) return null;
                
                FormsAuthenticationTicket ticket = null;
                try
                {
                    ticket = FormsAuthentication.Decrypt(authCookie.Value);
                }
                catch (Exception ex)
                {
                    ErrorSignal.FromCurrentContext().Raise(ex);
                }

                if (null != ticket) return ticket.Name;
                return null;
            }
        }

        public bool IsLoggedIn
        {
            get
            {
                return CurrentUser != null;
            }
        }

        public bool CurrentUserHasRole(string role)
        {
            if (!IsLoggedIn) return false;

            var db = new Users();
            return db.UserHasRole(CurrentUser, role);
        }
    }
}
