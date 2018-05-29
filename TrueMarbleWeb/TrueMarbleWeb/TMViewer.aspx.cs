using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TrueMarbleBiz;

public partial class _Default : Page, ITMBizControllerCallback
{
    private ITMBizController m_biz;
    private int m_zoom;

    protected void Page_Load(object sender, EventArgs e)
    {
        DuplexChannelFactory<ITMBizController> channelFactory;

        NetTcpBinding tcpBinding = new NetTcpBinding();
        string url = "net.tcp://localhost:50002/TMBiz";

        tcpBinding.MaxReceivedMessageSize = System.Int32.MaxValue;
        tcpBinding.ReaderQuotas.MaxArrayLength = System.Int32.MaxValue;

        channelFactory = new DuplexChannelFactory<ITMBizController>(new InstanceContext(this), tcpBinding, url);   // bind url to channel factory
        m_biz = channelFactory.CreateChannel();
    }

    protected void BtnSubmit_ServerClick(object sender, EventArgs e)
    {
        m_zoom = CheckZoom(Convert.ToInt32(txtZoom.Value));

        int zoomPath = (int)(Math.Pow(2.0, (double)(m_zoom + 1)));
        
        int xPath = Convert.ToInt32(txtX.Value);
        int yPath = Convert.ToInt32(txtY.Value);

        LoadTile(zoomPath, xPath, yPath);
    }

    protected void BtnWest_ServerClick(object sender, EventArgs e)
    {
        int zoomPath = (int)(Math.Pow(2.0, (double)(Convert.ToInt32(txtZoom.Value) + 1)));
        int xPath = Convert.ToInt32(txtX.Value) + 1;
        xPath = CheckX(xPath, m_zoom);
        txtX.Value = xPath.ToString();
        int yPath = Convert.ToInt32(txtY.Value);

        LoadTile(zoomPath, xPath, yPath);
    }

    protected void BtnEast_ServerClick(object sender, EventArgs e)
    {
        int zoomPath = (int)(Math.Pow(2.0, (double)(Convert.ToInt32(txtZoom.Value) + 1)));
        int xPath = Convert.ToInt32(txtX.Value) - 1;
        xPath = CheckX(xPath, m_zoom);
        txtX.Value = xPath.ToString();
        int yPath = Convert.ToInt32(txtY.Value);

        LoadTile(zoomPath, xPath, yPath);
    }

    protected void BtnNorth_ServerClick(object sender, EventArgs e)
    {
        int zoomPath = (int)(Math.Pow(2.0, (double)(Convert.ToInt32(txtZoom.Value) + 1)));
        int xPath = Convert.ToInt32(txtX.Value);
        int yPath = Convert.ToInt32(txtY.Value) - 1;
        yPath = CheckY(yPath, m_zoom);
        txtY.Value = yPath.ToString();

        LoadTile(zoomPath, xPath, yPath);
    }

    protected void BtnSouth_ServerClick(object sender, EventArgs e)
    {
        int zoomPath = (int)(Math.Pow(2.0, (double)(Convert.ToInt32(txtZoom.Value) + 1)));
        int xPath = Convert.ToInt32(txtX.Value);
        int yPath = Convert.ToInt32(txtY.Value) + 1;
        yPath = CheckY(yPath, m_zoom);
        txtY.Value = yPath.ToString();

        LoadTile(zoomPath, xPath, yPath);
    }

    private void LoadTile(int zoom, int x, int y)
    {
        /// Tiles / TrueMarble / res###km/tile_###_###.jpg
        imgTile.Src = "TMTileServer.ashx?x=" + x.ToString("0") + "&y=" + y.ToString("0") + "&zoom=" + zoom.ToString("0");
    }

    protected int CheckZoom(int zoom)
    {
        if (zoom > 6)
            return 6;
        else if (zoom < 0)
            return 0;

        return zoom;
    }

    protected int CheckX(int x, int zoom)
    {
        if (x > m_biz.GetNumTilesAcross(zoom) - 1)
            return m_biz.GetNumTilesAcross(zoom) - 1;
        if (x < 0)
            return 0;
        return x;
    }

    protected int CheckY(int y, int zoom)
    {
        if (y > m_biz.GetNumTilesDown(zoom) - 1)
             return m_biz.GetNumTilesDown(zoom) - 1;
        if (y < 0)
             return 0;

        return y;
    }

    public void OnVerificationComplete(bool result)
    {
        throw new NotImplementedException();
    }
}