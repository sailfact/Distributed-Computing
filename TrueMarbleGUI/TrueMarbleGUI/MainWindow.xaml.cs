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

        // MainWindow Constructor
        // sets default values of member fields
        public MainWindow()
        {
            InitializeComponent();
            m_zoom = 4;
            m_xValue = 0;
            m_yValue = 0;
        }

        // Window_Loaded 
        // first event fired 
        // binds server and initialises m_tmData
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DuplexChannelFactory<ITMBizController> channelFactory;

            NetTcpBinding tcpBinding = new NetTcpBinding();
            string url = "net.tcp://localhost:50002/TMBiz";

            // incease default message size quota
            tcpBinding.MaxReceivedMessageSize = System.Int32.MaxValue;
            tcpBinding.ReaderQuotas.MaxArrayLength = System.Int32.MaxValue;

            // bind channel to url
            try
            {
                channelFactory = new DuplexChannelFactory<ITMBizController>(new InstanceContext(this), tcpBinding, url);   // bind url to channel factory
                m_biz = channelFactory.CreateChannel();  // create true marblebiz on remote server
            }
            catch (ArgumentNullException ne)
            {
                MessageBox.Show("Error Creating Channel Factory : "+ ne.Message);
            }

            m_biz.VerifyTilesAsync();
        }

        // BtnLoad_Click
        // event handler for when btnLoad is clicked
        private void BtnLoad_Click(object sender, RoutedEventArgs e)
        {
            LoadTile(true);     //  loads tile 
        }

        // SldZoom_ValueChanged
        // event handler for when sldZoom is changed
        // updates m_zoom and changes m_xValue and m_yValye 
        // to avoid boundary errors
        private void SldZoom_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (m_biz != null)
            {
                m_zoom = (int)sldZoom.Value;
                try
                {
                    if (m_xValue > m_biz.GetNumTilesAcross(m_zoom) - 1)
                    {
                        m_xValue = m_biz.GetNumTilesAcross(m_zoom) - 1;
                    }

                    if (m_yValue > m_biz.GetNumTilesDown(m_zoom) - 1)
                    {
                        m_yValue = m_biz.GetNumTilesDown(m_zoom) - 1;
                    }
                }
                catch (CommunicationException ce)       // if server died
                {
                    MessageBox.Show("Error" + ce.Message);
                }

                LoadTile(true);     // reload the tile
            }
        }

        // BtnSouth_Click
        // event handler for when btnSouth is clicked
        // increments the y value rolls back to end if it exceeds lower limit 
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
            }
            catch (CommunicationException ce)   // catch if server dies
            {
                MessageBox.Show("Error" + ce.Message);
            }

            LoadTile(true);     // reload the tile
        }

        // BtnWest_Click
        // event handler for when btnWest is clicked
        // increments x value rolls back to end if it exceeds lower limit 
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
            }
            catch (CommunicationException ce)      // catch if server dies
            {
                MessageBox.Show("Error" + ce.Message);
            }

            LoadTile(true);     // reload the tile
        }

        // BtnNorth_Click
        // event handler for when btnNorth is clicked
        // increments y value rolls back to start if upper limit is exceeded
        private void BtnNorth_Click(object sender, RoutedEventArgs e)
        {
            try
            { 
                if (m_yValue == m_biz.GetNumTilesDown(m_zoom)-1)      // if y is at upper limit 
                {
                    m_yValue = 0;       // roll back to start
                }
                else
                {
                    m_yValue += 1;      // else increment 2
                }
            }   
            catch (CommunicationException ce)      // catch if server dies
            {
                MessageBox.Show("Error"+ce.Message);
            }

            LoadTile(true);     // reload the tile
        }

        // BtnEast_Click
        // event handler for when btnEast is clicked
        // increments x value rolls back to start if upper limit is exceeded
        private void BtnEast_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (m_xValue == m_biz.GetNumTilesAcross(m_zoom)-1)      // if x is at upper limit
                {
                    m_xValue = 0;       // roll back to start
                }
                else
                {
                    m_xValue += 1;         // else increment by 2
                }
            }
            catch (CommunicationException ce)       // catch if server dies
            {
                MessageBox.Show("Error" + ce.Message);
            }

            LoadTile(true);     // reload the tile
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            // don't know what this is for but will not run without it ??
        }

        // LoadTile
        // Loads the tile from m_tmData 
        // is called but event handlers when conditions change
        private void LoadTile(bool addToHist)
        {
            if (m_biz != null)       // only load if m_tmData is not null
            {
                // used for getting JPG
                JpegBitmapDecoder decoder;
                MemoryStream memoryStream;

                try
                {
                    memoryStream = new MemoryStream(m_biz.LoadTile(m_zoom, m_xValue, m_yValue)); // construct memoryStream with byte array for server call
                    decoder = new JpegBitmapDecoder(memoryStream, BitmapCreateOptions.None, BitmapCacheOption.None); // decode jpg
                    imgTile.Source = decoder.Frames[0]; // assign jpg to imgTile.source
                    if (addToHist)
                        m_biz.AddHistEntry(m_xValue, m_yValue, m_zoom);     // add entry to history
                }
                catch (FileFormatException fe)  // catch decoder if it fails
                {
                    MessageBox.Show(fe.Message);
                }
                catch (FaultException fe)   // if LoadTiles fails show the error message and continue
                {
                    MessageBox.Show(fe.Message);
                }
                catch (CommunicationException ce)   // catch exception if server died for some reason
                {
                    MessageBox.Show("Error" + ce.Message);
                }
            }
        }
        
        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            m_biz.HistBack(out m_xValue, out m_yValue, out m_zoom);

            LoadTile(false);
        }

        private void btnForward_Click(object sender, RoutedEventArgs e)
        {
            m_biz.HistForward(out m_xValue, out m_yValue, out m_zoom);

            LoadTile(false);
        }

        public void OnVerificationComplete(bool result)
        {
            if (result)
            {
                MessageBox.Show("Images Verified");
            }
            else
            {
                MessageBox.Show("Images Corrupted");
            }
        }

        private void MenuItem_Click_Save(object sender, RoutedEventArgs e)
        {
            BrowseHistory browseHistory = m_biz.GetFullHistory();
            FileStream fileStream;
            DataContractSerializer serializer;

            try
            {
                fileStream = new FileStream("C:/Users/Sailf/History.xml", FileMode.Create, FileAccess.Write);
                serializer = new DataContractSerializer(typeof(BrowseHistory));
                serializer.WriteObject(fileStream, browseHistory);

                fileStream.Close();
            }
            catch (UnauthorizedAccessException ua)
            {
                MessageBox.Show(ua.Message);
            }
            catch (IOException io)
            {
                MessageBox.Show(io.Message);
            }
        }

        private void MenuItem_Click_Load (object sender, RoutedEventArgs e)
        {
            try
            {
                FileStream fileStream = new FileStream("C:/Users/Sailf/History.xml", FileMode.Open, FileAccess.Read);
                DataContractSerializer serializer = new DataContractSerializer(typeof(BrowseHistory));

                BrowseHistory browseHistory = (BrowseHistory)serializer.ReadObject(fileStream);
                fileStream.Close();
                m_biz.SetFullHistory(browseHistory);
            }
            catch (UnauthorizedAccessException ua)
            {
                MessageBox.Show(ua.Message);
            }
            catch (IOException io)
            {
                MessageBox.Show(io.Message);
            }
        }
    }
}
 