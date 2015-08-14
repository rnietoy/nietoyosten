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

namespace NietoYostenMvc.Tests.Specs.FacebookLogin
{
    public class NonExistentUserLogsIn
    {
        private Users usersModel = new Users();
        private readonly IFormsAuth formsAuth;
        private readonly IFacebookApi facebookApi;
        private readonly IMailer mailer;
        private AccountController accountController;
        private readonly JsonResult result;
        private const string UserEmail = "unknown@nietoyosten.com";

        public NonExistentUserLogsIn()
        {
            TestUtil.InitDatabase();

            this.formsAuth = MockRepository.GenerateMock<IFormsAuth>();
            this.mailer = MockRepository.GenerateStub<IMailer>();

            this.facebookApi = MockRepository.GenerateStub<IFacebookApi>();
            this.facebookApi.Stub(x => x.GetUserEmail(Arg<string>.Is.Anything)).Return(UserEmail);

            this.accountController =
                new AccountControllerBuilder().WithJsonResult().WithFormsAuth(this.formsAuth).WithMailer(this.mailer).WithFacebookApi(this.facebookApi).Build();

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
        public void UserIsNotAuthenticated()
        {
            this.formsAuth.AssertWasNotCalled(x => x.SetAuthCookie(Arg<string>.Is.Anything, Arg<bool>.Is.Anything));
        }

        [Fact]
        public void NewUserIsCreatedButNotApproved()
        {
            var user = this.usersModel.Single(@where: "Email = @0", args: UserEmail);
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
