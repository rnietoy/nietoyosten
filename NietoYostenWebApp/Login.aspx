<%@ Page Title="" Language="C#" MasterPageFile="~/NietoYosten.Master" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="NietoYostenWebApp.Login" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript" src="Login.js"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div id="fb-root"></div>
<div class="grid_12">
    <div class="grid_6 alpha">
    <asp:Panel ID="LoginPanel" runat="server" DefaultButton="Login1$LoginButton">
        <asp:Login ID="Login1" runat="server">
        </asp:Login>
    </asp:Panel>
    <p>
    Olvidaste tu password? Da click
        <asp:HyperLink ID="recoverPassword" NavigateUrl="~/RecoverPassword.aspx" runat="server">aquí</asp:HyperLink>.
    </p>
    <p>No tienes una cuenta aún? Da click 
        <asp:HyperLink ID="createUser" NavigateUrl="~/CreateUser.aspx" runat="server">aquí</asp:HyperLink> para crear tu cuenta.
    </p>
    </div>
    <div class="grid_6 omega">
        <p style="text-align: center">
            <fb:login-button id="fblogin" registration-url="FbRegister.aspx" onlogin="fblogin_onlogin()" />
        </p>
    </div>
    <asp:HiddenField runat="server" ID="fbAppId" ClientIDMode="Static" Value="blank"/>
</div>
</asp:Content>
