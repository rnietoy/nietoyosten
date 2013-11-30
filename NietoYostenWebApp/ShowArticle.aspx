<%@ Page Title="" Language="C#" MasterPageFile="~/NietoYosten.Master" AutoEventWireup="true" CodeBehind="ShowArticle.aspx.cs" Inherits="NietoYostenWebApp.ShowArticle" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="articledate">
        <asp:Literal ID="litArticleDate" runat="server"></asp:Literal>
    </div>
    <h2><asp:Literal ID="litTitle" runat="server"></asp:Literal></h2>
    <asp:Literal ID="litContent" runat="server"></asp:Literal>
</asp:Content>
