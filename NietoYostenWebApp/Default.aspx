<%@ Page Title="" Language="C#" MasterPageFile="~/NietoYosten.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="NietoYostenWebApp.Default" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:SqlDataSource ID="homePageArticles" runat="server" 
        ConnectionString="<%$ ConnectionStrings:NietoYostenConnectionString %>" 
        SelectCommand="SELECT a.Content,
            a.ArticleId, a.IntroText, a.Title, h.Position, a.DateCreated, a.Published, u.UserName
            FROM Article AS a INNER JOIN 
            HomePageArticles AS h ON a.ArticleId = h.ArticleId INNER JOIN
            aspnet_Users AS u ON a.CreatedBy = u.UserId
            WHERE h.Enabled = 1 AND a.Published = 1
            ORDER BY Position">
    </asp:SqlDataSource>
    <asp:SqlDataSource ID="latestNews" runat="server"
        ConnectionString="<%$ ConnectionStrings:NietoYostenConnectionString %>"
        SelectCommand="SELECT TOP 5 a.ArticleId, a.Title, s.Name FROM Article AS a INNER JOIN
            Section AS s ON a.SectionId = s.SectionId WHERE s.Name = 'News'
            ORDER BY DateCreated DESC">
    </asp:SqlDataSource>
    <div class="grid_12">
        <div style="text-align: center; margin: auto; display: block">
            <p><img src="Images/HomepNY.jpg" width="740px" alt="Nieto Yosten Family Picture" /></p>
        </div>
        <asp:LoginView ID="LoginView1" runat="server">
            <AnonymousTemplate>
                <p>Bienvenidos a la pagina de la Familia Nieto Yosten! Utiliza el link de "Login" arriba
                para accesar el sitio.</p>
            </AnonymousTemplate>
            <LoggedInTemplate>
                <span class="latestNews">
                    <h3>Latest News</h3>
                    <asp:Repeater ID="rptLatestNews" runat="server" DataSourceID="latestNews">
                        <ItemTemplate>
                            <div class="newsitem"><a runat="server" 
                                href='<%# String.Format("~/Content/{0}#Article{1}", HttpUtility.UrlEncode(Eval("Name").ToString()), Eval("ArticleId")) %>'>
                                <%# HttpUtility.HtmlEncode(Eval("Title")) %>
                            </a></div>
                        </ItemTemplate>
                    </asp:Repeater>
                </span>
                <asp:Repeater ID="rptHomePageArticles" runat="server" DataSourceId="homePageArticles">
                    <ItemTemplate>
                        <div class="articledate">
                            <%# Eval("DateCreated", "{0:D}") %><br />
                            Written by <%# Eval("UserName") %>
                        </div>
                        <h2><%# HttpUtility.HtmlEncode(DataBinder.Eval(Container.DataItem, "Title")) %></h2>
                        <%# DataBinder.Eval(Container.DataItem, "IntroText") %>
                        <p>
                            <asp:HyperLink ID="lnkReadMore" runat="server"
                                NavigateUrl='<%# Eval("ArticleId", "~/ShowArticle.aspx?id={0}") %>'
                                Visible='<%# Eval("Content").ToString() != "" %>'>
                                Read More...
                            </asp:HyperLink>
                        </p>
                    </ItemTemplate>
                </asp:Repeater>
            </LoggedInTemplate>
        </asp:LoginView>
    </div>
</asp:Content>
