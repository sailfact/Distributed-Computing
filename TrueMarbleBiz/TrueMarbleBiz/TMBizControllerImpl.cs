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
                Console.WriteLine("Error: Binding URL to ChannelFactory\n\n"+e1.Message);
                Environment.Exit(1);
                
            }
            catch (CommunicationException e2)
            {
                Console.WriteLine("Error: Communicating with Data Server \n\n"+e2.Message);
                Environment.Exit(1);
            }
            catch (InvalidOperationException e3)
            {
                Console.WriteLine("Error: Modifying TcpBinding Message Quota\n\n"+e3.Message);
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
        /// </returns>
        public int GetNumTilesAcross(int zoom, out string errorMsg)
        {
            try
            {
                int across = m_tmData.GetNumTilesAcross(zoom, out errorMsg);
                if (across == -1)
                {
                    Console.WriteLine(errorMsg);
                }
                return across;
            }
            catch (CommunicationException e)
            {
                errorMsg = "Error: Retreiving NumTilesAcross from Data Server\n\n";
                Console.WriteLine(errorMsg + e.Message);
                return -1;
            }
        }

        /// <summary>
        /// gets the down value for the corresponding zoom level
        /// from the data tier and relays it to the gui
        /// </summary>
        /// <param name="zoom"></param>
        /// <returns>
        /// returns num tiles down or -1 if failure
        /// </returns>
        public int GetNumTilesDown(int zoom, out string errorMsg)
        {
            try
            {
                int down = m_tmData.GetNumTilesDown(zoom, out errorMsg);
                if (down == -1)
                {
                    Console.WriteLine(errorMsg);
                }
                return down;
            }
            catch (CommunicationException e)
            {
                errorMsg = "Error: Retreiving NumTilesAcross from Data Server\n\n";
                Console.WriteLine(errorMsg + e.Message);
                return -1;
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
        public byte[] LoadTile(int zoom, int x, int y, out string errorMsg)
        {
            try
            {
                byte[] array = m_tmData.LoadTile(zoom, x, y, out errorMsg);
                if (array == null)
                {
                    Console.WriteLine(errorMsg);
                }
                return array;
            }
            catch (CommunicationException e)
            {
                errorMsg = "Error: Retreiving NumTilesDown from Data Server\n\n";
                Console.WriteLine(errorMsg + e.Message);
                return null;
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
            int across, down;
            try
            {
                for (int zoom = 0; (zoom <= 6 && verified); zoom++)
                {
                    if ((across = m_tmData.GetNumTilesAcross(zoom, out string errMsg1)) == -1)
                    {
                        Console.WriteLine(errMsg1);
                        return false;
                    }
                    if ((down = m_tmData.GetNumTilesDown(zoom, out string errMsg2)) == -1)
                    {
                        Console.WriteLine(errMsg2);
                        return false;
                    }
                    for (int x = 0; (x < across - 1 && verified); x++)
                    {
                        for (int y = 0; (y < down - 1 && verified); y++)
                        {
                            try
                            {
                                memoryStream = new MemoryStream(m_tmData.LoadTile(zoom, x, y, out string errMsg3));
                                decoder = new JpegBitmapDecoder(memoryStream, BitmapCreateOptions.None, BitmapCacheOption.None);
                            }
                            catch (FileFormatException)  // if it fails it probably is corrupt
                            {
                                Console.WriteLine("\n\tTile zoom={0}, x={1}, y={2} Corrupted\n\n", zoom, x, y); // print which tile is corrupt
                                verified = false;   // keep iterating throught to see which other tiles are corrupt
                            }
                            catch (ArgumentNullException)  // Loadtiles failed
                            {
                                Console.WriteLine("Error Loading Tiles from Data Server, in Function'Verify Tiles'\n");
                                return false;   // notify gui that tiles can't be verified
                            }
                        }
                    }
                }
            }
            catch (CommunicationException e2) // Data server is offline
            {
                Console.WriteLine("Error: Communicating With Data Server, in Function 'VerifyTiles'\n\n" + e2.Message);
                return false;// notify gui that tiles can't be verified
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
            VerifyOperation addDel;     // delegate function reference
            ITMBizControllerCallback ob = null;     // remote client object reference
            AsyncResult asyncObj = (AsyncResult)res;    // result from async function

            try
            {
                if (asyncObj.EndInvokeCalled == false)      // if EndInvoke() has not been called
                {
                    addDel = (VerifyOperation)asyncObj.AsyncDelegate;   // gain access to delegate
                    ob = (ITMBizControllerCallback)asyncObj.AsyncState;     // get remote client obj reference
                    iResult = addDel.EndInvoke(asyncObj);   // retrieve result 
                }

            
                asyncObj.AsyncWaitHandle.Close();
                ob.OnVerificationComplete(iResult);     // send result to client
            }
            catch (CommunicationException e)
            {
                Console.WriteLine("Error: Sending Tile Verification to Client\n\n"+e.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="zoom"></param>
        public void AddHistEntry(int x, int y, int zoom)
        {
            m_hist.AddHistEntry(x, y, zoom);
        }

        /// <summary>
        /// 
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
        /// GetFullHistory
        /// returns BrowseHistory object
        /// </summary>
        /// <returns></returns>
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
