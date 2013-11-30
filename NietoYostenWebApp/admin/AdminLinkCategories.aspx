<%@ Page Title="" Language="C#" MasterPageFile="~/admin/Admin.Master" AutoEventWireup="true"
    CodeBehind="AdminLinkCategories.aspx.cs" Inherits="NietoYostenWebApp.admin.LinkCategories" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:LinqDataSource ID="dsLinkCategories" runat="server" ContextTypeName="NietoYostenWebApp.NietoYostenDbDataContext"
        EnableDelete="true" EnableInsert="true" EnableUpdate="true" TableName="WeblinkCategories">
    </asp:LinqDataSource>
    <h2>
        Admin Link Categories</h2>
    <p>
        <asp:GridView ID="gvCategories" runat="server" AutoGenerateColumns="false" DataKeyNames="Id"
            DataSourceID="dsLinkCategories">
            <Columns>
                <asp:BoundField DataField="Name" HeaderText="Category Name" SortExpression="Name" />
                <asp:CommandField ShowDeleteButton="true" ShowEditButton="true" />
            </Columns>
        </asp:GridView>
    </p>
    <p>
        Add new Category:<br />
        <asp:TextBox ID="txtNewCategory" runat="server" Width="200px"></asp:TextBox>&nbsp;&nbsp;
        <asp:Button ID="btnAddCategory" runat="server" Text="Add Category" OnClick="btnAddCategory_Click" />
    </p>
</asp:Content>
