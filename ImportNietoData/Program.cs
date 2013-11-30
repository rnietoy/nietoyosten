using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.VisualBasic.FileIO;
using FileHelpers;
using MySql.Data.MySqlClient;
using MySql.Data.Types;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Configuration;

namespace ImportNietoData
{
    class Program
    {
        //static SqlConnection sqlConn;
        static MySqlConnection mySqlConn;
        static SqlConnection aspNetDbConn;

        static Dictionary<int, string> mamboUserMap = new Dictionary<int, string> {
            { 62, "rafa" },
            { 63, "marcos" },
            { 64, "papi" },
            { 72, "ani" },
            { 76, "cristy" },
            { 77, "helen" },
            { 82, "papi" }};

        // Maps section numbers from Mambo's MySQL databse to section numbers in the new NietoYosten database.
        static Dictionary<int, int> mamboCatSectionMap = new Dictionary<int, int> {
            { 1, 1 },
            { 58, 1},
            { 68, 1},
            { 71, 1},
            { 32, 10},
            { 33, 11},
            { 34, 9},
            { 39, 12},
            { 40, 13},
            { 44, 7},
            { 45, 16},
            { 46, 14},
            { 47, 15},
            { 60, 3},
            { 65, 6},
            { 66, 5},
            { 67, 4}};

        static Dictionary<int, int> webLinkCategoryMap = new Dictionary<int, int> {
            { 16, 1},
            { 27, 2},
            { 28, 3},
            { 29, 4},
            { 31, 5}};

        static void OpenConnections()
        {
            // Open connection to ASP.NET DB membership database
            ConnectionStringSettings aspNetDbSettings = ConfigurationManager.ConnectionStrings["NietoYostenDb"];
            aspNetDbConn = new SqlConnection(aspNetDbSettings.ConnectionString);
            aspNetDbConn.Open();

            // Open connection to MySQL database
            string mySqlConnStr = "server=localhost;user=root;database=stuffush_mambony;port=3306;password=Magneto1";
            mySqlConn = new MySqlConnection(mySqlConnStr);
            mySqlConn.Open();
        }

        static object GetUserId(string userName)
        {
            string sql = "SELECT UserId FROM aspnet_Users WHERE UserName=@UserName";
            SqlCommand cmd = new SqlCommand(sql, aspNetDbConn);
            cmd.Parameters.AddWithValue("@UserName", userName);
            object userId = cmd.ExecuteScalar();
            return userId;
        }

        static void DeleteArticles()
        {
            // Delete HomePageArticles table first due to foreign key constraints
            SqlCommand cmdDeleteHomeArticles = new SqlCommand("TRUNCATE TABLE HomePageArticles", aspNetDbConn);
            cmdDeleteHomeArticles.ExecuteNonQuery();

            //string sql = "TRUNCATE TABLE Article";
            string sql = "DELETE FROM Article";
            SqlCommand cmd = new SqlCommand(sql, aspNetDbConn);
            cmd.ExecuteNonQuery();

            // Reset primary key counter
            sql = "DBCC CHECKIDENT(Article, RESEED, 0);";
            SqlCommand cmdResetCounter = new SqlCommand(sql, aspNetDbConn);
            cmdResetCounter.ExecuteNonQuery();
        }

        static void Main(string[] args)
        {
            if (args.Length == 0) return;

            // TextEncoding();

            OpenConnections();

            foreach (string arg in args)
            {
                switch (arg)
                {
                    case "ImportLinks":
                        ImportLinks();
                        break;

                    case "ImportLaRedaccion":
                        ImportLaRedaccion();
                        break;
                }
            }

            // Close DB connections
            mySqlConn.Close();
            aspNetDbConn.Close();
        }

        static void ImportArticles()
        {
            DeleteArticles();

            string readSql = "SELECT id, title, introtext, `fulltext`, created, created_by, modified, modified_by, catid, state FROM jos_content " +
                "WHERE created_by IN (62,63,64,72,77,76,82) AND catid<>0";
            MySqlCommand cmd = new MySqlCommand(readSql, mySqlConn);
            MySqlDataReader reader = cmd.ExecuteReader();

            // Transfer data
            while (reader.Read())
            {
                Console.WriteLine("Importing article id {0}", reader[0]);

                // Create row
                string insertSql = "INSERT INTO Article (Title, IntroText, Content, SectionId, CreatedBy, ModifiedBy, DateCreated, DateModified, Published) " +
                    "VALUES (@Title, @IntroText, @Content, @SectionId, @CreatedBy, @ModifiedBy, @DateCreated, @DateModified, @Published)";
                SqlCommand insertCmd = new SqlCommand(insertSql, aspNetDbConn);

                // Here the mapping occurs
                insertCmd.Parameters.AddWithValue("Title", FixString(reader["title"].ToString()));
                string introText = FixString(reader["introtext"].ToString());
                string content = FixString(reader["fulltext"].ToString());
                insertCmd.Parameters.AddWithValue("IntroText", introText);
                insertCmd.Parameters.AddWithValue("Content", content);
                insertCmd.Parameters.AddWithValue("CreatedBy", GetUserId(mamboUserMap[reader.GetInt32(5)]));
                if (reader.GetInt32(7) == 0)
                {
                    insertCmd.Parameters.AddWithValue("ModifiedBy", DBNull.Value);
                }
                else
                {
                    insertCmd.Parameters.AddWithValue("ModifiedBy", GetUserId(mamboUserMap[reader.GetInt32(7)]));
                }
                insertCmd.Parameters.AddWithValue("DateCreated", MySqlDateTimeToSqlDateTime(reader.GetMySqlDateTime(4)));
                insertCmd.Parameters.AddWithValue("DateModified", MySqlDateTimeToSqlDateTime(reader.GetMySqlDateTime(6)));
                insertCmd.Parameters.AddWithValue("SectionId", mamboCatSectionMap[reader.GetInt32(8)]);
                insertCmd.Parameters.AddWithValue("Published", reader.GetBoolean("state"));

                // Insert row
                insertCmd.ExecuteNonQuery();
            }

            FillHomePageArticles();
        }

