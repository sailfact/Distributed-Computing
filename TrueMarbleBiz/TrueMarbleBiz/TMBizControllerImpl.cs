using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using TrueMarbleData;

namespace TrueMarbleBiz
{
    /// <summary>
    /// TMBizControllerImpl 
    /// implements TMBizController
    /// 
    /// Biz Tier server for true marble
    /// verifies integrity of the image tiles
    /// 
    /// stores history of GUI client
    /// </summary>
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple,
                        UseSynchronizationContext = false)]
    class TMBizControllerImpl : ITMBizController
    {
        private ITMDataController m_tmData;
        private BrowseHistory m_hist;
        private delegate bool VerifyOperation();
        
        /// <summary>
        /// Constructor for TMBizController
        /// creates channel to Data server
        /// </summary>
        TMBizControllerImpl()
        {
            ChannelFactory<ITMDataController> channelFactory;

            NetTcpBinding tcpBinding = new NetTcpBinding();
            string url = "net.tcp://localhost:50001/TMData";    

            // incease default message size quota
            tcpBinding.MaxReceivedMessageSize = System.Int32.MaxValue;
            tcpBinding.ReaderQuotas.MaxArrayLength = System.Int32.MaxValue;

            // bind channel to url
            channelFactory = new ChannelFactory<ITMDataController>(tcpBinding, url);   // bind url to channel factory
            
            m_tmData = channelFactory.CreateChannel();  // create true marbledata on remote server
            m_hist = new BrowseHistory();
        }

        /// <summary>
        /// GetNumTilesAcross
        /// returns the number of tiles accross for the zoom level
        /// </summary>
        /// <param name="zoom"></param>
        /// <returns></returns>
        public int GetNumTilesAcross(int zoom)
        {
            return m_tmData.GetNumTilesAcross(zoom);
        }

        /// <summary>
        /// GetNumTilesDown
        /// returns the number of tiles down for the zoom level
        /// </summary>
        /// <param name="zoom"></param>
        /// <returns></returns>
        public int GetNumTilesDown(int zoom)
        {
            return m_tmData.GetNumTilesDown(zoom);
        }

        /// <summary>
        /// Loadtile
        /// returns tile for given zoom,x,y
        /// </summary>
        /// <param name="zoom"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public byte[] LoadTile(int zoom, int x, int y)
        {
            return m_tmData.LoadTile(zoom, x, y);
        }

        /// <summary>
        /// VerifyTiles
        /// opens every tile returns false if one is corrupt
        /// </summary>
        /// <returns></returns>
        public bool VerifyTiles()
        {
            MemoryStream memoryStream;
            JpegBitmapDecoder decoder;
            bool verified = true;
            for (int zoom = 0; (zoom <= 6 && verified); zoom++)
            {
                for (int x = 0; (x < m_tmData.GetNumTilesAcross(zoom)-1&&verified); x ++)
                {
                    for ( int y = 0; (y < m_tmData.GetNumTilesDown(zoom)-1&&verified); y ++)
                    {
                        try
                        {
                            memoryStream = new MemoryStream(m_tmData.LoadTile(zoom, x, y));
                            decoder = new JpegBitmapDecoder(memoryStream, BitmapCreateOptions.None, BitmapCacheOption.None);
                        }
                        catch 
                        {
                            // if it fails it probably is corrupt
                            verified = false;
                        }
                    }
                }
            }

            return verified;
        }

        /// <summary>
        /// VerifyTilesAsync
        /// calls VerifyTiles Asyncronously
        /// sets callback function
        /// </summary>
        public void VerifyTilesAsync()
        {
            VerifyOperation addDel = VerifyTiles;       // point delegate at a function to be called asynchronously
            AsyncCallback callbackDel;

            callbackDel = this.VerifyTiles_OnComplete;  // point callback delegate at call back function
            // pass remote client callback reference
            addDel.BeginInvoke(callbackDel,  OperationContext.Current.GetCallbackChannel<ITMBizControllerCallback>());             

            Console.WriteLine("Waiting for Verification");
        }

        /// <summary>
        /// VerifyTiles_OnComplete
        /// gets result for Asyncronous call and sends it to client
        /// </summary>
        /// <param name="res"></param>
        public void VerifyTiles_OnComplete(IAsyncResult res)
        {
            bool iResult = false;       // result of verification
            VerifyOperation addDel;     // delegate function reference
            ITMBizControllerCallback ob = null;     // remote client object reference
            AsyncResult asyncObj = (AsyncResult)res;    // result from async function

            if (asyncObj.EndInvokeCalled == false)      // if EndInvoke() has not been called
            {
                addDel = (VerifyOperation)asyncObj.AsyncDelegate;   // gain access to delegate
                ob = (ITMBizControllerCallback)asyncObj.AsyncState;     // get remote client obj reference
                iResult = addDel.EndInvoke(asyncObj);   // retrieve result 
            }

            try
            {
                asyncObj.AsyncWaitHandle.Close();
                ob.OnVerificationComplete(iResult);     // send result to client
            }
            catch (CommunicationObjectAbortedException ae)
            {
                Console.WriteLine("ERROR : {0}",ae.Message);
            }
        }

        /// <summary>
        /// AddHistEntry
        /// adds a history entry to browse history
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="zoom"></param>
        public void AddHistEntry(int x, int y, int zoom)
        {
            m_hist.AddHistEntry(x, y, zoom);
        }

        /// <summary>
        /// GetCurrHistEntry
        /// returns x,y,zoom from current history
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="zoom"></param>
        public void GetCurrHistEntry(out int x,  out int y, out int zoom)
        {
            HistEntry entry = m_hist.GetCurrHist();
            x = entry.X;
            y = entry.Y;
            zoom = entry.Zoom;
        }

        /// <summary>
        /// HistBack
        /// returns x,y,zoom from previous entry
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="zoom"></param>
        public void HistBack(out int x, out int y, out int zoom)
        {
            HistEntry entry = m_hist.GetHistBack();
            x = entry.X;
            y = entry.Y;
            zoom = entry.Zoom;
        }

        /// <summary>
        /// HistForward
        /// returns x,y,zoom from next entry
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="zoom"></param>
        public void HistForward(out int x, out int y, out int zoom)
        {
            HistEntry entry = m_hist.GetHistForward();
            x = entry.X;
            y = entry.Y;
            zoom = entry.Zoom;
        }

        /// <summary>
        /// GetFullHistory
        /// returns history object
        /// </summary>
        /// <returns></returns>
        public BrowseHistory GetFullHistory()
        {
            return m_hist;
        }

        /// <summary>
        /// SetFullHistory
        /// sets history object
        /// </summary>
        /// <param name="hist"></param>
        public void SetFullHistory(BrowseHistory hist)
        {
            m_hist = hist;
        }
    }
}
