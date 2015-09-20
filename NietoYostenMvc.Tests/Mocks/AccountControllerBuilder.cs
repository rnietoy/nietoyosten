using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Routing;
using NietoYostenMvc.Code;
using NietoYostenMvc.Code.FormsAuth;
using NietoYostenMvc.Controllers;
using Rhino.Mocks;

namespace NietoYostenMvc.Tests.Mocks
{
    class MockFactory
    {
        
    }

    public class AccountControllerBuilder
    {
        private IFormsAuth formsAuth;
        private IMailer mailer = MockRepository.GenerateMock<IMailer>();
        private IFacebookApi facebookApi;
        private RequestContext requestContext = new RequestContext(new MockHttpContext(), new RouteData());

        public AccountControllerBuilder()
        {
            this.formsAuth = MockRepository.GenerateMock<IFormsAuth>();

            this.facebookApi = MockRepository.GenerateStub<IFacebookApi>();
            this.facebookApi.Stub(x => x.GetUserEmail(Arg<string>.Is.Anything)).Return("fbuser@nietoyosten.com");
        }

        public AccountController Build()
        {
            var accountController = new AccountController(this.formsAuth, this.mailer, this.facebookApi);
            accountController.ControllerContext = new ControllerContext
            {
                Controller = accountController,
                RequestContext = this.requestContext
            };

            return accountController;
        }

        public AccountControllerBuilder WithFormsAuth(IFormsAuth formsAuth)
        {
            this.formsAuth = formsAuth;
            return this;
        }

        public AccountControllerBuilder WithMailer(IMailer mailer)
        {
            this.mailer = mailer;
            return this;
        }

        public AccountControllerBuilder WithFacebookApi(IFacebookApi facebookApi)
        {
            this.facebookApi = facebookApi;
            return this;
        }

        public AccountControllerBuilder WithJsonRequest()
        {
            var mockHttpRequest = new MockHttpRequest();
            mockHttpRequest.Items["X-Requested-With"] = "XMLHttpRequest";
            this.requestContext = new RequestContext(new MockHttpContext(mockHttpRequest), new RouteData());
            return this;
        }
    }
}
