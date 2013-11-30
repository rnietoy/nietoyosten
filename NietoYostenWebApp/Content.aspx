<%@ Page Title="" Language="C#" MasterPageFile="~/NietoYosten.Master" AutoEventWireup="true" CodeBehind="Content.aspx.cs" Inherits="NietoYostenWebApp.Content" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div class="grid_12">
    <h1><asp:Literal ID="litPageName" runat="server"/></h1>

    <asp:Repeater ID="rptArticles" runat="server">
        <ItemTemplate>
            <a name='<%# Eval("ArticleId", "Article{0}") %>'></a>
            <div class="articledate">
                <%# Eval("DateCreated", "{0:D}") %><br />
                Written by <%# Eval("UserName") %>
            </div>
            <h2><%# HttpUtility.HtmlEncode(DataBinder.Eval(Container.DataItem, "Title")) %></h2>
            <%# DataBinder.Eval(Container.DataItem, "IntroText") %>
            <asp:Literal ID="litReadMoreLink" runat="server" OnDataBinding="litReadMoreLink_DataBinding"></asp:Literal>
        </ItemTemplate>
    </asp:Repeater>
    <div class="pagelinks">
        <p>
            <asp:Repeater ID="rptPaginationLinks" runat="server">
                <ItemTemplate>
                    <asp:HyperLink ID="litPage" runat="server" NavigateUrl='<%# Eval("Href") %>'><%# Eval("Text") %></asp:HyperLink>
                </ItemTemplate>
            </asp:Repeater>
        </p>
    </div>
</div>
</asp:Content>
