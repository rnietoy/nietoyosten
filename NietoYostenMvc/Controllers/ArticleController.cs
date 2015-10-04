using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Elmah;
using NietoYostenMvc.Code;
using NietoYostenMvc.Models;

namespace NietoYostenMvc.Controllers
{
    public class ArticleController : ApplicationController
    {
        private readonly Articles articles;

        public ArticleController()
        {
            this.articles = new Articles();
        }

        /// <summary>
        /// Display list of articles for management
        /// </summary>
        /// <param name="id">name of section to display articles for</param>
        /// <returns>Table view of articles to manage</returns>
        [RequireLogin]
        public ActionResult Index(string id)
        {
            if (null == id)
            {
                id = "1";   // News section
            }

            string query = "SELECT ID, Name FROM Sections WHERE ParentSectionID IS NOT NULL OR Name = 'News' OR Name = 'Family' ORDER BY ParentSectionID";
            IEnumerable<SelectListItem> sections = this.articles.Query(query).
                    Select(x => new SelectListItem {Value = x.ID.ToString(), Text = x.Name });

            ArticleIndexViewModel vm = new ArticleIndexViewModel
            {
                SelectedSection = id,
                SectionOptions = sections,
                Articles = this.articles.GetArticlesBySectionId(id)
            };
            return this.View("Index", vm);
        }

        /// <summary>
        /// Edit an article
        /// </summary>
        /// <param name="id">Article id</param>
        /// <returns>Edit page for the given article</returns>
        [RequireLogin]
        [RequireRole(Role = "family")]
        public ActionResult Edit(int id)
        {
            EditArticleViewModel vm = this.articles.GetEditArticleViewModel(id);

            if (null == vm)
            {
                return this.HttpNotFound();
            }

            return this.View(vm);
        }

        [RequireLogin]
        [RequireRole(Role = "family")]
        [HttpPost]
        public ActionResult Edit(EditArticleViewModel vm)
        {
            try
            {
                this.articles.Update(
                    new
                    {
                        vm.Title,
                        IntroText = vm.IntroText ?? string.Empty,
                        Content = vm.Content ?? string.Empty,
                        ModifiedBy = this.CurrentUserID,
                        UpdatedAt = DateTime.Now,
                        vm.IsPublished
                    },
                    vm.ArticleID);
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);

                this.SetAlertMessage("Ocurrió un error al guardar el artículo.", AlertClass.AlertDanger);
                return this.View(vm);
            }

            return this.RedirectToAction("Index", new { id =  vm.SectionID });
        }

        [RequireLogin]
        [RequireRole(Role = "family")]
        public ActionResult Add(string id)
        {
            var vm = new EditArticleViewModel
            {
                Action = "Add",
                PageTitle = "Agregar artículo",
                ArticleID = 0,
                IntroText = null,
                Content = null,
                IsPublished = true,
                SectionID = int.Parse(id)
            };

            return this.View("Edit", vm);
        }

        [RequireLogin]
        [RequireRole(Role = "family")]
        [HttpPost]
        public ActionResult Add(EditArticleViewModel vm)
        {
            try
            {
                this.articles.Insert(
                    new
                    {
                        vm.Title,
                        IntroText = vm.IntroText ?? string.Empty,
                        Content = vm.Content ?? string.Empty,
                        CreatedBy = this.CurrentUserID,
                        ModifiedBy = this.CurrentUserID,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now,
                        vm.SectionID,
                        vm.IsPublished
                    });
            }
            catch (Exception ex)
            {
                ErrorSignal.FromCurrentContext().Raise(ex);

                this.SetAlertMessage("Ocurrió un error al agregar el artículo.", AlertClass.AlertDanger);

                vm.Action = "Add";
                vm.PageTitle = "Agregar artículo";
                return this.View("Edit", vm);
            }

            return this.RedirectToAction("Index", new { id = vm.SectionID });
        }
    }
}
