<%@ Page Title="" Language="C#" MasterPageFile="~/admin/Admin.Master" AutoEventWireup="true" CodeBehind="PendingUsers.aspx.cs" Inherits="NietoYostenWebApp.admin.PendingUsers" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2>Pending Users</h2>
    <asp:GridView ID="PendingUsersGrid" runat="server" AutoGenerateColumns="false" 
        onrowcommand="PendingUsersGrid_RowCommand">
        <Columns>
            <asp:BoundField DataField="UserName" HeaderText="User Name" SortExpression="UserName" />
            <asp:BoundField DataField="Email" HeaderText="Email" SortExpression="Email" />
            <asp:ButtonField ButtonType="Button" CommandName="Approve" Text="Approve" />
            <asp:ButtonField ButtonType="Button" CommandName="Reject" Text="Reject" />
        </Columns>
    </asp:GridView>
</asp:Content>
