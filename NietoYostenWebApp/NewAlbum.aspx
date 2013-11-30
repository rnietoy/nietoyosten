<%@ Page Title="" Language="C#" MasterPageFile="~/NietoYosten.Master" AutoEventWireup="true" CodeBehind="NewAlbum.aspx.cs" Inherits="NietoYostenWebApp.NewAlbum" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div class="grid_12">
    <asp:Literal runat="server" ID="ErrorMessage" Visible="False"></asp:Literal>
</div>
<div class="clear"></div>
<div class="grid_12">    
    <table>
        <tr>
            <td>Album Title:</td>
            <td><asp:TextBox runat="server" ID="AlbumTitle"/></td>
        </tr>
        <tr>
            <td>Album folder name:</td>
            <td><asp:TextBox runat="server" ID="AlbumFolder"/></td>
        </tr>
    </table>
    <p><asp:Button runat="server" ID="NewAlbumButton" Text="Create new album" 
            onclick="NewAlbumButton_Click" /></p>
    <asp:RegularExpressionValidator runat="server" ControlToValidate="AlbumFolder" ID="AlbumFolderValidator"
                                    ValidationExpression="[a-z0-9_]{1,32}"
                                    ErrorMessage="Bad folder name. Enter a name of up to 10 characters maximum. Use only letters and numbers. No spaces."
                                    Display="Dynamic" EnableClientScript="True"/>
</div>
</asp:Content>
