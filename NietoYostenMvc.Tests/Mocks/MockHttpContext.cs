using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace NietoYostenMvc.Tests.Mocks
{
    class MockHttpRequest : HttpRequestBase
    {
        public Dictionary<string, string> Items { get; set; }

        public override string this[string key]
        {
            get
            {
                return this.Items[key];

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
        private readonly HttpRequestBase request = new MockHttpRequest();

        public override HttpRequestBase Request
        {
            get { return this.request; }
        }

        public MockHttpContext()
        {
        }

        public MockHttpContext(HttpRequestBase httpRequest)
        {
            this.request = httpRequest;
        }
    }
}
