<%@ Page Title="" Language="C#" MasterPageFile="~/NietoYosten.Master" AutoEventWireup="true" CodeBehind="FbRegister.aspx.cs" Inherits="NietoYostenWebApp.FbRegister" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div class="grid_12">    
    <iframe runat="server"
        ID="fbRegisterFrame"
        src='blank'
        scrolling="auto"
        frameborder="no"
        style="border:none"
        allowTransparency="true"
        width="100%"
        height="500">
    </iframe>
</div>
</asp:Content>
