using System;
using System.ServiceModel;
using System.Windows;
using System.Windows.Media.Imaging;
using System.IO;
using TrueMarbleBiz;

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
            channelFactory = new DuplexChannelFactory<ITMBizController>(new InstanceContext(this), tcpBinding, url);   // bind url to channel factory

            m_biz = channelFactory.CreateChannel();  // create true marblebiz on remote server

            m_biz.VerifyTilesAsync();
        }

        // BtnLoad_Click
        // event handler for when btnLoad is clicked
        private void BtnLoad_Click(object sender, RoutedEventArgs e)
        {
            LoadTile();     //  loads tile 
        }

        // SldZoom_ValueChanged
        // event handler for when sldZoom is changed
        // updates m_zoom and changes m_xValue and m_yValye 
        // to avoid boundary errors
        private void SldZoom_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            m_zoom = (int)sldZoom.Value;    

            if (m_xValue > m_biz.GetNumTilesAcross(m_zoom)-1)   
            {
                m_xValue = m_biz.GetNumTilesAcross(m_zoom) -1 ;
            }

            if (m_yValue > m_biz.GetNumTilesDown(m_zoom)-1)
            {
                m_yValue = m_biz.GetNumTilesDown(m_zoom) -1 ;
            }

            LoadTile();     // reload the tile
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
                    m_yValue --;      // else increment -1
                }
            }
            catch (CommunicationException ce)   // catch if server dies
            {
                throw new FaultException(ce.Message);
            }

            LoadTile();     // reload the tile
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
                throw new FaultException(ce.Message);
            }

            LoadTile();     // reload the tile
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
                throw new FaultException(ce.Message);
            }

            LoadTile();     // reload the tile
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
                throw new FaultException(ce.Message);
            }

            LoadTile();     // reload the tile
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            // don't know what this is for but will not run without it ??
        }

        // LoadTile
        // Loads the tile from m_tmData 
        // is called but event handlers when conditions change
        private void LoadTile()
        {
            if (m_biz != null)       // only load if m_tmData is not null
            {
                // used for getting JPG
                JpegBitmapDecoder decoder;
                MemoryStream memoryStream;

                try
                {
                    memoryStream = new MemoryStream(m_biz.LoadTile(m_zoom, m_xValue, m_yValue)); // construct memoryStream with byte array for server call
                }
                catch (CommunicationException ce)   // catch exception if server died for some reason
                {
                    throw new FaultException(ce.Message);
                }

                try
                {
                    decoder = new JpegBitmapDecoder(memoryStream, BitmapCreateOptions.None, BitmapCacheOption.None); // decode jpg
                }
                catch (FileFormatException fe)  // catch decoder if it fails
                {
                    throw new FaultException(fe.Message);
                }

                imgTile.Source = decoder.Frames[0]; // assign jpg to imgTile.source
            }
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
    }
}
 