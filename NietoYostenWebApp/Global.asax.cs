using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using System.Web.Routing;
using Lucene.Net.Store;
using Lucene.Net.Index;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Analysis.Tokenattributes;

namespace NietoYostenWebApp
{
    public class Global : System.Web.HttpApplication
    {
        

        protected void Application_Start(object sender, EventArgs e)
        {
            RegisterRoutes(RouteTable.Routes);
            //InitTraceListener();
            //InitLucene();
            
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            MyLucene.InitLucene(Server);
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {
            
        }

        protected void Application_End(object sender, EventArgs e)
        {
            MyLucene.CloseLucene();
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.MapPageRoute("", "Content/{SectionName}", "~/Content.aspx");
            routes.MapPageRoute("", "admin/EditArticle/{ArticleId}", "~/admin/EditArticle.aspx");
        }

        

        void InitTraceListener()
        {
            string traceFile = Server.MapPath("~") + "/nietoyosten.log";
            System.IO.FileStream traceLog =
                new System.IO.FileStream(traceFile, System.IO.FileMode.OpenOrCreate);
            System.Diagnostics.TextWriterTraceListener listener =
                new System.Diagnostics.TextWriterTraceListener(traceLog);
            System.Diagnostics.Trace.Listeners.Add(listener);
        }
    }
}