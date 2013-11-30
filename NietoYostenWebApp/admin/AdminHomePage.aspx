<%@ Page Title="" Language="C#" MasterPageFile="~/admin/Admin.Master" AutoEventWireup="true"
    CodeBehind="AdminHomePage.aspx.cs" Inherits="NietoYostenWebApp.admin.AdminHomePage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2>
        Home Page Articles</h2>
    <table border="1" cellspacing="0" cellpadding="4">
        <tr style="font-weight: bold">
            <td>
                &nbsp;
            </td>
            <td>
                Show Article
            </td>
            <td>
                Section
            </td>
            <td>
                Article
            </td>
        </tr>
        <tr>
            <td>
                First
            </td>
            <td>
                <asp:CheckBox ID="chkShow1" runat="server" />
            </td>
            <td>
                <asp:DropDownList ID="ddlSection1" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlSection1_SelectedIndexChanged" />
            </td>
            <td>
                <asp:DropDownList ID="ddlArticle1" runat="server" />
            </td>
        </tr>
        <tr>
            <td>
                Second
            </td>
            <td>
                <asp:CheckBox ID="chkShow2" runat="server" />
            </td>
            <td>
                <asp:DropDownList ID="ddlSection2" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlSection2_SelectedIndexChanged" />
            </td>
            <td>
                <asp:DropDownList ID="ddlArticle2" runat="server" />
            </td>
        </tr>
        <tr>
            <td>
                Third
            </td>
            <td>
                <asp:CheckBox ID="chkShow3" runat="server" />
            </td>
            <td>
                <asp:DropDownList ID="ddlSection3" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlSection3_SelectedIndexChanged" />
            </td>
            <td>
                <asp:DropDownList ID="ddlArticle3" runat="server" />
            </td>
        </tr>
        <tr>
            <td>
                Fourth
            </td>
            <td>
                <asp:CheckBox ID="chkShow4" runat="server" />
            </td>
            <td>
                <asp:DropDownList ID="ddlSection4" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddlSection4_SelectedIndexChanged" />
            </td>
            <td>
                <asp:DropDownList ID="ddlArticle4" runat="server" />
            </td>
        </tr>
    </table>
    <br />
    <asp:Button ID="btnSave" runat="server" Text="Save" OnClick="btnSave_Click" />&nbsp;
</asp:Content>
