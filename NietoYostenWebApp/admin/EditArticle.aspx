<%@ Page Title="" Language="C#" MasterPageFile="~/admin/Admin.Master" AutoEventWireup="true"
    CodeBehind="EditArticle.aspx.cs" Inherits="NietoYostenWebApp.admin.EditArticle" %>

<%@ Register Assembly="CKEditor.NET" Namespace="CKEditor.NET" TagPrefix="CKEditor" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2>
        Edit Article</h2>
    <p>
        Instructions:<br />
        Write an intro text in the "Intro Text" text editor and then write the rest of the
        article in the "Full Text" text editor. If your article is short, you can write
        the whole text in the "Intro Text" and leave the "Full Text" editor empty.
    </p>
    <p>
        Section:
        <asp:DropDownList ID="ddlSection" runat="server">
        </asp:DropDownList>
        <br />
        Title:&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
        <asp:TextBox ID="txtTitle" Width="500px" runat="server"></asp:TextBox>
    </p>
    <p>
        Intro Text:<br />
        <CKEditor:CKEditorControl ID="introEditor" runat="server"></CKEditor:CKEditorControl>
    </p>
    <p>
        Full Text:<br />
        <CKEditor:CKEditorControl ID="contentEditor" runat="server"></CKEditor:CKEditorControl>
    </p>
    <p>
        <asp:Button ID="btnSave" runat="server" Text="Save" OnClick="btnSave_Click" />&nbsp;&nbsp;
    </p>
</asp:Content>
