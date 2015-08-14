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
    [Trait("A user that already has a facebook user id (and is approved) logs in with Facebook button.", "")]
    public class UserWithFacebookIdLogsIn
    {
        private readonly IFormsAuth formsAuth;
        private readonly IFacebookApi facebookApi;
        private AccountController accountController;
        private readonly JsonResult result;

        public UserWithFacebookIdLogsIn()
        {
            TestUtil.InitDatabase();
            this.AddUserWithFacebookId();

            this.formsAuth = MockRepository.GenerateMock<IFormsAuth>();
            this.formsAuth.Expect(x => x.SetAuthCookie("fbuser@nietoyosten.com", false));

            this.facebookApi = MockRepository.GenerateStub<IFacebookApi>();
            this.facebookApi.Stub(x => x.GetUserEmail(Arg<string>.Is.Anything)).Return("fbuser@nietoyosten.com");

            this.accountController =
                new AccountControllerBuilder().WithJsonResult().WithFormsAuth(this.formsAuth).Build();

            this.result = accountController.FbLogin(
                ConfigurationManager.AppSettings["SignedRequest"],
                ConfigurationManager.AppSettings["AccessToken"],
                "/Login") as JsonResult;
        }

        [Fact(DisplayName = "User is logged in successfully")]
        public void UserIsLoggedInSuccessfully()
        {
            Assert.NotNull(this.result);

            NyResult data = this.result.Data as NyResult;
            Assert.NotNull(data);
            Assert.Equal(true, data.Success);
            Assert.Equal("/Login", data.RedirectUrl);
            this.formsAuth.VerifyAllExpectations();
        }

        private void AddUserWithFacebookId()
        {
            var usersModel = new Users();
            usersModel.Register("fbuser@nietoyosten.com", TestUtil.DefaultUserPassword, TestUtil.DefaultUserPassword);
            usersModel.SetFacebookUserId("fbuser@nietoyosten.com", 1446565532337099);

            dynamic user = usersModel.Single(where: "Email = @0", args: "fbuser@nietoyosten.com");
            user.IsApproved = true;
            usersModel.Update(user, user.ID);
        }
    }
}
