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
        private RequestContext requestContext = new RequestContext(new MockHttpContext(), new RouteData());

        public AccountControllerBuilder()
        {
            this.formsAuth = MockRepository.GenerateMock<IFormsAuth>();
        }

        public AccountController Build()
        {
            var accountController = new AccountController(formsAuth, mailer);
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

        public AccountControllerBuilder WithJsonResult()
        {
            var mockHttpRequest = new MockHttpRequest();
            mockHttpRequest.Items = new Dictionary<string, string> {{"X-Requested-With", "XMLHttpRequest"}};

            this.requestContext = new RequestContext(new MockHttpContext(mockHttpRequest), new RouteData());
            return this;
        }
    }
}
