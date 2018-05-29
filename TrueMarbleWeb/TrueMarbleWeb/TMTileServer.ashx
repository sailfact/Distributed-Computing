<%@ WebHandler Language="C#" Class="TMTileServer" %>

using System;
using System.ServiceModel;
using System.Web;
using TrueMarbleBiz;

public class TMTileServer : IHttpHandler, ITMBizControllerCallback
{
    public void ProcessRequest(HttpContext context)
    {
        DuplexChannelFactory<ITMBizController> channelFactory;

        NetTcpBinding tcpBinding = new NetTcpBinding();
        string url = "net.tcp://localhost:50002/TMBiz";

        tcpBinding.MaxReceivedMessageSize = System.Int32.MaxValue;
        tcpBinding.ReaderQuotas.MaxArrayLength = System.Int32.MaxValue;

        channelFactory = new DuplexChannelFactory<ITMBizController>(new InstanceContext(this), tcpBinding, url);   // bind url to channel factory
        ITMBizController biz = channelFactory.CreateChannel();
        int x = 0, y = 0, zoom = 4;
        byte[] array = null;
        try
        {
            zoom = Convert.ToInt32(context.Request["zoom"]);
            if (zoom > 6)
                zoom = 6;
            else if (zoom < 0)
                zoom = 0;

            x = Convert.ToInt32(context.Request["x"]);
            if (x > biz.GetNumTilesAcross(zoom) - 1)
                x = biz.GetNumTilesAcross(zoom) - 1;
            if (x < 0)
                x = 0;
            y = Convert.ToInt32(context.Request["y"]);
            if (y > biz.GetNumTilesDown(zoom) - 1)
                y = biz.GetNumTilesDown(zoom) - 1;
            if (y < 0)
                y = 0;

            array = biz.LoadTile(zoom, x, y);
            context.Response.Clear();
            context.Response.ContentType = "image/jpeg";
            context.Response.BinaryWrite(array);
            context.Response.End();
        }
        catch (HttpException e)
        {
            string script = "<script>alert('" + e.Message + "');</script>";
        }


    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }

    public void OnVerificationComplete(bool result)
    {

    }
}