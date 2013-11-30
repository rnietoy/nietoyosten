<%@ Page Title="" Language="C#" MasterPageFile="~/NietoYosten.Master" AutoEventWireup="true" CodeBehind="ViewPicture.aspx.cs" Inherits="NietoYostenWebApp.ViewPicture" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div class="grid_12" style="text-align: center">
    <p><asp:Image ID="PageImage" runat="server" /></p>
    <p><asp:Literal ID="PicTitle" runat="server"></asp:Literal></p>
    <p><asp:HyperLink runat="server" ID="DownloadOriginalLink" Text="Download original"/></p>
</div>
</asp:Content>
