<%@ Page Title="" Language="C#" MasterPageFile="~/NietoYosten.Master" AutoEventWireup="true" CodeBehind="Pictures.aspx.cs" Inherits="NietoYostenWebApp.Pictures" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
        $(document).ready(function () {
            $('#DeleteAlbumButton').click(function () {
                var yes = confirm("Are you sure you want to delete the selected albums(s)? All pictures contained in the album(s) will be deleted as well.");
                if (yes) {
                    $('#DeleteAlbumConfirmed').attr({ value: "true" });
                } else {
                    $('#DeleteAlbumConfirmed').attr({ value: "false" });
                }
            });
        });
	</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div class="grid_12">
    <asp:Literal runat="server" ID="ErrorMessage" Visible="False"></asp:Literal>
</div>
<div class="clear"></div>
<div class="grid_12">
    <div class="grid_2 alpha">
        <p><asp:HyperLink runat="server" ID="NewAlbumLink" NavigateUrl="~/NewAlbum.aspx">New Album</asp:HyperLink></p>
        <p><asp:Button runat="server" ID="DeleteAlbumButton" Text="Delete album(s)" ClientIDMode="Static" 
                onclick="DeleteAlbumButton_Click"/></p>
        <asp:HiddenField runat="server" ID="DeleteAlbumConfirmed" Value="false" ClientIDMode="Static"/>
    </div>
    <div class="grid_10 omega">
        <asp:Repeater ID="rptAlbums" runat="server" >
            <ItemTemplate>
                <p>
                <asp:CheckBox runat="server" ID="AlbumCheckbox"/>
                <asp:HyperLink runat="server" NavigateUrl='<%# Eval("Id", "~/ViewAlbum.aspx?AlbumId={0}") %>' >
                    <%# Eval("Title") %>
                </asp:HyperLink>
                <asp:HiddenField runat="server" ID="AlbumId" Value='<%# Eval("Id") %>' />
                </p>
            </ItemTemplate>
        </asp:Repeater>
    </div>
</div>
</asp:Content>
