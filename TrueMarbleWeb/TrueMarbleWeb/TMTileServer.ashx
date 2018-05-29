<%@ WebHandler Language="C#" Class="TMTileServer" %>

using System;
using System.Web;
using TrueMarbleBiz;

public class TMTileServer : IHttpHandler
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
    string errMsg = "";
    try
    {
        zoom = Convert.ToInt32(context.Request["zoom"]);
        if (zoom > 6 || zoom < 0)
            zoom = 4;
        x = Convert.ToInt32(context.Request["x"]);
        if (x > biz.GetNumTilesAcross(zoom, out errMsg) - 1)
            x = biz.GetNumTilesAcross(zoom, out errMsg) - 1;
        if (x < 0)
            x = 0;
        y = Convert.ToInt32(context.Request["y"]);
        if (y > biz.GetNumTilesDown(zoom, out errMsg) - 1)
            y = biz.GetNumTilesDown(zoom, out errMsg) - 1;
        if (y < 0)
            y = 0;

        array = biz.LoadTile(zoom, x, y, out errMsg);

    }
    catch (HttpException e)
    {
        string script = "<script>alert('" + e.Message + "');</script>";
    }

    context.Response.Clear();
    context.Response.ContentType = "image/jpeg";
    context.Response.BinaryWrite(array);
    context.Response.End();
}

 
public bool IsReusable {
    get {
        return false;
    }
}

}