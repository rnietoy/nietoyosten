using System;
using System.Text;
using System.Collections.Generic;
using System.Configuration;
using System.Web.Mvc;
using Facebook;
using NietoYostenMvc.Code;
using NietoYostenMvc.Code.FormsAuth;
using NietoYostenMvc.Controllers;
using NietoYostenMvc.Models;
using NietoYostenMvc.Tests.Mocks;
using Rhino.Mocks;
using Xunit;

namespace NietoYostenMvc.Tests.Specs.FacebookLogin
{
    /// <summary>
    /// Facebok API throws exception when trying to access the user's email address
    /// This could be due to an invalid/expired/tampered token, etc.
    /// </summary>
    public class FacebookApiThrowsWhenGettingEmail
    {
        private Users usersModel = new Users();
        private readonly IFormsAuth formsAuth;
        private readonly IFacebookApi facebookApi;
        private readonly IMailer mailer;
        private AccountController accountController;
        private readonly JsonResult result;
        private const string UserEmail = "unknown@nietoyosten.com";

        public FacebookApiThrowsWhenGettingEmail()
        {
            // Init database (this will delete all users, so there are no users with facebook ids in there).
            TestUtil.InitDatabase();

            this.usersModel = new Users();
            this.usersModel.Register("fbuser@nietoyosten.com", TestUtil.DefaultUserPassword, TestUtil.DefaultUserPassword);
            dynamic user = this.usersModel.Single(where: "Email = @0", args: "fbuser@nietoyosten.com");
            user.IsApproved = true;
            this.usersModel.Update(user, user.ID);

            this.facebookApi = MockRepository.GenerateStub<IFacebookApi>();
            this.facebookApi.Stub(x => x.GetUserEmail(Arg<string>.Is.Anything)).Return(null);

            this.formsAuth = MockRepository.GenerateMock<IFormsAuth>();

            this.accountController =
                new AccountControllerBuilder().WithJsonRequest().WithFormsAuth(this.formsAuth).WithMailer(this.mailer).WithFacebookApi(this.facebookApi).Build();

            this.result = this.accountController.FbLogin(
                ConfigurationManager.AppSettings["SignedRequest"],
                ConfigurationManager.AppSettings["AccessToken"],
                "/Login") as JsonResult;
        }

        [Fact]
        public void UserIsNotMerged()
        {
            dynamic user = this.usersModel.Single(where: "Email = @0", args: "fbuser@nietoyosten.com");
            Assert.Null(user.FacebookUserID);
        }

        [Fact]
        public void UserIsNotLoggedIn()
        {
            this.formsAuth.AssertWasNotCalled(x => x.SetAuthCookie("fbuser@nietoyosten.com", false));
        }
    }
}
