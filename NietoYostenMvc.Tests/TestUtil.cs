using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using Massive;
using NietoYostenMvc.Models;

namespace NietoYostenMvc.Tests
{
    public class TestUtil
    {
        private static readonly DynamicModel usersModel = new Users();
        private static readonly AlbumsModel albumsModel;

        static TestUtil()
        {
            albumsModel = AlbumsModel.GetInstance();
        }

        public static void InitDatabase()
        {
            DynamicModel dm = new DynamicModel("NietoYostenDb", "Albums", "ID");

            // Delete all pictures in test albums
            dm.Execute("DELETE FROM Pictures WHERE AlbumID IN (SELECT ID FROM Albums WHERE Title LIKE 'Test%')");

            // Delete all test albums
            dm.Execute("DELETE FROM Albums WHERE Title LIKE 'Test%'");
        }

        public static dynamic DefaultUser { get { return usersModel.Single(23); } }

        public static int DefaultUserId { get { return 23; } }

        public static string DefaultAlbumName { get { return "TestAlbum"; } }

        public static string FormatUnique(string formatString)
        {
            Random random = new Random();
            int i = random.Next(10000);

            return string.Format(formatString, i);
        }
    }
}
