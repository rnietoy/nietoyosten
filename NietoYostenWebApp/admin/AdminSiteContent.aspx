<%@ Page Title="" Language="C#" MasterPageFile="~/admin/Admin.Master" AutoEventWireup="true"
    CodeBehind="AdminSiteContent.aspx.cs" Inherits="NietoYostenWebApp.admin.AdminSiteContent" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:SqlDataSource ID="articlesDataSource" runat="server" ConnectionString="<%$ ConnectionStrings:NietoYostenConnectionString %>"
        SelectCommand="SELECT [ArticleId], [Title], [Published] FROM [Article] WHERE [SectionId] = @SectionId">
        <SelectParameters>
            <asp:ControlParameter Name="SectionId" ControlID="ddlSection" PropertyName="SelectedValue" />
        </SelectParameters>
    </asp:SqlDataSource>
    <h2>Site Content</h2>
    <strong>Section:&nbsp;</strong>
    <asp:DropDownList ID="ddlSection" AutoPostBack="true" runat="server">
    </asp:DropDownList>
    &nbsp;
    <asp:Button ID="btnAddArticle" runat="server" OnClick="btnAddArticle_Click" Text="Add Article" />&nbsp;
    <asp:Button ID="btnPublish" runat="server" Text="Publish" OnClick="btnPublishAndUnpublish_Click" />&nbsp;
    <asp:Button ID="btnUnpublish" runat="server" Text="Unpublish" OnClick="btnPublishAndUnpublish_Click" />&nbsp;
    <br />
    <br />
    <asp:Repeater ID="rptArticles" runat="server">
        <HeaderTemplate>
            <table border="1" cellpadding="4" cellspacing="0">
                <tr>
                    <td>
                        <b>Check</b>
                    </td>
                    <td>
                        <b>Id</b>
                    </td>
                    <td>
                        <b>Title</b>
                    </td>
                    <td>
                        <b>Published</b>
                    </td>
                </tr>
        </HeaderTemplate>
        <ItemTemplate>
            <tr>
                <td>
                    <asp:CheckBox ID='chkArticle' runat="server" OnDataBinding="chkArticle_DataBinding" />
                </td>
                <td>
                    <%# DataBinder.Eval(Container.DataItem, "ArticleId") %>
                </td>
                <td>
                    <asp:HyperLink ID="lnkArticle" runat="server"
                        NavigateUrl='<%# String.Format("EditArticle/{0}", HttpUtility.HtmlEncode(Eval("ArticleId"))) %>'>
                        <%# HttpUtility.HtmlEncode(DataBinder.Eval(Container.DataItem, "Title")) %>
                    </asp:HyperLink>
                </td>
                <td>
                    <%# Eval("Published").ToString() == "True" ? "Yes" : "No" %>
                </td>
            </tr>
        </ItemTemplate>
        <FooterTemplate>
            </table>
        </FooterTemplate>
    </asp:Repeater>
</asp:Content>
