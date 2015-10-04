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
        public const int PageSize = 5;
        private readonly Articles articles;

        public HomeController()
        {
            this.articles = new Articles();
        }
        public ActionResult Index()
        {
            IEnumerable<dynamic> homePageArticles = this.articles.GetHomePageArticles();
            return this.View(new ShowArticlesViewModel
            {
                Articles = homePageArticles,
                CurrentPage = 1,
                TotalPages = 1,
                SectionName = "home"
            });
        }

        [RequireLogin]
        public ActionResult ShowSection(string section)
        {
            int page = this.GetCurrentPage();
            int totalPages;
            IEnumerable<dynamic> result = this.articles.GetArticles(section, page, HomeController.PageSize, out totalPages);

            ShowArticlesViewModel vm = new ShowArticlesViewModel
            {
                Articles = result,
                CurrentPage = page,
                TotalPages = totalPages,
                SectionName = section
            };

            return View("Index", vm);
        }

        [RequireLogin]
        public ActionResult ShowArticle(int ID)
        {
            dynamic article = this.articles.GetArticle(ID);
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
    }
}
