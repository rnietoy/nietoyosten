﻿using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Elmah;
using NietoYostenMvc.Models;

namespace NietoYostenMvc.Controllers
{
    public class ApplicationController : Controller
    {
        public static dynamic GetCurrentUser(HttpContextBase context)
        {
            HttpCookie authCookie = context.Request.Cookies[FormsAuthentication.FormsCookieName];
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
        public dynamic CurrentUser
        {
            get
            {
                return GetCurrentUser(HttpContext);
            }
        }

        public int CurrentUserID
        {
            get
            {
                var users = new Users();
                return users.Scalar("SELECT ID FROM Users WHERE Email=@0", CurrentUser);
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
