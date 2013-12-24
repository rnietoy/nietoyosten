using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Massive;

namespace NietoYostenMvc.Models
{
    public class Articles : DynamicModel
    {
        public Articles() : base("NietoYostenDb", "Articles", "ID") {}

        public IEnumerable<dynamic> GetHomePageArticles(bool isUserAnonymous)
        {
            string query = "select A.Title, A.IntroText, A.Content, A.CreatedAt, U.Email from Articles A " +
                           "inner join HomePageArticles H on H.ArticleID = A.ID " +
                           "inner join Users U ON U.ID = A.CreatedBy ";

            if (isUserAnonymous)
            {
                query = query + "where A.IsPublic = 1";
            }

            return this.Query(query);
        }

        public dynamic GetArticle(int ID)
        {
            IEnumerable<dynamic> results = this.Query("select A.Title, A.IntroText, A.Content, A.CreatedAt, A.IsPublished, A.IsPublic, U.Email " +
                           "from Articles A " +
                           "inner join Users U ON U.ID = A.CreatedBy " +
                           "where A.ID = @0", ID);
            return results.FirstOrDefault();
        }

        public IEnumerable<dynamic> GetArticlesBySection(string sectionName, bool isUserAnonymous = true)
        {
            string query = "select A.Title, A.IntroText, A.Content, A.CreatedAt, U.Email from Articles A " +
                           "inner join Sections S on S.ID = A.SectionID " +
                           "inner join Users U ON U.ID = A.CreatedBy " +
                           "where A.IsPublished = 1 and S.Name = @0 ";

            if (isUserAnonymous)
            {
                query = query + "and A.IsPublic = 1";
            }

            return this.Query(query, sectionName);
        }
    }
}