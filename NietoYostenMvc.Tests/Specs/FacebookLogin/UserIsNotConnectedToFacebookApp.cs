using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    public class UserIsNotConnectedToFacebookApp
    {
        private readonly IFormsAuth formsAuth;
        private readonly IFacebookApi facebookApi;
        private AccountController accountController;
        private readonly JsonResult result;

        public UserIsNotConnectedToFacebookApp()
        {
            TestUtil.InitDatabase();
            this.AddUserWithFacebookId();

            this.formsAuth = MockRepository.GenerateMock<IFormsAuth>();
            this.formsAuth.Expect(x => x.SetAuthCookie("fbuser@nietoyosten.com", false));

            this.facebookApi = MockRepository.GenerateStub<IFacebookApi>();
            this.facebookApi.Stub(x => x.GetUserEmail(Arg<string>.Is.Anything)).Return(null);

            this.accountController =
                new AccountControllerBuilder().WithJsonRequest().WithFormsAuth(this.formsAuth).Build();

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
            usersModel.SetFacebookUserId("fbuser@nietoyosten.com", long.Parse(ConfigurationManager.AppSettings["FacebookTestUserId"]));

            dynamic user = usersModel.Single(where: "Email = @0", args: "fbuser@nietoyosten.com");
            user.IsApproved = true;
            usersModel.Update(user, user.ID);
        }
    }
}
