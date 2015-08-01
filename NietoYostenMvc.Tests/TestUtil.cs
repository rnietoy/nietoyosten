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
        public const string DefaultUserPassword = "Pa%%word";

        private static readonly DynamicModel usersModel = new Users();
        private static readonly AlbumsModel albumsModel;

        static TestUtil()
        {
            albumsModel = AlbumsModel.GetInstance();
        }

        public static void InitDatabase()
        {
            DynamicModel dm = new DynamicModel("NietoYostenDb", "Albums", "ID");
            dm.Execute("DELETE FROM Pictures");
            dm.Execute("DELETE FROM Albums");
            dm.Execute("DELETE FROM ApprovalRequests");
            dm.Execute("DELETE FROM Users");

            var users = new Users();
            dynamic testUser = users.Register("test@nietoyosten.com", TestUtil.DefaultUserPassword, TestUtil.DefaultUserPassword);
            TestUtil.TestUserId = (int)users.Scalar("SELECT ID FROM Users WHERE Email=@0", args:"test@nietoyosten.com");
        }

        public static dynamic DefaultUser { get { return usersModel.Single(23); } }

        public static int TestUserId { get; set; }

        public static string DefaultAlbumName { get { return "TestAlbum"; } }

        public static string FormatUnique(string formatString)
        {
            Random random = new Random();
            int i = random.Next(10000);

            return string.Format(formatString, i);
        }
    }
}
