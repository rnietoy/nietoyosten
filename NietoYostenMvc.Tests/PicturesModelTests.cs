using System;
using System.Linq;
using NietoYostenMvc.Code;
using NietoYostenMvc.Models;
using Xunit;
using Xunit.Abstractions;

namespace NietoYostenMvc.Tests
{
    public class PicturesModelTests
    {
        private readonly string defaultAlbumName = "TestAlbum";
        private int defaultAlbumId;
        private readonly int defaultUserId = 23;

        private readonly ITestOutputHelper output;
        private PicturesModel picturesModel;

        public PicturesModelTests(ITestOutputHelper output)
        {
            this.output = output;
            this.picturesModel = PicturesModel.GetInstance();

            TestUtil.InitDatabase();

            // Create a test album
            AlbumsModel albumsModel = AlbumsModel.GetInstance();
            albumsModel.Add(this.defaultAlbumName, this.defaultAlbumName, this.defaultUserId);
            dynamic album = albumsModel.GetByFolderName(defaultAlbumName);
            this.defaultAlbumId = (int)album.ID;
        }

        [Fact]
        public void Test_AddPicture()
        {
            dynamic result = picturesModel.Add(this.defaultAlbumName, "picture1.jpeg", this.defaultUserId);
            output.WriteLine("Picture ID: " + result.ID);
            Assert.NotNull(result.ID);
        }

        [Fact]
        public void Test_Add_Picture_With_Existing_FileName()
        {
            picturesModel.Add(this.defaultAlbumName, "picture1.jpeg", this.defaultUserId);

            Exception ex = Assert.Throws<UserFriendlyException>(
                () => picturesModel.Add(this.defaultAlbumName, "picture1.jpeg", this.defaultUserId));

            Assert.Equal("An image with this file name already exists in this album.", ex.Message);
        }

        [Fact]
        public void Test_Get_Page_With_Default_PageSize()
        {
            PicturesModel model = PicturesModel.GetInstance();

            PageResult pageResult = model.GetPage("randc_before", 2);
            Assert.Equal(26, pageResult.TotalPages);
            Assert.Equal(20, pageResult.Items.Count());
            dynamic[] items = pageResult.Items.ToArray();

            Assert.Equal("A171.jpg", items[0].FileName);
            Assert.Equal("A188.jpg", items[11].FileName);
        }

        [Fact]
        public void Test_Get_Page_With_Custom_PageSize()
        {
            PicturesModel model = PicturesModel.GetInstance(pageSize: 12);

            PageResult pageResult = model.GetPage("randc_before", 2);
            Assert.Equal(44, pageResult.TotalPages);
            Assert.Equal(12, pageResult.Items.Count());
            dynamic[] items = pageResult.Items.ToArray();

            Assert.Equal("A156.jpg", items[0].FileName);
            Assert.Equal("A175.jpg", items[11].FileName);
        }

        [Fact]
        public void Test_Get_NonExistent_Picture()
        {
            dynamic picture = this.picturesModel.Get(11);
            Assert.Null(picture);
        }

        [Fact]
        public void Test_Get_Picture()
        {
            int pictureId = 249;
            dynamic picture = this.picturesModel.Get(pictureId);
            Assert.Equal(pictureId, picture.ID);
            Assert.Equal("B218.jpg", picture.FileName);
            Assert.Equal("randc_before", picture.FolderName);
            Assert.Equal(pictureId - 1, picture.PreviousID);
            Assert.Equal(pictureId + 1, picture.NextID);
            Assert.Equal(12, picture.AlbumPage);
            Assert.Equal("randc_before/B218.jpg", picture.FullName);
        }

        [Fact]
        public void Test_Get_FirstPicture_In_Album()
        {
            const int pictureId = 16;
            dynamic picture = this.picturesModel.Get(pictureId);
            Assert.Equal(pictureId, picture.ID);
            Assert.Equal("A139.jpg", picture.FileName);
            Assert.Equal("randc_before", picture.FolderName);
            Assert.Null(picture.PreviousID);
            Assert.Equal(pictureId + 1, picture.NextID);
            Assert.Equal(1, picture.AlbumPage);
        }

        [Fact]
        public void Test_Get_LastPicture_In_Album()
        {
            const int pictureId = 532;
            dynamic picture = this.picturesModel.Get(pictureId);
            Assert.Equal(pictureId, picture.ID);
            Assert.Equal("G079.jpg", picture.FileName);
            Assert.Equal("randc_before", picture.FolderName);
            Assert.Equal(pictureId - 1, picture.PreviousID);
            Assert.Null(picture.NextID);
            Assert.Equal(26, picture.AlbumPage);
        }

        [Fact]
        public void Test_Delete_Picture()
        {
            string filename = TestUtil.FormatUnique("picture{0:D3}");
            picturesModel.Add(this.defaultAlbumName, filename, this.defaultUserId);

            dynamic picture = picturesModel.GetByFileName(this.defaultAlbumId, filename);
            picturesModel.Delete(picture.ID);

            Assert.Null(picturesModel.Get(picture.ID));
        }

        [Fact]
        public void Test_Output()
        {
            this.output.WriteLine("hello");
        }
    }
}
