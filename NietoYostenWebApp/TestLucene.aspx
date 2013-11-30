<%@ Page Language="C#" Debug="true" %>
<%@ Import Namespace="Lucene.Net.Store" %>
<%@ Import Namespace="Lucene.Net.Analysis.Standard" %>
<%@ Import Namespace="Lucene.Net.Index" %>
<%@ Import Namespace="Lucene.Net.Analysis.Tokenattributes" %>

<script runat="server">
    
    Directory directory;
    StandardAnalyzer analyzer;
    IndexWriter writer;
    
    void btnInit_Click(Object sender, EventArgs e)
    {
        //Directory directory;
        //StandardAnalyzer analyzer;
        //IndexWriter writer;
        
        string indexDir = Server.MapPath("~/LuceneIndex");
        
        directory = FSDirectory.Open(new System.IO.DirectoryInfo(indexDir));
        analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_29);
        
        if (IndexWriter.IsLocked(directory))
        {
            System.Diagnostics.Debug.WriteLine("Something left a lock in the index folder: deleting it");
            IndexWriter.Unlock(directory);
            System.Diagnostics.Debug.WriteLine("Lock deleted.");
        }
        writer = new IndexWriter(directory, analyzer, IndexWriter.MaxFieldLength.UNLIMITED);
        lblStatus.Text = "Init sucessful";
    }

    void btnClose_Click(object sender, EventArgs e)
    {
        writer.Close();
        directory.Close();
        lblStatus.Text = "Close sucessful";
    }
   
</script>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Test Lucene</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:Button ID="btnInit" runat="server" OnClick="btnInit_Click" Text="Init Lucene" />
        <asp:Button ID="btnClose" runat="server" OnClick="btnClose_Click" Text="Close Lucene" />
        <p>
        <asp:Label ID="lblStatus" runat="server" />
        </p>
    </div>
    </form>
</body>
</html>
