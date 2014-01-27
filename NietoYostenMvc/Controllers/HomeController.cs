using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Massive;
using NietoYostenMvc.Code;
using NietoYostenMvc.Models;

namespace NietoYostenMvc.Controllers
{
    public class HomeController : ApplicationController
    {
        private Articles _articles;
        public HomeController()
        {
            _articles = new Articles();
        }
        public ActionResult Index()
        {
            IEnumerable<dynamic> homePageArticles;
            // Get home page articles
            if (null == CurrentUser)
            {
                homePageArticles = _articles.GetHomePageArticles(true);
            }
            else
            {
                homePageArticles = _articles.GetHomePageArticles(false);
            }

            return View(homePageArticles);
        }

        public ActionResult ShowSection(string section)
        {
            bool isAnonymous = (null == CurrentUser);
            IEnumerable<dynamic> articles = _articles.GetArticlesBySection(section, isAnonymous);

            return View("Index", articles);
        }

        public ActionResult ShowArticle(int ID)
        {
            dynamic article = _articles.GetArticle(ID);
            if (null == article)
            {
                return HttpNotFound();
            }

            if (!article.IsPublished)
            {
                return HttpNotFound();
            }

            if (!article.IsPublic && null == CurrentUser)
            {
                TempData["ReturnUrl"] = HttpContext.Request.RawUrl;
                return RedirectToAction("Login", "Account");
            }
            return View(article);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            ViewBag.Message = HttpContext.User.Identity.IsAuthenticated
                ? "You are authenticated"
                : "You are not authenticated";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [RequireRole(Role = "friend")]
        public ActionResult Family()
        {
            return View();
        }
    }
}
