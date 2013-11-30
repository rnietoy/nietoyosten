using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Configuration;
// Lucene
using Lucene.Net.Store;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Index;
using Lucene.Net.Documents;

namespace NietoYostenWebApp.admin
{
    public partial class CreateSearchIndex : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnCreateIndex_Click(object sender, EventArgs e)
        {
            MyLucene.CreateIndex();
            // Set status
            //litResult.Text = string.Format("Done. Indexed {0} documents.", count);
        }

        protected void btnError_Click(object sender, EventArgs e)
        {
            throw new Exception("Button Exception");
        }
    }
}