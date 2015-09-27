using System;
using System.Linq;
using Massive;
using NietoYostenMvc.Code;
using NietoYostenMvc.Models;
using Xunit;

namespace NietoYostenMvc.Tests
{
    public class PicturesModelTests
    {
        private const string testAlbumName = "TestAlbum";
        private readonly int testAlbumId;

        private readonly PicturesModel picturesModel;

        public PicturesModelTests()
        {
            this.picturesModel = PicturesModel.GetInstance();
            TestUtil.InitDatabase();

            // Create a test album
            AlbumsModel albumsModel = AlbumsModel.GetInstance();
            albumsModel.Add(testAlbumName, testAlbumName, TestUtil.TestUserId);
            dynamic album = albumsModel.GetByFolderName(testAlbumName);
            this.testAlbumId = (int)album.ID;
        }

        [Fact]
        public void Test_AddPicture()
        {
            dynamic addResult = picturesModel.Add(testAlbumName, "picture1.jpeg", TestUtil.TestUserId);
            dynamic picture = picturesModel.Get(addResult.ID);
            Assert.NotNull(picture.ID);
        }

        [Fact]
        public void Test_Add_Picture_With_Existing_FileName()
        {
            picturesModel.Add(testAlbumName, "picture1.jpeg", TestUtil.TestUserId);

            Exception ex = Assert.Throws<UserFriendlyException>(
                () => picturesModel.Add(testAlbumName, "picture1.jpeg", TestUtil.TestUserId));

            Assert.Equal("An image with this file name already exists in this album.", ex.Message);
        }

        [Fact]
        public void Test_Get_Page_With_Default_PageSize()
        {
            // Default page size is 20.
            PicturesModel model = PicturesModel.GetInstance();
            this.InsertTestPictures(50);

            PageResult pageResult = model.GetPage(testAlbumName, 2);
            Assert.Equal(3, pageResult.TotalPages);
            Assert.Equal(20, pageResult.Items.Count());
            dynamic[] items = pageResult.Items.ToArray();

            Assert.Equal("picture21.jpg", items[0].FileName);
            Assert.Equal("picture32.jpg", items[11].FileName);
        }

        [Fact]
        public void Test_Get_Page_With_Custom_PageSize()
        {
            PicturesModel model = PicturesModel.GetInstance(pageSize: 12);
            this.InsertTestPictures(50);

            PageResult pageResult = model.GetPage(testAlbumName, 2);
            Assert.Equal(5, pageResult.TotalPages);
            Assert.Equal(12, pageResult.Items.Count());
            dynamic[] items = pageResult.Items.ToArray();

            Assert.Equal("picture13.jpg", items[0].FileName);
            Assert.Equal("picture24.jpg", items[11].FileName);
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
            this.picturesModel.Add(testAlbumName, "testpicture1.jpg", TestUtil.TestUserId);
            this.picturesModel.Add(testAlbumName, "testpicture2.jpg", TestUtil.TestUserId);
            this.picturesModel.Add(testAlbumName, "testpicture3.jpg", TestUtil.TestUserId);

            // Test getting picture by filename
            dynamic picture = this.picturesModel.GetByFileName(this.testAlbumId, "testpicture2.jpg");
            int pictureId = picture.ID;
            
            Assert.Equal(pictureId, picture.ID);
            Assert.Equal("testpicture2.jpg", picture.FileName);
            Assert.Equal(testAlbumName, picture.FolderName);
            Assert.Equal(pictureId - 1, picture.PreviousID);
            Assert.Equal(pictureId + 1, picture.NextID);
            Assert.Equal(1, picture.AlbumPage);
            Assert.Equal(testAlbumName + "/testpicture2.jpg", picture.FullName);

            // Test getting picture by ID
            picture = this.picturesModel.Get(pictureId);
            Assert.Equal(pictureId, picture.ID);
            Assert.Equal("testpicture2.jpg", picture.FileName);
            Assert.Equal(testAlbumName, picture.FolderName);
            Assert.Equal(pictureId - 1, picture.PreviousID);
            Assert.Equal(pictureId + 1, picture.NextID);
            Assert.Equal(1, picture.AlbumPage);
            Assert.Equal(testAlbumName + "/testpicture2.jpg", picture.FullName);
        }

        [Fact]
        public void Test_Get_FirstPicture_In_Album()
        {
            this.InsertTestPictures(25);
            dynamic picture = this.picturesModel.GetByFileName(this.testAlbumId, "picture1.jpg");
            int pictureId = picture.ID;
            
            Assert.Equal("picture1.jpg", picture.FileName);
            Assert.Equal(testAlbumName, picture.FolderName);
            Assert.Null(picture.PreviousID);
            Assert.Equal(pictureId + 1, picture.NextID);
            Assert.Equal(1, picture.AlbumPage);
        }

        [Fact]
        public void Test_Get_LastPicture_In_Album()
        {
            this.InsertTestPictures(25);
            dynamic picture = this.picturesModel.GetByFileName(this.testAlbumId, "picture25.jpg");
            int pictureId = picture.ID;

            Assert.Equal(pictureId, picture.ID);
            Assert.Equal("picture25.jpg", picture.FileName);
            Assert.Equal(testAlbumName, picture.FolderName);
            Assert.Equal(pictureId - 1, picture.PreviousID);
            Assert.Null(picture.NextID);
            Assert.Equal(2, picture.AlbumPage);
        }

        [Fact]
        public void Test_Delete_Picture()
        {
            string filename = TestUtil.FormatUnique("picture{0:D3}");
            this.picturesModel.Add(testAlbumName, filename, TestUtil.TestUserId);

            dynamic picture = this.picturesModel.GetByFileName(this.testAlbumId, filename);
            this.picturesModel.Delete(picture.ID);

            Assert.Null(picturesModel.Get(picture.ID));
        }

        /// <summary>
        /// Insert several pages of test pictures so we can test pagination.
        /// </summary>
        private void InsertTestPictures(int count)
        {
            var dynamicModel = new DynamicModel("NietoYostenDb", "Pictures", "ID");
            DateTime startTime = DateTime.Now;

            for (int i = 1; i <= count; i++)
            {
                int albumId = (int)dynamicModel.Scalar(
                    "SELECT ID FROM Albums WHERE Title = @0", testAlbumName);

                dynamicModel.Insert(new
                {
                    AlbumID = albumId,
                    Title = $"picture{i:D}.jpg",
                    FileName = $"picture{i:D}.jpg",
                    UploadedBy = TestUtil.TestUserId,
                    UploadedAt = startTime.AddSeconds(i)
                });
            }
        }
    }
}
