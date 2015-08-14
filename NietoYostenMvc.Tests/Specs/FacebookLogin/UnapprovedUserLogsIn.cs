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
    [Trait("An unapproved user with a facebook user id tried to login with facebook.", "")]
    public class UnapprovedUserLogsIn
    {
        private readonly IFormsAuth formsAuth;
        private readonly IFacebookApi facebookApi;
        private AccountController accountController;
        private readonly JsonResult result;

        public UnapprovedUserLogsIn()
        {
            TestUtil.InitDatabase();
            this.AddUnApprovedUserWithFacebookId();
            this.formsAuth = MockRepository.GenerateMock<IFormsAuth>();

            this.facebookApi = MockRepository.GenerateStub<IFacebookApi>();
            this.facebookApi.Stub(x => x.GetUserEmail(Arg<string>.Is.Anything)).Return("unapproved@nietoyosten.com");

            this.accountController =
                new AccountControllerBuilder().WithJsonResult().WithFormsAuth(this.formsAuth).Build();

            this.result = accountController.FbLogin(
                ConfigurationManager.AppSettings["SignedRequest"],
                ConfigurationManager.AppSettings["AccessToken"],
                "/Login") as JsonResult;
        }

        [Fact(DisplayName = "User is not authenticated.")]
        public void UserIsNotAuthenticated()
        {
            this.formsAuth.AssertWasNotCalled(x => x.SetAuthCookie(Arg<string>.Is.Anything, Arg<bool>.Is.Anything));
        }

        [Fact(DisplayName = "Message is returned to user saying account is not approved.")]
        public void MessageIsReturned()
        {
            NyResult nyResult = this.result.Data as NyResult;
            Assert.Contains("This user account has not been approved yet", nyResult.Message);
        }

        private void AddUnApprovedUserWithFacebookId()
        {
            var usersModel = new Users();
            usersModel.Register("unapproved@nietoyosten.com", TestUtil.DefaultUserPassword, TestUtil.DefaultUserPassword);
            usersModel.SetFacebookUserId("unapproved@nietoyosten.com", 1446565532337099);
        }
    }
}