        static SqlDateTime MySqlDateTimeToSqlDateTime(MySqlDateTime mySqlDateTime)
        {
            if (!mySqlDateTime.IsValidDateTime) return SqlDateTime.Null;
            return mySqlDateTime.GetDateTime();
        }

        /// <summary>
        /// Checks if the text in the string is not encoded correctly (has non-english characters that don't display correctly
        /// If not, it re-encodes the text in UTF8. It returns a string with the correct text.
        /// </summary>
        /// <param name="mojibake"></param>
        /// <returns></returns>
        static string FixString(string mojibake)
        {
            byte[] bytes = Encoding.Default.GetBytes(mojibake);
            if (Has2ByteUtf8Code(bytes))
            {
                return Encoding.UTF8.GetString(bytes);
            }
            return mojibake;
        }

        static bool Has2ByteUtf8Code(byte[] bytes)
        {
            for (int i = 0; i < bytes.Length - 1; i++)
            {
                if (bytes[i] >= 0x80 && bytes[i + 1] >= 0x80) return true;
            }
            return false;
        }

        static void TextEncoding()
        {
            byte[] bytes = new byte[] { 0x22, 0x6e, 0xc3, 0xba, 0x6d, 0x65, 0x72, 0x6f };
            string utfStr = Encoding.UTF8.GetString(bytes);
            Console.WriteLine(utfStr);
        }

        static void FillHomePageArticles()
        {
            string sql = "INSERT INTO HomePageArticles (Position, Enabled, ArticleId) " +
                "VALUES (1, 1, 88), (2,1,59), (3,1,124), (4,1,6);";
            SqlCommand cmd = new SqlCommand(sql, aspNetDbConn);
            cmd.ExecuteNonQuery();
        }

        static void ImportLinks()
        {
            string sql = "SELECT id, title, url, description, catid from jos_weblinks";
            MySqlCommand readCmd = new MySqlCommand(sql, mySqlConn);
            MySqlDataReader reader = readCmd.ExecuteReader();

            while (reader.Read())
            {
                Console.WriteLine("Importing link: {0}", reader["id"].ToString());

                string insertSql = "INSERT INTO WebLinks (Title, Url, Description, CategoryId) VALUES " +
                    "(@Title, @Url, @Description, @CategoryId);";
                SqlCommand insertCmd = new SqlCommand(insertSql, aspNetDbConn);

                insertCmd.Parameters.AddWithValue("@Title", reader["title"].ToString());
                insertCmd.Parameters.AddWithValue("@Url", reader["url"].ToString());
                if (!webLinkCategoryMap.ContainsKey(reader.GetInt32(4))) continue;
                insertCmd.Parameters.AddWithValue("@CategoryId", webLinkCategoryMap[reader.GetInt32(4)]);
                insertCmd.Parameters.AddWithValue("@Description", reader["description"].ToString());

                insertCmd.ExecuteNonQuery();
            }
        }

        static void ImportLaRedaccion()
        {
            StreamWriter sw = new StreamWriter("updateLaRedaccion.sql", false);
            string userId = "415ed098-84c5-468e-bdfd-e9faf951e0b5";

            string queryRedaccionArticles = "SELECT id, title FROM jos_content WHERE created_by = 82";
            MySqlCommand readCmd = new MySqlCommand(queryRedaccionArticles, mySqlConn);
            MySqlDataReader reader = readCmd.ExecuteReader();

            while (reader.Read())
            {
                string title = EscapeQuotes(FixString(reader["title"].ToString()));
                
                string updateCmd =
                    string.Format("UPDATE Article SET CreatedBy = N'{0}' WHERE Title=N'{1}'", userId, title);
                sw.WriteLine(updateCmd);
                sw.WriteLine("GO");
            }
            sw.Close();
        }

        static string EscapeQuotes(string str)
        {
            return str.Replace("'", "''");
        }
    }
}
