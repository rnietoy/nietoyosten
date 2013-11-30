<%@ Page Title="" Language="C#" MasterPageFile="~/admin/Admin.Master" AutoEventWireup="true"
    CodeBehind="AddLink.aspx.cs" Inherits="NietoYostenWebApp.admin.AddLink" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:LinqDataSource ID="dsLinkCategories" runat="server" ContextTypeName="NietoYostenWebApp.NietoYostenDbDataContext"
        TableName="WeblinkCategories">
    </asp:LinqDataSource>
    <asp:LinqDataSource ID="dsLink" runat="server" ContextTypeName="NietoYostenWebApp.NietoYostenDbDataContext"
        TableName="WebLinks" EnableInsert="true" OnInserting="dsLink_Inserting">
    </asp:LinqDataSource>
    <h2>
        Add a Weblink</h2>
    <p>
        Select Category:&nbsp;
        <asp:DropDownList ID="ddlLinkCategories" runat="server" DataSourceID="dsLinkCategories"
            DataTextField="Name" DataValueField="Id">
        </asp:DropDownList>
    </p>
    <p>
        <asp:DetailsView ID="LinkDetails" runat="server" DataSourceID="dsLink" AutoGenerateRows="false"
            DataKeyNames="Id" DefaultMode="Insert" OnItemInserted="LinkDetails_ItemInserted">
            <Fields>
                <asp:BoundField DataField="Url" HeaderText="Url" />
                <asp:BoundField DataField="Title" HeaderText="Title" />
                <asp:BoundField DataField="Description" HeaderText="Description" />
                <asp:CommandField ShowInsertButton="true" ShowCancelButton="false" />
            </Fields>
        </asp:DetailsView>
    </p>
</asp:Content>
