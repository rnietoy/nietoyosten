<%@ Page Title="" Language="C#" MasterPageFile="~/NietoYosten.Master" AutoEventWireup="true" CodeBehind="Search.aspx.cs" Inherits="NietoYostenWebApp.Search" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <h2>Search Results</h2>
    <asp:Repeater ID="rptSearchResults" runat="server">
        <ItemTemplate>
            <a href='<%# Eval("ArticleId", "~/ShowArticle.aspx?id={0}") %>' runat="server">
                <%# HttpUtility.HtmlEncode(Eval("Title")) %>
            </a>
            &nbsp;-&nbsp;
            By <%# HttpUtility.HtmlEncode(Eval("Author")) %> on <%# HttpUtility.HtmlEncode(Eval("Date")) %>
        </ItemTemplate>
        <SeparatorTemplate><br /></SeparatorTemplate>
    </asp:Repeater>
</asp:Content>
