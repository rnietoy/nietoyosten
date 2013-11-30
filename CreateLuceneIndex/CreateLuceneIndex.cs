using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Configuration;
using Lucene.Net.Store;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Index;
using Lucene.Net.Documents;

namespace CreateLuceneIndex
{
    class CreateLuceneIndex
    {
        static SqlConnection dbConn;

        static void Main(string[] args)
        {
            // Initialize Lucene
            Directory directory = FSDirectory.Open(new System.IO.DirectoryInfo("LuceneIndex"));
            StandardAnalyzer analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_29);
            IndexWriter writer = new IndexWriter(directory, analyzer, IndexWriter.MaxFieldLength.UNLIMITED);

            // Open DB connection
            OpenConnections();

            // Insert documents into the index
            
            // Clear the index
            writer.DeleteAll();
            string sql = "SELECT a.ArticleId, a.Title, a.IntroText, a.Content, a.Published, a.DateCreated, " + 
                "u.UserName, a.SectionId FROM Article AS a INNER JOIN aspnet_Users AS u ON " +
                "a.CreatedBy = u.UserId";

            SqlCommand cmd = new SqlCommand(sql, dbConn);
            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                // Extract the fields to be indexed
                int articleId = reader.GetInt32(reader.GetOrdinal("ArticleId"));
                string title = reader["Title"].ToString();
                string content = reader["IntroText"].ToString() + reader["Content"].ToString();
                string intro = content.Substring(0, content.Length > 100 ? 100 : content.Length);
                string author = reader["UserName"].ToString();
                int sectionId = reader.GetInt32(reader.GetOrdinal("SectionId"));
                bool published = reader.GetBoolean(reader.GetOrdinal("Published"));
                DateTime pubDate = reader.GetDateTime(reader.GetOrdinal("DateCreated"));
                string strPubDate = DateTools.DateToString(pubDate, DateTools.Resolution.DAY);

                Document doc = new Document();
                doc.Add(new Field("ArticleId", articleId.ToString(), Field.Store.YES, Field.Index.NO));
                doc.Add(new Field("Title", title, Field.Store.YES, Field.Index.ANALYZED));
                doc.Add(new Field("Content", content, Field.Store.NO, Field.Index.ANALYZED));
                doc.Add(new Field("Intro", title, Field.Store.YES, Field.Index.NO));
                doc.Add(new Field("Author", author, Field.Store.YES, Field.Index.NO));
                doc.Add(new Field("SectionId", sectionId.ToString(), Field.Store.NO, Field.Index.NOT_ANALYZED));
                doc.Add(new Field("Published", published.ToString(), Field.Store.NO, Field.Index.NOT_ANALYZED));
                doc.Add(new Field("PubDate", strPubDate, Field.Store.YES, Field.Index.NOT_ANALYZED));
                
                writer.AddDocument(doc);
            }

            // Close everything, shut down.
            writer.Optimize();
            writer.Commit();
            writer.Close();

            dbConn.Close();
        }

        static void OpenConnections()
        {
            // Open connection to ASP.NET DB membership database
            ConnectionStringSettings aspNetDbSettings = ConfigurationManager.ConnectionStrings["NietoYostenDb"];
            dbConn = new SqlConnection(aspNetDbSettings.ConnectionString);
            dbConn.Open();
        }
    }
}
