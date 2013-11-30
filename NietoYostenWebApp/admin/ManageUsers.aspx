<%@ Page Title="" Language="C#" MasterPageFile="~/admin/Admin.Master" AutoEventWireup="true" CodeBehind="ManageUsers.aspx.cs" Inherits="NietoYostenWebApp.admin.ManageUsers" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2>Manage Users</h2>
    <asp:GridView ID="UsersGrid" runat="server" AutoGenerateColumns="false" 
        onrowcommand="UsersGrid_RowCommand">
        <Columns>
            <asp:BoundField DataField="UserName" HeaderText="User Name" SortExpression="UserName" />
            <asp:BoundField DataField="Email" HeaderText="Email" SortExpression="Email" />
            <asp:BoundField DataField="Roles" HeaderText="Roles" SortExpression="Roles" />
            <asp:BoundField DataField="Approved" HeaderText="Approved" />
            <asp:ButtonField ButtonType="Button" CommandName="DeleteUser" Text="Delete" />
        </Columns>
    </asp:GridView>
</asp:Content>
