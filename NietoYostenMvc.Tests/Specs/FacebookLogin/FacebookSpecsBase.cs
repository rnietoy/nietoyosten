
using System.Web.Mvc;
using NietoYostenMvc.Code;
using NietoYostenMvc.Code.FormsAuth;
using NietoYostenMvc.Controllers;
using NietoYostenMvc.Models;
using NietoYostenMvc.Tests.Mocks;
using Rhino.Mocks;

namespace NietoYostenMvc.Tests.Specs.FacebookLogin
{
    public class FacebookSpecsBase
    {
        protected readonly Users usersModel = new Users();
        protected readonly IFormsAuth formsAuth;
        protected readonly IFacebookApi facebookApi;
        protected readonly AccountController accountController;

        public FacebookSpecsBase()
        {
            this.facebookApi = MockRepository.GenerateStub<IFacebookApi>();
            this.formsAuth = MockRepository.GenerateMock<IFormsAuth>();
            this.accountController =
                new AccountControllerBuilder().WithJsonRequest().WithFormsAuth(this.formsAuth).WithFacebookApi(this.facebookApi).Build();

        }
    }

}
