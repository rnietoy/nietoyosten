<%@ Page Title="" Language="C#" MasterPageFile="~/NietoYosten.Master" AutoEventWireup="true" CodeBehind="Links.aspx.cs" Inherits="NietoYostenWebApp.Links" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:SqlDataSource ID="dsLinks" runat="server"
        ConnectionString='<%$ ConnectionStrings:NietoYostenConnectionString %>'
        SelectCommand="SELECT Title, Url FROM Weblinks WHERE CategoryId=@CategoryId">
        <SelectParameters>
            <asp:QueryStringParameter Name="CategoryId" QueryStringField="catId" />
        </SelectParameters>
    </asp:SqlDataSource>
    <h1><asp:Literal ID="litHeading" runat="server"></asp:Literal></h1>
    <p>
    <asp:Repeater ID="rptLinks" runat="server" DataSourceID="dsLinks">
        <ItemTemplate>
            <asp:HyperLink ID="lnkLink" runat="server" 
                NavigateUrl='<%# HttpUtility.HtmlEncode(Eval("Url")) %>'>
                <%# HttpUtility.HtmlEncode(Eval("Title")) %>
            </asp:HyperLink>
        </ItemTemplate>
        <SeparatorTemplate><br /></SeparatorTemplate>
    </asp:Repeater>
    </p>
</asp:Content>
