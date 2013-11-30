<%@ Page Title="" Language="C#" MasterPageFile="~/NietoYosten.Master" AutoEventWireup="true" CodeBehind="UploadPicture.aspx.cs" Inherits="NietoYostenWebApp.UploadPicture" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="http://ajax.googleapis.com/ajax/libs/jqueryui/1.7.2/themes/start/jquery-ui.css" rel="stylesheet" media="screen" type="text/css" />
    <script type="text/javascript" src="http://ajax.googleapis.com/ajax/libs/jquery/1.3.2/jquery.min.js"></script>
    <script type="text/javascript" src="http://ajax.googleapis.com/ajax/libs/jqueryui/1.7.2/jquery-ui.min.js"></script>
    <script type="text/javascript" src="UploadPicture.js"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div class="grid_12">
    <p ID="ErrorMessage" class="errormsg" style="display: none"></p>
</div>
<div class="clear"></div>
<div class="grid_12">
    <h1>Upload pictures</h1>
    <p id="TopParagraph">
        <input type="file" id="files" name="files[]" multiple="true" value="Select Files" />  
        <input type="button" id="UploadButton" value="Upload"/>
    </p>
    <table id="FileTable" border="1" style="display: none;">
        <tr>
            <th>File</th>
            <th>Upload progress</th>
            <th>Status</th>
        </tr>
    </table>
</div>
</asp:Content>
