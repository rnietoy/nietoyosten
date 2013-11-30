<%@ Page Title="" Language="C#" MasterPageFile="~/NietoYosten.Master" AutoEventWireup="true" CodeBehind="ViewAlbum.aspx.cs" Inherits="NietoYostenWebApp.ViewAlbum" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
        $(document).ready(function () {
            $('#DeletePictureButton').click(function () {
                var yes = confirm("Are you sure you want to delete the selected picture(s)?");
                if (yes) {
                    $('#DeleteConfirmed').attr({ value: "true" });
                } else {
                    $('#DeleteConfirmed').attr({ value: "false" });
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
    <!-- Pictures menu column -->
    <div class="grid_2 alpha">
        <p><asp:Button runat="server" ID="UploadPictureButton" Text="Upload picture(s)"/></p>
        <p><asp:Button runat="server" ID="DeletePictureButton" Text="Delete picture(s)" 
                onclick="DeletePicture_Click" ClientIDMode="Static"  /></p>
        <p><asp:HiddenField runat="server" ID="DeleteConfirmed" Value="false" ClientIDMode="Static"/></p>
    </div>
    
    <!-- Picture thumbnails grid (4 pics per row) -->
    <div class="grid_10 omega">
        <asp:Repeater ID="rptThumbnailGrid" runat="server">
            <ItemTemplate>
                <!-- Picture thumbnail row -->
                <div class="grid_10">
                    <asp:Repeater ID="rptThumbnailRow" runat="server" DataSource="<%# Container.DataItem %>" ClientIDMode="Predictable">
                        <ItemTemplate>
                            <asp:Panel runat="server" class='<%# string.Format("grid_2 {0}", Eval("AlphaOmega")) %>' style="text-align: center"
                                       Visible='<%# (bool)Eval("Empty") == false %>'>
                                <p><asp:ImageButton
                                    ImageUrl='<%# Eval("RelativePath") %>'
                                    PostBackUrl='<%# string.Format("~/ViewPicture.aspx?PictureId={0}", DataBinder.Eval(Container.DataItem, "PictureId")) %>'
                                    runat="server"/>
                                </p>
                                <p><asp:CheckBox runat="server" ID='PictureCheckbox' Text='<%# Eval("Title") %>' /></p>
                                <input type="hidden" id="PictureId" value='<%# Eval("PictureId") %>'/>
                            </asp:Panel>
                        </ItemTemplate>
                    </asp:Repeater>
                </div>
            </ItemTemplate>
        </asp:Repeater>
    </div>
</div>
</asp:Content>
