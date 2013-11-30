<%@ Page Title="" Language="C#" MasterPageFile="~/admin/Admin.Master" AutoEventWireup="true" CodeBehind="CreateSearchIndex.aspx.cs" Inherits="NietoYostenWebApp.admin.CreateSearchIndex" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2>Create Search Index</h2>
    <p>
    Press the button below to create or re-create the search index for the site. Please note that this
    will delete the previous index. While the index is being re-created searches might not return adequate results.
    </p>
    <p><asp:Button ID="btnCreateIndex" runat="server" Text="Create Index" onclick="btnCreateIndex_Click" /></p>
    <p><asp:Literal ID="litResult" runat="server" Text="" /></p>
    <p><asp:Button ID="btnError" runat="server" Text="Throw Error" OnClick="btnError_Click" /></p>
</asp:Content>
