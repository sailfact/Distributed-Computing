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
        int x = Convert.ToInt32(txtX.Value);
        int y = Convert.ToInt32(txtY.Value);
        int zoom = Convert.ToInt32(txtZoom.Value);

        string path = "Tiles/TrueMarble/res" + zoom.ToString("000") + "km_" + x.ToString("000") + "_" + y.ToString("000") + ".jpg";
        imgTile.Src = path;
    }
}