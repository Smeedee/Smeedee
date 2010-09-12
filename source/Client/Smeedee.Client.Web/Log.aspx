<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Log.aspx.cs" Inherits="Smeedee.Client.Web.Log" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Smeedee Log</title>
<style type="text/css">
html, body 
{
    width:100%;
    margin:0;
    background-color:#000000;
    color:#fff;
    font-family:Arial;
}
.logWrapper 
{
    width:90%;
    margin:auto;
    color:White;
}
.header, .spacer 
{
    height:48px;
    padding:11px 0; /*padding top + padding bottom + height == 70px corresponds to silverlight app */
    width:100%;
    font-size:30px;
    color:#595959;
}
.header img 
{
    float:right;
}
td
{
    padding:5px;
    text-align:left;
    vertical-align:top;
    min-width:100px;
    white-space:pre-wrap;
}
table
{
    word-wrap: break-word; /*CSS3 - does the trick for opera*/   
    word-break: break-all; /*CSS3 - does the trick for chrome, ie*/   
}
th 
{
    background-color:black;
    border:1px solid black;
    border-bottom:1px solid white;
}
table 
{   
    background-color:#595959;
    border: 1px solid white;
}
.altRow 
{
    background-color:#404040;
}
</style>
<!--[if lte IE 7]>
<style type="text/css"> th { border: 1px solid white } </style>
<![endif]-->
</head>
<body>
    <form id="form1" runat="server">
    <div class="logWrapper">
        <div class="header">
            <img src="Smeedee-icon-48x48.png" />
            <div>Smeedee Log</div>
        </div>
        <asp:GridView id="LogGrid" Runat="Server" AlternatingRowStyle-CssClass="altRow" AllowSorting="true" />
        <asp:Button id="DeleteAllButton" Runat="Server" Text="Delete All Entries" />
        <div class="spacer"></div>
    </div>
    </form>
</body>
</html>
