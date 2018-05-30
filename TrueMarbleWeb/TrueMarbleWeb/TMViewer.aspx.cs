using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class _Default : Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        
    }

    protected void BtnSubmit_ServerClick(object sender, EventArgs e)
    {
        int zoom = Convert.ToInt32(txtZoom.Value);
        if (zoom > 6)
        {
            zoom = 6;
            txtZoom.Value = zoom.ToString("0");
        }
        else if (zoom < 0)
        {
            zoom = 0;
            txtZoom.Value = zoom.ToString("0");
        }

        int xPath = Convert.ToInt32(txtX.Value);
        int yPath = Convert.ToInt32(txtY.Value);

        LoadTile(zoom, xPath, yPath);
    }

    protected void BtnWest_ServerClick(object sender, EventArgs e)
    {
        int zoom = Convert.ToInt32(txtZoom.Value);
        int xPath = Convert.ToInt32(txtX.Value) - 1;
        txtX.Value = xPath.ToString();
        int yPath = Convert.ToInt32(txtY.Value);

        LoadTile(zoom, xPath, yPath);
    }

    protected void BtnEast_ServerClick(object sender, EventArgs e)
    {
        int zoom = Convert.ToInt32(txtZoom.Value);
        int xPath = Convert.ToInt32(txtX.Value) + 1;
        txtX.Value = xPath.ToString();
        int yPath = Convert.ToInt32(txtY.Value);

        LoadTile(zoom, xPath, yPath);
    }

    protected void BtnNorth_ServerClick(object sender, EventArgs e)
    {
        int zoom = Convert.ToInt32(txtZoom.Value);
        int xPath = Convert.ToInt32(txtX.Value);
        int yPath = Convert.ToInt32(txtY.Value) + 1;
        txtY.Value = yPath.ToString();

        LoadTile(zoom, xPath, yPath);
    }

    protected void BtnSouth_ServerClick(object sender, EventArgs e)
    {
        int zoom = Convert.ToInt32(txtZoom.Value);
        int xPath = Convert.ToInt32(txtX.Value);
        int yPath = Convert.ToInt32(txtY.Value) - 1;
        txtY.Value = yPath.ToString();

        LoadTile(zoom, xPath, yPath);
    }

    private void LoadTile(int zoom, int x, int y)
    {
        imgTile.Src = "TMTileServer.ashx?zoom=" + zoom.ToString("0") + "&x=" + x.ToString("0") + "&y=" + y.ToString("0");
    }
}