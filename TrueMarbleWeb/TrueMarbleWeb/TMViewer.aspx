<%@ Page Title="Home Page" Language="C#" AutoEventWireup="true" CodeFile="TMViewer.aspx.cs" Inherits="_Default" %>

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server"> <title>TrueMarble Web Viewer</title></head>
<body>
    <form id="frmMain" runat="server">
    <div>
        X<input type="text" id="txtX" runat="server"/> 
        Y<input type="text" id="txtY" runat="server"/>
        Zoom<input type="text" id="txtZoom" runat="server"/>
        <input id="btnSubmit" type="button" value="Get Tile" runat="server" onserverclick="BtnSubmit_ServerClick" />
    </div>
    <p>
        <img src="" id="imgTile" runat="server" />
    </p>
    <div runat="server" id="divMsg">

    </div>
    </form>
</body>
</html>