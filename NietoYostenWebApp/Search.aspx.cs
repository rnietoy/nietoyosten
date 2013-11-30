using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Index;
using System.Globalization;

namespace NietoYostenWebApp
{
    public partial class Search : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string queryStr = Request.QueryString["query"];
                Hits hits = MyLucene.Search(queryStr);

                // Iterate over the results and build the list
                int nResults = hits.Length();
                List<SearchResult> results = new List<SearchResult>();
                for (int i = 0; i < nResults; i++)
                {
                    Document foundDoc = hits.Doc(i);
                    float score = hits.Score(i);
                    SearchResult r = new SearchResult();
                    r.ArticleId = foundDoc.Get("ArticleId");
                    r.Title = foundDoc.Get("Title");
                    r.Author = foundDoc.Get("Author");
                    r.Date = TransformDate(foundDoc.Get("PubDate"));
                    results.Add(r);
                }

                // Build resulting list to repeater control
                rptSearchResults.DataSource = results;
                rptSearchResults.DataBind();
            }
        }

        string TransformDate(string luceneDate)
        {
            string format = "yyyyMMdd";
            CultureInfo provider = CultureInfo.InvariantCulture;
            DateTime dt = DateTime.ParseExact(luceneDate, format, provider);
            return dt.ToString("D");
        }

        class SearchResult
        {
            string articleId;
            string title;
            string author;
            string date;
            string section;

            public SearchResult()
            {
                // do nothing
            }

            public string ArticleId
            {
                get { return articleId; }
                set { articleId = value; }
            }

            public string Title
            {
                get { return title; }
                set { title = value; }
            }

            public string Author
            {
                get { return author; }
                set { author = value; }
            }

            public string Date
            {
                get { return date; }
                set { date = value; }
            }

            public string Section
            {
                get { return section; }
                set { section = value; }
            }
        }
    }
}