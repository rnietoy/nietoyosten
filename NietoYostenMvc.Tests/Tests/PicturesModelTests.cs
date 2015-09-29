using System;
using System.Linq;
using Massive;
using NietoYostenMvc.Code;
using NietoYostenMvc.Models;
using Xunit;

// ReSharper disable once CheckNamespace
namespace NietoYostenMvc.Tests
{
    public class PicturesModelTests
    {
        private const string TestAlbumName = "TestAlbum";
        private readonly int testAlbumId;

        private readonly PicturesModel picturesModel;
        private readonly DynamicModel dynamicModel = new DynamicModel("NietoYostenDb", "Pictures", "ID");

        public PicturesModelTests()
        {
            this.picturesModel = PicturesModel.GetInstance();
            TestUtil.InitDatabase();

            // Create a test album
            AlbumsModel albumsModel = AlbumsModel.GetInstance();
            albumsModel.Add(TestAlbumName, TestAlbumName, TestUtil.TestUserId);
            dynamic album = albumsModel.GetByFolderName(TestAlbumName);
            this.testAlbumId = (int)album.ID;
        }

        [Fact]
        public void Test_AddPicture()
        {
            dynamic addResult = this.picturesModel.Add(TestAlbumName, "picture1.jpeg", TestUtil.TestUserId);
            dynamic picture = this.picturesModel.Get(addResult.ID);
            Assert.NotNull(picture.ID);
        }

        [Fact]
        public void Test_Add_Picture_With_Existing_FileName()
        {
            this.picturesModel.Add(TestAlbumName, "picture1.jpeg", TestUtil.TestUserId);

            Exception ex = Assert.Throws<UserFriendlyException>(
                () => this.picturesModel.Add(TestAlbumName, "picture1.jpeg", TestUtil.TestUserId));

            Assert.Equal("An image with this file name already exists in this album.", ex.Message);
        }

        [Fact]
        public void Test_Get_Page_With_Default_PageSize()
        {
            // Default page size is 20.
            PicturesModel model = PicturesModel.GetInstance();
            this.InsertTestPictures(50);

            PageResult pageResult = model.GetPage(TestAlbumName, 2);
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

            PageResult pageResult = model.GetPage(TestAlbumName, 2);
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
            DateTime dateTime = DateTime.Now;

            this.InsertTestPicture("picture5.jpg", dateTime);
            this.InsertTestPicture("picture4.jpg", dateTime);
            this.InsertTestPicture("picture3.jpg", dateTime.AddSeconds(-1));
            this.InsertTestPicture("picture2.jpg", dateTime.AddSeconds(-1));
            this.InsertTestPicture("picture1.jpg", dateTime.AddSeconds(-1));

            // Test getting the middle picture (by sort order) by filename (this would be picture3.jpg)
            dynamic picture = this.picturesModel.GetByFileName(this.testAlbumId, "picture3.jpg");
            dynamic prevPicture = this.picturesModel.GetByFileName(this.testAlbumId, "picture2.jpg");
            dynamic nextPicture = this.picturesModel.GetByFileName(this.testAlbumId, "picture4.jpg");
            
            Assert.Equal("picture3.jpg", picture.FileName);
            Assert.Equal(TestAlbumName, picture.FolderName);
            Assert.Equal(prevPicture.ID, picture.PreviousID);
            Assert.Equal(nextPicture.ID, picture.NextID);
            Assert.Equal(1, picture.AlbumPage);
            Assert.Equal(TestAlbumName + "/picture3.jpg", picture.FullName);

            // Test getting picture by ID
            int pictureId = picture.ID;
            picture = this.picturesModel.Get(pictureId);
            Assert.Equal(pictureId, picture.ID);
            Assert.Equal("picture3.jpg", picture.FileName);
            Assert.Equal(TestAlbumName, picture.FolderName);
            Assert.Equal(prevPicture.ID, picture.PreviousID);
            Assert.Equal(nextPicture.ID, picture.NextID);
            Assert.Equal(1, picture.AlbumPage);
            Assert.Equal(TestAlbumName + "/picture3.jpg", picture.FullName);
        }

        [Fact]
        public void Test_Get_FirstPicture_In_Album()
        {
            this.InsertTestPictures(25);
            dynamic picture = this.picturesModel.GetByFileName(this.testAlbumId, "picture1.jpg");
            dynamic picture2 = this.picturesModel.GetByFileName(this.testAlbumId, "picture2.jpg");

            Assert.Equal("picture1.jpg", picture.FileName);
            Assert.Equal(TestAlbumName, picture.FolderName);
            Assert.Null(picture.PreviousID);
            Assert.Equal(picture2.ID, picture.NextID);
            Assert.Equal(1, picture.AlbumPage);
        }

        [Fact]
        public void Test_Get_LastPicture_In_Album()
        {
            this.InsertTestPictures(25);
            dynamic picture = this.picturesModel.GetByFileName(this.testAlbumId, "picture25.jpg");
            int pictureId = picture.ID;
            dynamic picture24 = this.picturesModel.GetByFileName(this.testAlbumId, "picture24.jpg");

            Assert.Equal(pictureId, picture.ID);
            Assert.Equal("picture25.jpg", picture.FileName);
            Assert.Equal(TestAlbumName, picture.FolderName);
            Assert.Equal(picture24.ID, picture.PreviousID);
            Assert.Null(picture.NextID);
            Assert.Equal(2, picture.AlbumPage);
        }

        [Fact]
        public void Test_Delete_Picture()
        {
            string filename = TestUtil.FormatUnique("picture{0:D3}");
            this.picturesModel.Add(TestAlbumName, filename, TestUtil.TestUserId);

            dynamic picture = this.picturesModel.GetByFileName(this.testAlbumId, filename);
            this.picturesModel.Delete(picture.ID);

            Assert.Null(this.picturesModel.Get(picture.ID));
        }

        /// <summary>
        /// Insert several pages of test pictures so we can test pagination.
        /// </summary>
        private void InsertTestPictures(int count)
        {
            DateTime startTime = DateTime.Now;

            for (int i = 1; i <= count; i++)
            {
                this.dynamicModel.Insert(new
                {
                    AlbumID = this.testAlbumId,
                    Title = $"picture{i:D}.jpg",
                    FileName = $"picture{i:D}.jpg",
                    UploadedBy = TestUtil.TestUserId,
                    UploadedAt = startTime.AddSeconds(i)
                });
            }
        }

        private void InsertTestPicture(string title, DateTime uploadedAt = default(DateTime))
        {
            if (uploadedAt == default(DateTime))
            {
                uploadedAt = DateTime.Now;
            }

            this.dynamicModel.Insert(new
            {
                AlbumID = this.testAlbumId,
                Title = title,
                FileName = title,
                UploadedBy = TestUtil.TestUserId,
                UploadedAt = uploadedAt
            });
        }
    }
}
