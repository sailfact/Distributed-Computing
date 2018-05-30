<%@ Page Title="TrueMarble Viewer" Language="C#" AutoEventWireup="true" CodeFile="TMViewer.aspx.cs" Inherits="_Default" %>
<html xmlns="http://www.w3.org/1999/xhtml">
    <head runat="server">
        <title>TrueMarble Viewer</title>
        <script>
            function GetNumTileXRPCAsync(iZoom, fnOnCompletion) {
                req = null;
                if (window.XMLHttpRequest != undefined)
                    req = new XMLHttpRequest();
                else
                    req = new ActiveXObject("Microsoft.XMLHTTP");

                req.onreadystatechange = fnOnCompletion;
                req.open("POST", "http://localhost/WebServices/TMWebService.svc", true);
                // Perform call using simple CGI message format rather than SOAP
                req.setRequestHeader("Content-Type", "text/xml");
                req.setRequestHeader("SOAPAction", "http://tempuri.org/ITMWebService/GetNumTiles/GetNumAcross");

                var sMsg = '<soap:Envelope xmlns:s="http://schemas.xmlsoap.org/soap/envelope/"> \
                                <soap:Header> \
                                    <Action soap:mustUnderstand="1" xmlns="http://schemas.microsoft.com/ws/2005/05/addressing/none">http://tempuri.org/ITMWebService/GetNumTilesAcross</Action> \
                                </soap:Header>\
                                <soap:Body> \
                                    <GetNumTilesAcross xmlns="http://tempuri.org/"> \
                                        <zoom>'+ String(iZoom) + '</zoom> \
                                    </GetNumTilesAcross> \
                                </soap:Body> \
                            </soap:Envelope>';
                req.send(sMsg);
            }

            function fnOnComplete() {
                if (req.readyState == 4) {
                    if (req.status == 200) {
                        var ndResult = req.responseXML.documentElement.getElementsByTagName("GetNumTileXRPCAsync");
                        alert(req.responseText);
                    } else {
                        alert("Asynchronous call failed. ResponseText was:\n" + req.responseText)
                    }
                    req = null;
                }
            }
        </script>
    </head>
    <body>
        <form id="frmMain" runat="server" method="get">
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

