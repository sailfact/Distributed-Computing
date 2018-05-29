using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class _Default : Page
{
    protected int m_zoomPath;
    protected int m_xPath;
    protected int m_yPath;

    protected void Page_Load(object sender, EventArgs e)
    {

    }

    protected void BtnSubmit_ServerClick(object sender, EventArgs e)
    {
        int zoom = Convert.ToInt32(txtZoom.Value);
        int zoomPath = (int)(Math.Pow(2.0, (double)(zoom + 1)));

        int xPath = Convert.ToInt32(txtX.Value);
        int yPath = Convert.ToInt32(txtY.Value);

        LoadTile(zoomPath, xPath, yPath);
    }

    protected void BtnWest_ServerClick(object sender, EventArgs e)
    {
        int zoomPath = (int)(Math.Pow(2.0, (double)(Convert.ToInt32(txtZoom.Value) + 1)));
        int xPath = Convert.ToInt32(txtX.Value) + 1;
        txtX.Value = xPath.ToString();
        int yPath = Convert.ToInt32(txtY.Value);

        LoadTile(zoomPath, xPath, yPath);
    }

    protected void BtnEast_ServerClick(object sender, EventArgs e)
    {
        int zoomPath = (int)(Math.Pow(2.0, (double)(Convert.ToInt32(txtZoom.Value) + 1)));
        int xPath = Convert.ToInt32(txtX.Value) - 1;
        txtX.Value = xPath.ToString();
        int yPath = Convert.ToInt32(txtY.Value);

        LoadTile(zoomPath, xPath, yPath);
    }

    protected void BtnNorth_ServerClick(object sender, EventArgs e)
    {
        int zoomPath = (int)(Math.Pow(2.0, (double)(Convert.ToInt32(txtZoom.Value) + 1)));
        int xPath = Convert.ToInt32(txtX.Value);
        int yPath = Convert.ToInt32(txtY.Value) - 1;
        txtY.Value = yPath.ToString();

        LoadTile(zoomPath, xPath, yPath);
    }

    protected void BtnSouth_ServerClick(object sender, EventArgs e)
    {
        int zoomPath = (int)(Math.Pow(2.0, (double)(Convert.ToInt32(txtZoom.Value) + 1)));
        int xPath = Convert.ToInt32(txtX.Value);
        int yPath = Convert.ToInt32(txtY.Value) + 1;
        txtY.Value = yPath.ToString();

        LoadTile(zoomPath, xPath, yPath);
    }

    protected void LoadTile(int zoom, int x, int y)
    {
        /// Tiles / TrueMarble / res###km/tile_###_###.jpg
        imgTile.Src = "TMTileServer.ashx?x=" + x.ToString("0") + "&y=" + y.ToString("0") + "&zoom=" + zoom.ToString("0");
    }
}