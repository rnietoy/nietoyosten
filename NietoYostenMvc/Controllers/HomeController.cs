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
            IEnumerable<dynamic> homePageArticles = _articles.GetHomePageArticles();
            return View(homePageArticles);
        }

        [RequireLogin]
        public ActionResult ShowSection(string section)
        {
            IEnumerable<dynamic> articles = _articles.GetArticlesBySection(section);
            return View("Index", articles);
        }

        [RequireLogin]
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

        [RequireLogin]
        public ActionResult Family()
        {
            return View();
        }
    }
}
