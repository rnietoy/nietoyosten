using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
// Lucene
using Lucene.Net.Store;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Index;
using Lucene.Net.Documents;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;

namespace NietoYostenWebApp
{
    public class MyLucene
    {
        // Lucene Global variables
        public static Directory directory;
        public static StandardAnalyzer analyzer;

        static Object myLock = new Object();

        public static void InitLucene(HttpServerUtility server)
        {
            string indexDir = server.MapPath("~/App_Data/LuceneIndex");
            analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_29);
            lock (myLock)
            {
                if (directory == null)
                {
                    directory = FSDirectory.Open(new System.IO.DirectoryInfo(indexDir));
                }
            }
            //if (writer == null)
            //{
            //    if (IndexWriter.IsLocked(directory))
            //    {
            //        System.Diagnostics.Debug.WriteLine("Something left a lock in the index folder: deleting it");
            //        IndexWriter.Unlock(directory);
            //        System.Diagnostics.Debug.WriteLine("Lock deleted.");
            //    }
            //}
        }

        public static void CloseLucene()
        {
            lock (myLock)
            {
                if (directory != null)
                {
                    directory.Close();
                }
            }
        }

        public static void CreateIndexThreadProc()
        {
            lock (myLock)
            {
                IndexWriter writer = new IndexWriter(directory, analyzer, IndexWriter.MaxFieldLength.UNLIMITED);

                // Clear the index
                writer.DeleteAll();

                int count = 0;
                NietoYostenDbDataContext db = new NietoYostenDbDataContext();
                foreach (Article article in db.Articles)
                {
                    Document doc = ArticleToDocument(article);
                    writer.AddDocument(doc);
                    count++;
                }

                // Commit documents to index
                writer.Optimize();
                writer.Commit();
                writer.Close();
            }
        }

        public static void CreateIndex()
        {
            System.Threading.Thread thread = new System.Threading.Thread(CreateIndexThreadProc);
            thread.Start();
        }

        static void UpdateArticleThreadProc(Object obj)
        {
            Article article = obj as Article;
            lock (myLock)
            {
                Term term = new Term("ArticleId", article.ArticleId.ToString());
                Document doc = ArticleToDocument(article);
                IndexWriter writer = new IndexWriter(directory, analyzer, IndexWriter.MaxFieldLength.UNLIMITED);
                writer.UpdateDocument(term, doc);
                writer.Optimize();
                writer.Commit();
                writer.Close();
            }
        }

        public static void UpdateArticle(Article article)
        {
            System.Threading.Thread thread = new System.Threading.Thread(UpdateArticleThreadProc);
            thread.Start(article);
        }


        static public Document ArticleToDocument(Article article)
        {
            // Build the fields to be indexed
            string content = article.IntroText + System.Environment.NewLine + article.Content;
            string intro = content.Substring(0, content.Length > 100 ? 100 : content.Length);
            MembershipUser user = Membership.GetUser(article.CreatedBy);
            string author = user.UserName;
            string strPubDate = DateTools.DateToString(article.DateCreated, DateTools.Resolution.DAY);

            Document doc = new Document();
            doc.Add(new Field("ArticleId", article.ArticleId.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
            doc.Add(new Field("Title", article.Title, Field.Store.YES, Field.Index.ANALYZED));
            doc.Add(new Field("Content", content, Field.Store.NO, Field.Index.ANALYZED));
            doc.Add(new Field("Intro", intro, Field.Store.YES, Field.Index.NO));
            doc.Add(new Field("Author", author, Field.Store.YES, Field.Index.NO));
            doc.Add(new Field("SectionId", article.SectionId.ToString(), Field.Store.NO, Field.Index.NOT_ANALYZED));
            doc.Add(new Field("Published", article.Published.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
            doc.Add(new Field("PubDate", strPubDate, Field.Store.YES, Field.Index.NOT_ANALYZED));

            return doc;
        }

        public static Hits Search(string queryStr)
        {
            lock (myLock)
            {
                // Perform the query using Lucene
                QueryParser parser = new QueryParser(Lucene.Net.Util.Version.LUCENE_29, "Content", analyzer);

                Query parsedQuery = parser.Parse(
                    string.Format("(Title:\"{0}\" OR Content:\"{0}\")", queryStr));

                IndexSearcher searcher = new IndexSearcher(directory, true);

                BooleanQuery bquery = new BooleanQuery();
                bquery.Add(parsedQuery, BooleanClause.Occur.MUST);
                bquery.Add(new TermQuery(new Term("Published", "True")), BooleanClause.Occur.MUST);

                Hits hits = searcher.Search(bquery);
                return hits;
            }
        }

    }
}