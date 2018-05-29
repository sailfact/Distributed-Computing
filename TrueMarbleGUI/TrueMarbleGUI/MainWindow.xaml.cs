using System;
using System.ServiceModel;
using System.Windows;
using System.Windows.Media.Imaging;
using System.IO;
using TrueMarbleBiz;
using System.Runtime.Serialization;

namespace TrueMarbleGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// Written by Ross Curley
    ///            19098081
    /// </summary>
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Multiple,
                      UseSynchronizationContext = false)]
    public partial class MainWindow : Window, ITMBizControllerCallback
    {
        private ITMBizController m_biz;
        private int m_zoom;
        private int m_xValue;
        private int m_yValue;

        /// <summary>
        /// MainWindow Constructor
        /// sets default values of member fields
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            m_zoom = 4;
            m_xValue = 0;
            m_yValue = 0;
        }

        /// <summary>
        /// Window_Loaded 
        /// first event fired 
        /// binds server and initialises m_tmData
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DuplexChannelFactory<ITMBizController> channelFactory;

            NetTcpBinding tcpBinding = new NetTcpBinding();
            string url = "net.tcp://localhost:50002/TMBiz";
            try
            {
                // incease default message size quota
                tcpBinding.MaxReceivedMessageSize = System.Int32.MaxValue;
                tcpBinding.ReaderQuotas.MaxArrayLength = System.Int32.MaxValue;
        
                channelFactory = new DuplexChannelFactory<ITMBizController>(new InstanceContext(this), tcpBinding, url);   // bind url to channel factory
                m_biz = channelFactory.CreateChannel();  // create true marblebiz on remote server
                m_biz.VerifyTilesAsync();
            }
            catch (ArgumentNullException)
            {
                MessageBox.Show("Error Connecting to Server, please  try again later\n");
                this.Close();
            }
            catch (InvalidOperationException)
            {
                MessageBox.Show("Error Connecting to Server, please  try again later\n");
                this.Close();
            }
            catch (EndpointNotFoundException)
            {
                MessageBox.Show("Service not avialable at this time, please  try again later\n");
                this.Close();
            }
        }

        /// <summary>
        /// BtnLoad_Click
        /// event handler for when btnLoad is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnLoad_Click(object sender, RoutedEventArgs e)
        {
            LoadTile(true);     //  loads tile 
        }

        /// <summary>
        /// SldZoom_ValueChanged
        /// event handler for when sldZoom is changed
        /// updates m_zoom and changes m_xValue and m_yValye 
        /// to avoid boundary errors
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SldZoom_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (m_biz != null)
            {
                m_zoom = (int)sldZoom.Value;
                try
                {
                    int across = m_biz.GetNumTilesAcross(m_zoom);
                    int down = m_biz.GetNumTilesDown(m_zoom);
                    if (m_xValue > across - 1)  // if the current x value is more than the tiles accross
                    {
                        m_xValue = across - 1;  // change x to max accross
                    }

                    if (m_yValue > down - 1)    // if the current y value is more than the tiles down
                    {
                        m_yValue = down - 1;    // change the y to max down
                    }
                    LoadTile(true);     // reload the tile
                }
                catch (CommunicationException)       // if server died
                {
                    MessageBox.Show("Error Connecting to server, please  try again later\n");
                    this.Close();
                }
            }
        }

        /// <summary>
        /// BtnSouth_Click
        /// event handler for when btnSouth is clicked
        /// increments the y value rolls back to end if it exceeds lower limit 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnSouth_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (m_yValue == 0)      // if  y is at lower limit 
                {
                    m_yValue = m_biz.GetNumTilesDown(m_zoom) - 1;       // roll back to end
                }
                else
                {
                    m_yValue--;      // else increment -1
                }
                LoadTile(true);     // reload the tile
            }
            catch (FaultException<BizServerFault> ex)
            {
                MessageBox.Show(ex.Detail.Operation + "\n\n" + ex.Detail.Message);
            }
            catch (CommunicationException)   // catch if server dies
            {
                MessageBox.Show("Error Connecting to server, please  try again later\n");
                this.Close();
            }
        }

        /// <summary>
        /// BtnWest_Click
        /// event handler for when btnWest is clicked
        /// increments x value rolls back to end if it exceeds lower limit 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnWest_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (m_xValue == 0)      // if x is at lower limit 
                {
                    m_xValue = m_biz.GetNumTilesAcross(m_zoom) - 1;       // roll back to start
                }
                else
                {
                    m_xValue -= 1;      // else increment - 1;
                }
                LoadTile(true);     // reload the tile
            }
            catch (FaultException<BizServerFault> ex)
            {
                MessageBox.Show(ex.Detail.Operation + "\n\n" + ex.Detail.Message);
            }
            catch (CommunicationException)      // catch if server dies
            {
                MessageBox.Show("Error Connecting to server, please try again later\n");
                this.Close();
            }
        }

        /// <summary>
        /// BtnNorth_Click
        /// event handler for when btnNorth is clicked
        /// increments y value rolls back to start if upper limit is exceeded
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnNorth_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (m_yValue == m_biz.GetNumTilesDown(m_zoom) - 1)      // if y is at upper limit 
                {
                    m_yValue = 0;       // roll back to start
                }
                else
                {
                    m_yValue += 1;      // else increment 2
                }
                LoadTile(true);     // reload the tile
            }
            catch (FaultException<BizServerFault> ex)
            {
                MessageBox.Show(ex.Detail.Operation + "\n\n" + ex.Detail.Message);
            }
            catch (CommunicationException)      // catch if server dies
            {
                MessageBox.Show("Error Connecting to server, please try again later\n");
                this.Close();
            }
        }

        /// <summary>
        /// BtnEast_Click
        /// event handler for when btnEast is clicked
        /// increments x value rolls back to start if upper limit is exceeded
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnEast_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (m_xValue == m_biz.GetNumTilesAcross(m_zoom) - 1)
                {
                    m_xValue = 0;       // roll back to start
                }
                else
                {
                    m_xValue += 1;         // else increment by 2
                }
                LoadTile(true);     // reload the tile
            }
            catch (FaultException<BizServerFault> ex)
            {
                MessageBox.Show(ex.Detail.Operation + "\n\n" + ex.Detail.Message);
            }
            catch (CommunicationException)       // catch if server dies
            {
                MessageBox.Show("Error Connecting to server, please  try again later\n");
                this.Close();
            }
        }
        
        /// <summary>
        /// LoadTile
        /// Loads the tile from m_tmData 
        /// is called but event handlers when conditions change
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LoadTile(bool addToHist)
        {
            if (m_biz != null)       // only load if m_tmData is not null
            {
                // used for getting JPG
                JpegBitmapDecoder decoder;
                MemoryStream memoryStream;
                string errorMsg = null;
                try
                {
                    memoryStream = new MemoryStream(m_biz.LoadTile(m_zoom, m_xValue, m_yValue)); // construct memoryStream with byte array for server call
                    decoder = new JpegBitmapDecoder(memoryStream, BitmapCreateOptions.None, BitmapCacheOption.None); // decode jpg
                    imgTile.Source = decoder.Frames[0]; // assign jpg to imgTile.source
                    if (addToHist)
                    {
                        m_biz.AddHistEntry(m_xValue, m_yValue, m_zoom);     // add entry to history
                    }
                }
                catch (FaultException<BizServerFault> e)
                {
                    MessageBox.Show(e.Detail.Operation + "\n\n" + e.Detail.Message);
                }
                catch (ArgumentNullException)
                {
                    MessageBox.Show(errorMsg);
                }
                catch (FileFormatException)  // catch decoder if it fails
                {
                    MessageBox.Show("Error occurrd while decoding tile");
                }
                catch (CommunicationException)   // catch exception if server died for some reason
                {
                    MessageBox.Show("Error Connecting to server, please  try again later\n");
                    this.Close();
                }
            }
        }
        
        /// <summary>
        /// BtnBack_Click 
        /// sets x,y,z from history
        /// and loads last tile
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (m_biz.GetHistIdx() != -1)
                {
                    m_biz.HistBack(out m_xValue, out m_yValue, out m_zoom);
                    LoadTile(false);
                }
            }
            catch (CommunicationException)
            {
                MessageBox.Show("Error Connecting to server, please  try again later\n");
                this.Close();
            }
        }

        /// <summary>
        /// BtnForward_Click 
        /// sets x,y,z from history
        /// and loads next tile
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnForward_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (m_biz.GetHistIdx() != -1)
                {
                    m_biz.HistForward(out m_xValue, out m_yValue, out m_zoom);
                    LoadTile(false);
                }
            }
            catch (CommunicationException)
            {
                MessageBox.Show("Error Connecting to server, please  try again later\n");
                this.Close();
            }
        }

        /// <summary>
        /// Call back function prints whether verification of 
        /// tiles where valid
        /// </summary>
        /// <param name="result"></param>
        public void OnVerificationComplete(bool result)
        {
            this.Dispatcher.Invoke(() => 
            {
                if (result)
                {
                    TxtBlock.Text = "Images Verified";
                }
                else
                {
                    TxtBlock.Text = "Images Not Verified";
                }
            });
        }

        /// <summary>
        /// MenuItem_Click_Save
        /// gets the BrowseHistory object and serializes it
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_Click_Save(object sender, RoutedEventArgs e)
        {
            BrowseHistory browseHistory;
            FileStream fileStream = null;
            DataContractSerializer serializer;

            try
            {
                browseHistory = m_biz.GetFullHistory();
                string userpath = Environment.GetEnvironmentVariable("homepath");
                fileStream = new FileStream("C:"+userpath+"/History.xml", FileMode.Create, FileAccess.Write);
                serializer = new DataContractSerializer(typeof(BrowseHistory));
                serializer.WriteObject(fileStream, browseHistory);
            }
            catch (FileNotFoundException)
            {
                MessageBox.Show("Error While Saving History Occurred\n\nError: File Not Found\n");
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show("Error While Saving History Occurred\n\nError: Unauthorized Access\n");
            }
            catch (IOException)
            {
                MessageBox.Show("Error While Saving History Occurred\n\n");
            }
            catch (SerializationException)
            {
                MessageBox.Show("Error While Saving History Occurred\n\nError: Serialization\n");
            }
            catch (InvalidDataContractException)
            {
                MessageBox.Show("Error While Saving History Occurred\n\nError: Invalid Data Contract\n");
            }
            catch (CommunicationException)
            {
                MessageBox.Show("Error occurred between you and the server please try again later");
                this.Close();
            }
            finally
            {
                if (fileStream != null)
                {
                    fileStream.Close();
                }
            }
        }

        /// <summary>
        /// MenuItem_Click_Load
        /// loads saved BrowseHistory
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_Click_Load(object sender, RoutedEventArgs e)
        {
            BrowseHistory browseHistory = null;
            FileStream fileStream = null;
            DataContractSerializer serializer;
            try
            {
                string userpath = Environment.GetEnvironmentVariable("homepath");   // get users home directory
                fileStream = new FileStream("C:"+userpath+"/History.xml", FileMode.Open, FileAccess.Read);
                serializer = new DataContractSerializer(typeof(BrowseHistory));

                browseHistory = (BrowseHistory)serializer.ReadObject(fileStream);
                m_biz.SetFullHistory(browseHistory);
            }
            catch (FileNotFoundException)
            {
                MessageBox.Show("Error While Loading History : File Not Found\n");
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show("Error While Loading History : Unauthorized Access");
                m_biz.SetFullHistory(new BrowseHistory());  // set a new history object
            }
            catch (IOException)
            {
                MessageBox.Show("Error While Loading History : IO Error");
                m_biz.SetFullHistory(new BrowseHistory());  // set a new history object
            }
            catch (CommunicationException)
            {
                MessageBox.Show("Error occurred between you and the server please try again later");
                this.Close();
            }
            finally
            {
                if (fileStream != null)
                {
                    fileStream.Close();
                }
            }
        }

        /// <summary>
        /// MenuItem_Click
        /// menu item to open history window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItem_Click_Hist(object sender, RoutedEventArgs e)
        {
            try
            {
                DisplayHistory histWind = new DisplayHistory(m_biz.GetFullHistory());
                if (histWind.ShowDialog() == true)
                {
                    if (histWind.HistEntry != null)
                    {
                        m_xValue = histWind.HistEntry.X;
                        m_yValue = histWind.HistEntry.Y;
                        m_zoom = histWind.HistEntry.Zoom;
                        LoadTile(false);
                    }                    
                }
            }
            catch (InvalidOperationException)
            {
                MessageBox.Show("An Error Occurred while loading the history window\n");
            }
            catch (CommunicationException)
            {
                MessageBox.Show("An Error Occurred while loading the history window\nThe Server is Not Communicating, Please Try Again Later");
                this.Close();
            }
        }
    }
}
 