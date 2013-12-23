using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace NietoYostenMvc.Controllers
{
    public class ApplicationController : Controller
    {
        public dynamic CurrentUser
        {
            get
            {
                var authCookie = HttpContext.Request.Cookies[FormsAuthentication.FormsCookieName];
                if (null != authCookie)
                {
                    var ticket = FormsAuthentication.Decrypt(authCookie.Value);
                    return ticket.Name;
                }
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

    }
}
