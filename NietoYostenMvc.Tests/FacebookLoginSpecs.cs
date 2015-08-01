using System.Configuration;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using NietoYostenMvc.Code;
using NietoYostenMvc.Code.FormsAuth;
using NietoYostenMvc.Controllers;
using NietoYostenMvc.Models;
using Rhino.Mocks;
using Xunit;
using Xunit.Abstractions;

namespace NietoYostenMvc.Tests
{
    class MockHttpRequest : HttpRequestBase
    {
        public override string this[string key]
        {
            get
            {
                if (key == "X-Requested-With")
                {
                    return "XMLHttpRequest";
                }
                return "";
            }
        }
    }

    class MockHttpContext : HttpContextBase
    {
        private readonly IPrincipal _user = new GenericPrincipal(
                 new GenericIdentity("someUser"), null /* roles */);

        private readonly HttpRequestBase request = new MockHttpRequest();

        public override IPrincipal User
        {
            get
            {
                return _user;
            }
            set
            {
                base.User = value;
            }
        }

        public override HttpRequestBase Request
        {
            get { return this.request; }
        }
    }

    /// <summary>
    /// A user that is already linked with a facebook user id logs in with Facebook.
    /// </summary>
    public class LinkedUserLogsIn
    {
        private readonly ITestOutputHelper output;
        private AccountController accountController;
        private readonly JsonResult result;

        public LinkedUserLogsIn(ITestOutputHelper output)
        {
            this.output = output;
            TestUtil.InitDatabase();
            this.AddUserWithFacebookId();

            IFormsAuth formsAuth = MockRepository.GenerateMock<IFormsAuth>();

            this.accountController = new AccountController(formsAuth);
            this.accountController.ControllerContext = new ControllerContext
            {
                Controller = this.accountController,
                RequestContext = new RequestContext(new MockHttpContext(), new RouteData())
            };

            FacebookUtil.GetUserEmailTestValue = "fbuser@nietoyosten.com";

            this.result = accountController.FbLogin(
                ConfigurationManager.AppSettings["SignedRequest"],
                ConfigurationManager.AppSettings["AccessToken"],
                "/Login") as JsonResult;
        }

        [Fact]
        public void UserIsLoggedIn()
        {
            Assert.NotNull(this.result);

            NyResult data = this.result.Data as NyResult;
            Assert.NotNull(data);
            Assert.Equal(true, data.Success);
            Assert.Equal("/Login", data.RedirectUrl);
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
}
