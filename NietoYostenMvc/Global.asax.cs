using System.Collections.Specialized;
using System.Configuration;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using ImageResizer.Configuration;
using ImageResizer.Plugins.AzureReader2;

namespace NietoYostenMvc
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            Elmah.Mvc.Bootstrap.Initialize();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            // Set AzureReader2 plugin
            var nvc = new NameValueCollection();
            nvc["endpoint"] = "https://nietoyosten.blob.core.windows.net";
            nvc["connectionString"] = "DefaultEndpointsProtocol=https;AccountName=nietoyosten;AccountKey=" + ConfigurationManager.AppSettings["STORAGE_ACCOUNT_KEY"];
            new AzureReader2Plugin(nvc).Install(Config.Current);
        }
    }
}