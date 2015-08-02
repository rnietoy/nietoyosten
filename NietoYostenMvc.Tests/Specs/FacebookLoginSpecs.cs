using System;
using System.Configuration;
using System.Net.Mail;
using System.Web.Mvc;
using NietoYostenMvc.Code;
using NietoYostenMvc.Code.FormsAuth;
using NietoYostenMvc.Controllers;
using NietoYostenMvc.Models;
using NietoYostenMvc.Tests.Mocks;
using Rhino.Mocks;
using Xunit;
using Xunit.Abstractions;

namespace NietoYostenMvc.Tests
{
    /// <summary>
    /// A user that already has a facebook user id logs in with Facebook button.
    /// </summary>
    public class UserWithFacebookIdLogsIn
    {
        private readonly ITestOutputHelper output;
        private readonly IFormsAuth formsAuth;
        private AccountController accountController;
        private readonly JsonResult result;

        public UserWithFacebookIdLogsIn(ITestOutputHelper output)
        {
            this.output = output;
            TestUtil.InitDatabase();
            this.AddUserWithFacebookId();

            this.formsAuth = MockRepository.GenerateMock<IFormsAuth>();
            this.formsAuth.Expect(x => x.SetAuthCookie("fbuser@nietoyosten.com", false));

            this.accountController =
                new AccountControllerBuilder().WithJsonResult().WithFormsAuth(this.formsAuth).Build();

            FacebookUtil.TestUserEmail = "fbuser@nietoyosten.com";

            this.result = accountController.FbLogin(
                ConfigurationManager.AppSettings["SignedRequest"],
                ConfigurationManager.AppSettings["AccessToken"],
                "/Login") as JsonResult;
        }

        [Fact]
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

            dynamic user = usersModel.Single(where:"Email = @0", args:"fbuser@nietoyosten.com");
            user.IsApproved = true;
            usersModel.Update(user, user.ID);
        }
    }

    public class UserWithNoFacebookIdLogsIn
    {
        private Users usersModel = new Users();
        private readonly IFormsAuth formsAuth;
        private AccountController accountController;
        private readonly JsonResult result;

        public UserWithNoFacebookIdLogsIn()
        {
            TestUtil.InitDatabase();

            this.usersModel = new Users();
            this.usersModel.Register("fbuser@nietoyosten.com", TestUtil.DefaultUserPassword, TestUtil.DefaultUserPassword);
            dynamic user = this.usersModel.Single(where:"Email = @0", args:"fbuser@nietoyosten.com");
            user.IsApproved = true;
            usersModel.Update(user, user.ID);

            this.formsAuth = MockRepository.GenerateMock<IFormsAuth>();
            this.formsAuth.Expect(x => x.SetAuthCookie("fbuser@nietoyosten.com", false));

            this.accountController =
                new AccountControllerBuilder().WithJsonResult().WithFormsAuth(this.formsAuth).Build();

            FacebookUtil.TestUserEmail = "fbuser@nietoyosten.com";

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

    public class NonExistentUserLogsIn
    {
        private Users usersModel = new Users();
        private readonly IFormsAuth formsAuth;
        private readonly IMailer mailer;
        private AccountController accountController;
        private readonly JsonResult result;
        private const string UserEmail = "unknown@nietoyosten.com";

        public NonExistentUserLogsIn()
        {
            TestUtil.InitDatabase();

            this.formsAuth = MockRepository.GenerateMock<IFormsAuth>();
            this.mailer = MockRepository.GenerateStub<IMailer>();

            this.accountController =
                new AccountControllerBuilder().WithJsonResult().WithFormsAuth(this.formsAuth).WithMailer(this.mailer).Build();

            FacebookUtil.TestUserEmail = UserEmail;

            this.result = accountController.FbLogin(
                ConfigurationManager.AppSettings["SignedRequest"],
                ConfigurationManager.AppSettings["AccessToken"],
                "/Login") as JsonResult;
        }

        [Fact]
        public void RequestIsSuccessful()
        {
            Assert.NotNull(this.result);
            NyResult data = this.result.Data as NyResult;
            Assert.NotNull(data);
            Assert.Equal(true, data.Success);
        }

        [Fact]
        public void UserIsNotLoggedIn()
        {
            this.formsAuth.AssertWasNotCalled(x => x.SetAuthCookie(Arg<string>.Is.Anything, Arg<bool>.Is.Anything));
        }

        [Fact]
        public void NewUserIsCreatedButNotApproved()
        {
            var user = this.usersModel.Single(where: "Email = @0", args: UserEmail);
            Assert.NotNull(user);
            Assert.False(user.IsApproved);
        }

        [Fact]
        public void NotificationEmailIsSent()
        {
            this.mailer.AssertWasCalled(x => x.SendMail(Arg<MailMessage>.Is.Anything));
            this.mailer.AssertWasCalled(x => x.SendMail(Arg<MailMessage>.Matches(m => m.From.Address.ToString() == "noreply@nietoyosten.com")));
            this.mailer.AssertWasCalled(x => x.SendMail(Arg<MailMessage>.Matches(m => m.Subject.Contains("User approval request"))));
        }
    }
}
