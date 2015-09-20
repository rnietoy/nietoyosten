using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace NietoYostenMvc.Tests.Mocks
{
    class MockHttpRequest : HttpRequestBase
    {
        private readonly Dictionary<string, string> items = new Dictionary<string, string>();
        private readonly NameValueCollection headers = new NameValueCollection();

        public override string this[string key]
        {
            get
            {
                string value;
                this.items.TryGetValue(key, out value);
                return value;
            }
        }

        public override NameValueCollection Headers => this.headers;

        public Dictionary<string, string> Items => this.items;
    }

    class MockHttpContext : HttpContextBase
    {
        private readonly HttpRequestBase request = new MockHttpRequest();

        public override HttpRequestBase Request => this.request;

        public MockHttpContext()
        {
        }

        public MockHttpContext(HttpRequestBase httpRequest)
        {
            this.request = httpRequest;
        }
    }
}
