using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NietoYostenMvc.Code;
using NietoYostenMvc.Models;

namespace NietoYostenMvc.Controllers
{
    public class ArticleController : ApplicationController
    {
        private Articles _articles;

        public ArticleController()
        {
            _articles = new Articles();
        }

        [RequireLogin]
        public ActionResult Index(string id)
        {
            IEnumerable<dynamic> articles = _articles.GetArticlesBySection(id);
            return View("Index", articles);
        }

    }
}
