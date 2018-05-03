<%@ Page Title="Home Page" Language="C#" AutoEventWireup="true" CodeFile="TMViewer.aspx.cs" Inherits="_Default" %>
<html xmlns="http://www.w3.org/1999/xhtml" >
    <head runat="server"> <title>TrueMarble Web Viewer</title>
    </head>
    <body>
        <form id="frmMain" runat="server">
            <div>
                <p>
                    <input id="txtX" type="hidden" runat="server" value="0"/>
                    <input id="txtY" type="hidden" runat="server"  value="0"/>
                    ZOOM<input id="txtZoom" type="text" runat="server"/>
                    <input id="btnSubmit" type="button" value="Get Tile" runat="server" onserverclick="BtnSubmit_ServerClick"/>
                </p>
                <p>
                    <img src="" id="imgTile" runat="server" />
                </p>
                <p>
                    <input id="btnWest" type="button" value="<" runat="server" onserverclick="BtnWest_ServerClick" />
                    <input id="btnRight" type="button" value=">" runat="server" onserverclick="BtnEast_ServerClick" />
                    <input id="btnNorth" type="button" value="/\" runat="server" onserverclick="BtnNorth_ServerClick" />
                    <input id="btnSouth" type="button" value="\/" runat="server" onserverclick="BtnSouth_ServerClick" />
                </p>

            </div>
            <div runat="server" id=divMsg></div>
        </form>
    </body>
</html>
