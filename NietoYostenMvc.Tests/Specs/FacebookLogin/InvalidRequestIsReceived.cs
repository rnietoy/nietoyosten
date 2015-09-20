
using System.Web.Mvc;
using NietoYostenMvc.Code;
using NietoYostenMvc.Tests.Mocks;
using Xunit;

namespace NietoYostenMvc.Tests.Specs.FacebookLogin
{
    public class InvalidRequestIsReceived : FacebookSpecsBase
    {

        [Fact]
        public void SignedRequestIsEmpty()
        {
            var result = this.accountController.FbLogin(
                "",
                "",
                "/Login") as JsonResult;

            NyResult data = result.Data as NyResult;
            Assert.False(data.Success);
        }

        [Fact]
        public void SignedRequestIsInvalid()
        {
            var result = this.accountController.FbLogin(
                "asdf",
                "asdf",
                "/Login") as JsonResult;

            NyResult data = result.Data as NyResult;
            Assert.False(data.Success);
        }

        [Fact]
        public void RequestIsNotJson()
        {
            var controller =
                new AccountControllerBuilder().WithFormsAuth(this.formsAuth).WithFacebookApi(this.facebookApi).Build();

            var result = controller.FbLogin("asdf", "asdf", "/Login");
            Assert.IsType(typeof(HttpNotFoundResult), result);
        }
    }
}
