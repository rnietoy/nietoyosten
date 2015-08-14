using System.Configuration;
using System.Web.Mvc;
using NietoYostenMvc.Code;
using NietoYostenMvc.Code.FormsAuth;
using NietoYostenMvc.Controllers;
using NietoYostenMvc.Models;
using NietoYostenMvc.Tests.Mocks;
using Rhino.Mocks;
using Xunit;

namespace NietoYostenMvc.Tests.Specs.FacebookLogin
{
    public class UserWithNoFacebookIdLogsIn
    {
        private Users usersModel = new Users();
        private readonly IFormsAuth formsAuth;
        private readonly IFacebookApi facebookApi;
        private AccountController accountController;
        private readonly JsonResult result;

        public UserWithNoFacebookIdLogsIn()
        {
            // Init database (this will delete all users, so there are no users with facebook ids in there).
            TestUtil.InitDatabase();

            this.usersModel = new Users();
            this.usersModel.Register("fbuser@nietoyosten.com", TestUtil.DefaultUserPassword, TestUtil.DefaultUserPassword);
            dynamic user = this.usersModel.Single(where: "Email = @0", args: "fbuser@nietoyosten.com");
            user.IsApproved = true;
            usersModel.Update(user, user.ID);

            this.facebookApi = MockRepository.GenerateStub<IFacebookApi>();
            this.facebookApi.Stub(x => x.GetUserEmail(Arg<string>.Is.Anything)).Return("fbuser@nietoyosten.com");

            this.formsAuth = MockRepository.GenerateMock<IFormsAuth>();
            this.formsAuth.Expect(x => x.SetAuthCookie("fbuser@nietoyosten.com", false));

            this.accountController =
                new AccountControllerBuilder().WithJsonResult().WithFormsAuth(this.formsAuth).WithFacebookApi(this.facebookApi).Build();

            this.result = accountController.FbLogin(
                ConfigurationManager.AppSettings["SignedRequest"],
                ConfigurationManager.AppSettings["AccessToken"],
                "/Login") as JsonResult;
        }

        [Fact]
        public void UserIsMerged()
        {
            dynamic user = this.usersModel.Single(where: "Email = @0", args: "fbuser@nietoyosten.com");
            Assert.Equal(1446565532337099, user.FacebookUserID);
        }

        [Fact]
        public void UserIsLoggedIn()
        {
            Assert.NotNull(this.result);

            NyResult data = this.result.Data as NyResult;
            Assert.NotNull(data);
            Assert.Equal(true, data.Success);
            this.formsAuth.VerifyAllExpectations();
        }

        [Fact]
        public void UserIsRedirectedToHomepage()
        {
            NyResult data = this.result.Data as NyResult;
            Assert.Equal("/", data.RedirectUrl);
        }
    }
}
