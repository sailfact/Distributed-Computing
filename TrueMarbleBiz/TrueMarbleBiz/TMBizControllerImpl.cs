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
            try
            {
                // incease default message size quota
                tcpBinding.MaxReceivedMessageSize = System.Int32.MaxValue;
                tcpBinding.ReaderQuotas.MaxArrayLength = System.Int32.MaxValue;

                // bind channel to url
                channelFactory = new ChannelFactory<ITMDataController>(tcpBinding, url);   // bind url to channel factory

                m_tmData = channelFactory.CreateChannel();  // create true marbledata on remote server
                m_hist = new BrowseHistory();
            }
            catch (ArgumentNullException e1)
            {
                Console.WriteLine("\nError: Binding URL to ChannelFactory\n"+e1.Message);
                Environment.Exit(1);
                
            }
            catch (CommunicationException e2)
            {
                Console.WriteLine("\nError: Communicating with Data Server \n"+e2.Message);
                Environment.Exit(1);
            }
            catch (InvalidOperationException e3)
            {
                Console.WriteLine("\nError: Modifying TcpBinding Message Quota\n"+e3.Message);
                Environment.Exit(1);
            }

        }
        /// <summary>
        /// GetNumTilesAcross
        /// gets the across value for the corresponding zoom level
        /// from the data tier and relays it to the gui
        /// </summary>
        /// <param name="zoom"></param>
        /// <returns>
        /// returns num tiles across or -1 if failure
        /// aswell as a error message via reference
        /// </returns>
        public int GetNumTilesAcross(int zoom)
        {
            try
            {
                return m_tmData.GetNumTilesAcross(zoom);
            }
            catch (FaultException<DataServerFault>)
            {
                throw new FaultException<BizServerFault>(new BizServerFault("TMBizControllerImpl.GetNumTilesDown", "Error In Data Server"));
            }
            catch (CommunicationException e)
            {
                throw new FaultException<BizServerFault>(new BizServerFault("TMBizControllerImpl.GetNumTilesAcross", "Error communicating with data server"));
            }
        }
        /// <summary>
        /// gets the down value for the corresponding zoom level
        /// from the data tier and relays it to the gui
        /// </summary>
        /// <param name="zoom"></param>
        /// <returns>
        /// returns num tiles down or -1 if failure
        /// aswell as a error message via reference
        /// </returns>
        public int GetNumTilesDown(int zoom)
        {
            try
            {
                return m_tmData.GetNumTilesDown(zoom);
            }
            catch (FaultException<DataServerFault>)
            {
                throw new FaultException<BizServerFault>(new BizServerFault("TMBizControllerImpl.GetNumTilesDown", "Error In Data Server"));
            }
            catch (CommunicationException)
            {
                throw new FaultException<BizServerFault>(new BizServerFault("TMBizControllerImpl.GetNumTilesDown", "Error communicating with data server"));
            }
        }      
        /// <summary>
        /// LoadTile
        /// Loads tile from data tier and returns byte array
        /// </summary>
        /// <param name="zoom"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns>
        /// byte array of raw tile jpg
        /// or null if error
        /// </returns>
        public byte[] LoadTile(int zoom, int x, int y)
        {
            try
            {
                return m_tmData.LoadTile(zoom, x, y);
            }
            catch (FaultException<DataServerFault> e1)
            {
                throw new FaultException<BizServerFault>(new BizServerFault("TMBizControllerImpl.LoadTile", "An error occurred in the data tier"));
            }
            catch (CommunicationException e2)
            {
                throw new FaultException<BizServerFault>(new BizServerFault("TMBizControllerImpl.LoadTile", "Error communicating with Data Server"));
            }
        }
        /// <summary>
        /// VerifyTiles
        /// loops through every tile coordinate and check whether they can be decoded
        /// </summary>
        /// <returns>
        /// true if tiles are all valid, false if they 
        /// aren't or there was a problem checking
        /// </returns>
        public bool VerifyTiles()
        {
            bool verified = true;
            MemoryStream memoryStream;
            JpegBitmapDecoder decoder;
            try
            {
                for (int zoom = 0; (zoom <= 6 && verified); zoom++)
                {
                    for (int x = 0; (x < m_tmData.GetNumTilesAcross(zoom) - 1 && verified); x++)
                    {
                        for (int y = 0; (y < m_tmData.GetNumTilesDown(zoom) - 1 && verified); y++)
                        {
                            try
                            {
                                memoryStream = new MemoryStream(m_tmData.LoadTile(zoom, x, y));
                                decoder = new JpegBitmapDecoder(memoryStream, BitmapCreateOptions.None, BitmapCacheOption.None);
                            }
                            catch (FileFormatException)  // if it fails it probably is corrupt
                            {
                                Console.WriteLine("\n\tTile zoom={0}, x={1}, y={2} Corrupted\n\n", zoom, x, y); // print which tile is corrupt
                                verified = false;   // keep iterating throught to see which other tiles are corrupt
                            }
                            catch (ArgumentNullException)  // Loadtiles failed
                            {
                                Console.WriteLine("\nError Loading Tiles from Data Server, in Function'Verify Tiles'\n");
                                return false;   // notify gui that tiles can't be verified
                            }
                        }
                    }
                }
            }
            catch (FaultException<DataServerFault> e) // Data server is offline
            {
                Console.WriteLine("\nError: Communicating With Data Server, in Function 'VerifyTiles'\n" + e.Detail.Message);
                throw new FaultException<BizServerFault>(new BizServerFault("TMBizControllerImpl.VerifyTiles", e.Detail.Message));
            }

            return verified;
        }
        /// <summary>
        /// VerifyTilesAsync
        /// assigns a delegate to the function Verify tiles to be run Asyncronously
        /// </summary>
        public void VerifyTilesAsync()
        {
            VerifyOperation addDel = VerifyTiles;       // point delegate at a function to be called asynchronously
            AsyncCallback callbackDel;

            callbackDel = this.VerifyTiles_OnComplete;  // point callback delegate at call back function
            // pass remote client callback reference
            addDel.BeginInvoke(callbackDel,  OperationContext.Current.GetCallbackChannel<ITMBizControllerCallback>());             

            Console.WriteLine("Waiting for Verification...");
        }
        /// <summary>
        /// VerifyTiles_OnComplete
        /// Callback funtion for Delgate
        /// gets the result from the call and sends it to the Client
        /// </summary>
        /// <param name="res"></param>
        public void VerifyTiles_OnComplete(IAsyncResult res)
        {
            bool iResult = false;       // result of verification
            VerifyOperation del;     // delegate function reference
            ITMBizControllerCallback ob = null;     // remote client object reference
            AsyncResult asyncObj = (AsyncResult)res;    // result from async function

            try
            {
                if (asyncObj.EndInvokeCalled == false)      // if EndInvoke() has not been called
                {
                    del = (VerifyOperation)asyncObj.AsyncDelegate;   // gain access to delegate
                    ob = (ITMBizControllerCallback)asyncObj.AsyncState;     // get remote client obj reference
                    iResult = del.EndInvoke(asyncObj);   // retrieve result 
                }
                asyncObj.AsyncWaitHandle.Close();
                Console.WriteLine("Verification Complete.\n");
                ob.OnVerificationComplete(iResult);     // send result to client
            }
            catch (CommunicationException e)
            {
                Console.WriteLine("\nError: Sending Tile Verification to Client\n"+e.Message);
            }
        }
        /// <summary>
        /// AddHistEntry
        /// add x,y and zoom to Browse History
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
        /// get x, y and zoom from the current history entry
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
        /// sets zoom,x,y to last hist entry
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
        /// sets zoom,x,y to next hist entry
        /// </summary
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
        /// GetHistIdx
        /// </summary>
        /// <returns>
        /// returns the current history index for m_hist
        /// </returns>
        public int GetHistIdx()
        {
            return m_hist.CurEntryIdx;
        }
        /// <summary>
        /// GetFullHistory
        /// returns BrowseHistory object
        /// </summary>
        /// <returns>history list</returns>
        public BrowseHistory GetFullHistory()
        {
            return m_hist;
        }
        /// <summary>
        /// SetFullHistory
        /// sets a new BrowseHistory object and sets curidx to the end
        /// </summary>
        /// <param name="hist"></param>
        public void SetFullHistory(BrowseHistory hist)
        {
            m_hist = hist;
            m_hist.CurEntryIdx = m_hist.History.Count()-1;      // set idx so history is behind it
        }
    }
}