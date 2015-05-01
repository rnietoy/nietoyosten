using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Massive;
using NietoYostenMvc.Code;
using NietoYostenMvc.Models;
using Xunit;
using Xunit.Abstractions;

namespace NietoYostenMvc.Tests
{
    public class AlbumsModelTests
    {
        private readonly ITestOutputHelper output;
        private readonly AlbumsModel albumsModel;

        public AlbumsModelTests(ITestOutputHelper output)
        {
            this.output = output;
            this.albumsModel = AlbumsModel.GetInstance();

            TestUtil.InitDatabase();
        }

        [Fact]
        public void Test_Add_Album()
        {
            string albumName = TestUtil.FormatUnique("europe_vacations{0}");
            this.albumsModel.Add("Test Europe vacations", albumName, TestUtil.DefaultUserId);
        }

        [Fact]
        public void Test_Add_Duplicate_FolderName()
        {
            string albumName = TestUtil.FormatUnique("europe_vacations{0}");
            this.albumsModel.Add("Test Europe vacations", albumName, TestUtil.DefaultUserId);

            Exception ex = Assert.Throws<UserFriendlyException>(
                () => this.albumsModel.Add("Europe vacations", albumName, TestUtil.DefaultUserId));

            Assert.Equal("An album with this folder name already exists.", ex.Message);
        }
    }
}
