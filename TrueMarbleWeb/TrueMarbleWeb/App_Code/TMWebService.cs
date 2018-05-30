using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using TrueMarbleBiz;

// NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "TMWebService" in code, svc and config file together.
public class TMWebService : ITMWebService, ITMBizControllerCallback
{
    private ITMBizController m_biz;

    public TMWebService()
    {
        DuplexChannelFactory<ITMBizController> channelFactory;

        NetTcpBinding tcpBinding = new NetTcpBinding();
        string url = "net.tcp://localhost:50002/TMBiz";
        tcpBinding.MaxReceivedMessageSize = System.Int32.MaxValue;
        tcpBinding.ReaderQuotas.MaxArrayLength = System.Int32.MaxValue;

        channelFactory = new DuplexChannelFactory<ITMBizController>(new InstanceContext(this), tcpBinding, url);   // bind url to channel factory
        m_biz = channelFactory.CreateChannel();
    }

    public int GetNumTilesAcross(int zoom)
    {
        return m_biz.GetNumTilesAcross(zoom);
    }

    public int GetNumTilesDown(int zoom)
    {
        return m_biz.GetNumTilesDown(zoom);
    }

    public byte[] LoadTile(int zoom, int x, int y)
    {
        return m_biz.LoadTile(zoom, x, y);
    }

    public void OnVerificationComplete(bool result)
    {
        throw new NotImplementedException();
    }
}
