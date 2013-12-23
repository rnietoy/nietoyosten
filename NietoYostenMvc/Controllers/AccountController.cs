using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Security;
using NietoYostenMvc.Models;

namespace NietoYostenMvc.Controllers
{
    public class AccountController : Controller
    {
        private Users _users;

        public AccountController()
        {
            _users = new Users();
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string email, string password)
        {
            // Check credentials here.
            return View();
        }
    }
}
