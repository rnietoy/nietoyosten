<%@ Page Title="" Language="C#" MasterPageFile="~/admin/Admin.Master" AutoEventWireup="true"
    CodeBehind="AdminLinks.aspx.cs" Inherits="NietoYostenWebApp.admin.AdminLinks" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:LinqDataSource ID="dsLinkCategories" runat="server" ContextTypeName="NietoYostenWebApp.NietoYostenDbDataContext"
        TableName="WeblinkCategories">
    </asp:LinqDataSource>
    <asp:LinqDataSource ID="dsLinks" runat="server" ContextTypeName="NietoYostenWebApp.NietoYostenDbDataContext"
        TableName="Weblinks" Where="CategoryId == @CategoryId" EnableDelete="true" EnableUpdate="true">
        <WhereParameters>
            <asp:ControlParameter ControlID="ddlLinkCategories" Name="CategoryId" PropertyName="SelectedValue"
                DbType="Int32" />
        </WhereParameters>
    </asp:LinqDataSource>
    <h2>
        Admin Links</h2>
    <p>
        Select Category:&nbsp;
        <asp:DropDownList ID="ddlLinkCategories" runat="server" DataSourceID="dsLinkCategories"
            DataTextField="Name" DataValueField="Id" AutoPostBack="true" OnSelectedIndexChanged="ddlLinkCategories_SelectedIndexChanged">
        </asp:DropDownList>
        &nbsp;
        <asp:Button ID="btnAddLink" runat="server" Text="Add Link" OnClick="btnAddLink_Click" />
    </p>
    <asp:GridView ID="gvLinks" runat="server" DataSourceID="dsLinks" AutoGenerateColumns="false"
        DataKeyNames="Id" SelectedIndex="0" RowStyle-Wrap="true">
        <Columns>
            <asp:BoundField DataField="Title" HeaderText="Title" SortExpression="Title" ItemStyle-Width="200px" />
            <asp:BoundField DataField="Url" HeaderText="Url" ControlStyle-Width="250px" ItemStyle-Wrap="true"
                ItemStyle-Width="250px" />
            <asp:BoundField DataField="Description" HeaderText="Description" SortExpression="Description" />
            <asp:CommandField ShowDeleteButton="true" ShowEditButton="true" />
        </Columns>
    </asp:GridView>
</asp:Content>
