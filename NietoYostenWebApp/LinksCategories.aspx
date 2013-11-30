<%@ Page Title="" Language="C#" MasterPageFile="~/NietoYosten.Master" AutoEventWireup="true" CodeBehind="LinksCategories.aspx.cs" Inherits="NietoYostenWebApp.LinksCategories" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:SqlDataSource ID="dsLinkCategories" runat="server"
        ConnectionString='<%$ ConnectionStrings:NietoYostenConnectionString %>'
        SelectCommand="SELECT Id, Name FROM WeblinkCategories">
    </asp:SqlDataSource>
    <h1>Links</h1>
    <p>
    <asp:Repeater ID="rptLinkCategories" runat="server" DataSourceID="dsLinkCategories">
        <ItemTemplate>
            <asp:HyperLink ID="lnkLinkCategory" runat="server"
                NavigateUrl='<%# Eval("Id", "~/Links.aspx?catId={0}") %>'>
                <%# HttpUtility.HtmlEncode(Eval("Name")) %>
            </asp:HyperLink>
        </ItemTemplate>
        <SeparatorTemplate><br /></SeparatorTemplate>
    </asp:Repeater>
    </p>
</asp:Content>
