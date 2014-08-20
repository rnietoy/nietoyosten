using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;
using System.Web.Mvc;
using Massive;

namespace NietoYostenMvc.Models
{
    public class Articles : DynamicModel
    {
        public Articles() : base("NietoYostenDb", "Articles", "ID") {}

        public IEnumerable<dynamic> GetHomePageArticles()
        {
            string query = "select A.Title, A.IntroText, A.Content, A.CreatedAt, U.Email from Articles A " +
                           "inner join HomePageArticles H on H.ArticleID = A.ID " +
                           "inner join Users U ON U.ID = A.CreatedBy";

            return this.Query(query);
        }

        public dynamic GetArticle(int ID)
        {
            IEnumerable<dynamic> results = this.Query("select A.Title, A.IntroText, A.Content, A.CreatedAt, A.IsPublished, U.Email " +
                           "from Articles A " +
                           "inner join Users U ON U.ID = A.CreatedBy " +
                           "where A.ID = @0", ID);
            return results.FirstOrDefault();
        }

        public EditArticleViewModel GetEditArticleViewModel(int ID)
        {
            IEnumerable<dynamic> results = this.Query(
                "SELECT A.Title, A.IntroText, A.Content, A.CreatedAt, A.IsPublished, S.ID AS SectionID " +
                "FROM Articles A " +
                "INNER JOIN Sections S ON S.ID = A.SectionID " +
                "WHERE A.ID = @0",
                ID);

            dynamic article = results.FirstOrDefault();
            if (null == article) return null;

            return new EditArticleViewModel
            {
                Action = "Edit",
                PageTitle = "Editar artículo",
                ArticleID = ID,
                Title = article.Title,
                Content = article.Content,
                IntroText = article.IntroText,
                SectionID = article.SectionID,
                IsPublished = article.IsPublished
            };
        }

        public IEnumerable<dynamic> GetArticles(string sectionName, int page, int pageSize, out int totalPages)
        {
            string query = "SELECT A.ID, A.Title, A.IntroText, A.Content, A.CreatedAt, U.Email FROM Articles A " +
                           "INNER JOIN Sections S ON S.ID = A.SectionID " +
                           "INNER JOIN Users U ON U.ID = A.CreatedBy " +
                           "WHERE A.IsPublished = 1 AND S.Name = @0";

            dynamic result = this.Paged(
                sql: query,
                orderBy: "CreatedAt DESC",
                primaryKey: "ID",
                pageSize: pageSize,
                currentPage: page, 
                args: sectionName);

            totalPages = result.TotalPages;
            return result.Items;
        }

        public IEnumerable<dynamic> GetArticlesBySectionId(string sectionId)
        {
            string query = "SELECT A.ID, A.Title, A.IntroText, A.Content, A.CreatedAt, A.IsPublished, U.Email FROM Articles A " +
                           "INNER JOIN Sections S ON S.ID = A.SectionID " +
                           "INNER JOIN Users U ON U.ID = A.CreatedBy " +
                           "WHERE S.ID = @0 " +
                           "ORDER BY A.CreatedAt DESC";

            return this.Query(query, sectionId);
        }
    }

    public class ShowArticlesViewModel
    {
        public string SectionName { get; set; }
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
        public IEnumerable<dynamic> Articles { get; set; }
    }

    public class ArticleIndexViewModel
    {   
        /// <summary>
        /// ID of selected section
        /// </summary>
        public string SelectedSection { get; set; }
        public IEnumerable<SelectListItem> SectionOptions { get; set; }
        public IEnumerable<dynamic> Articles { get; set; }
    }

    public class EditArticleViewModel
    {
        /// <summary>
        /// Set to "Edit" or "Add"
        /// </summary>
        public string Action { get; set; }
        public string PageTitle { get; set; }
        public int ArticleID { get; set; }
        public int SectionID { get; set; }
        public string SectionName { get; set; }
        public string Title { get; set; }
        
        [AllowHtml]
        public string IntroText { get; set; }
        
        [AllowHtml]
        public string Content { get; set; }
        public bool IsPublished { get; set; }
    }
}