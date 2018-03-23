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
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple,
                        UseSynchronizationContext = false)]
    class TMBizControllerImpl : ITMBizController
    {
        private ITMDataController m_tmData;
        private delegate bool VerifyOperation();
        
        // Constructor for TMBizController
        // creates channel to Data server
        TMBizControllerImpl()
        {
            DuplexChannelFactory<ITMDataController> channelFactory;

            NetTcpBinding tcpBinding = new NetTcpBinding();
            string url = "net.tcp://localhost:50001/TMData";    

            // incease default message size quota
            tcpBinding.MaxReceivedMessageSize = System.Int32.MaxValue;
            tcpBinding.ReaderQuotas.MaxArrayLength = System.Int32.MaxValue;

            // bind channel to url
            channelFactory = new DuplexChannelFactory<ITMDataController>(tcpBinding, url);   // bind url to channel factory

            m_tmData = channelFactory.CreateChannel();  // create true marbledata on remote server
        }

        public int GetNumTilesAcross(int zoom)
        {
            return m_tmData.GetNumTilesAcross(zoom);
        }

        public int GetNumTilesDown(int zoom)
        {
            return m_tmData.GetNumTilesDown(zoom);
        }
        
        public byte[] LoadTile(int zoom, int x, int y)
        {
            return m_tmData.LoadTile(zoom, x, y);
        }

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
                        catch (FileFormatException f)
                        {
                            verified = false;
                        }
                    }
                }
            }

            return verified;
        }

        public void VerifyTilesAsync()
        {
            VerifyOperation addDel = VerifyTiles;       // point delegate at a function to be called asynchronously
            AsyncCallback callbackDel;

            callbackDel = this.VerifyTiles_OnComplete;  // point callback delegate at call back function
            // pass remote client callback reference
            addDel.BeginInvoke(null,  OperationContext.Current.GetCallbackChannel<ITMBizControllerCallback>());             

            Console.WriteLine("Waiting for Verification");
            Console.ReadLine();         // wait for callback
        }

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

            asyncObj.AsyncWaitHandle.Close();
            ob.OnVerificationComplete(iResult);     // send result to client
        }
    }
}
