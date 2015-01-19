using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using NietoYostenMvc.Controllers;

namespace NietoYostenMvc.Code
{
    public class UrlAuthModule : IHttpModule
    {
        public void Init(HttpApplication app)
        {
            app.AuthenticateRequest += AuthenticateRequestHandler;
        }

        public void Dispose() { }

        private void AuthenticateRequestHandler(Object source, EventArgs e)
        {
            HttpApplication application = (HttpApplication)source;
            HttpContextBase context = new HttpContextWrapper(application.Context);

            if (null != ApplicationController.GetCurrentUser(context)) return;

            // Restrict access to private resources (i.e. images)
            if (context.Request.Path.StartsWith("/content/pictures", StringComparison.InvariantCultureIgnoreCase) ||
                context.Request.Path.StartsWith("/azure/pictures", StringComparison.InvariantCultureIgnoreCase) ||
                context.Request.Path.StartsWith("/images/caras", StringComparison.InvariantCultureIgnoreCase))
            {
                context.Response.Redirect("~/account/login");
            }
        }
    }
}