using System;
using System.ServiceModel;
using System.Windows;
using System.Windows.Media.Imaging;
using System.IO;
using TrueMarbleData;

namespace TrueMarbleGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ITMDataController m_tmData;
        private int m_zoom;
        private int m_xValue;
        private int m_yValue;

        public MainWindow()
        {
            InitializeComponent();
            m_zoom = 0;
            m_xValue = 0;
            m_yValue = 0;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ChannelFactory<ITMDataController> channelFactory;

            NetTcpBinding tcpBinding = new NetTcpBinding();
            string url = "net.tcp://localhost:50001/TMData";

            tcpBinding.MaxReceivedMessageSize = System.Int32.MaxValue;
            tcpBinding.ReaderQuotas.MaxArrayLength = System.Int32.MaxValue;
            
            // bind channel to url
            channelFactory = new ChannelFactory<ITMDataController>(tcpBinding, url);    

            m_tmData = channelFactory.CreateChannel();
        }

        private void BtnLoad_Click(object sender, RoutedEventArgs e)
        {
            LoadTile();     //  load stile 
        }

        private void SldZoom_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            m_zoom = (int)sldZoom.Value;    

            if (m_xValue > m_tmData.GetNumTilesAcross(m_zoom)-1)
            {
                m_xValue = m_tmData.GetNumTilesAcross(m_zoom) -1 ;
            }

            if (m_yValue > m_tmData.GetNumTilesDown(m_zoom)-1)
            {
                m_yValue = m_tmData.GetNumTilesDown(m_zoom) -1 ;
            }

            lblX.Content = m_xValue;
            lblY.Content = m_yValue;

            //LoadTile();     // reload the tile
        }

        private void BtnSouth_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (m_yValue == 0)      // if  y is at lower limit 
                {
                    m_yValue = m_tmData.GetNumTilesDown(m_zoom) - 1;       // roll back to end
                }
                else
                {
                    m_yValue -= 1;      // else increment -1
                }
            }
            catch (CommunicationException ce)
            {
                throw new FaultException(ce.Message);
            }
            lblY.Content = m_yValue;

            //LoadTile();     // reload the tile
        }

        private void BtnWest_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (m_xValue == 0)      // if x is at lower limit 
                {
                    m_xValue = m_tmData.GetNumTilesAcross(m_zoom) - 1;       // roll back to start
                }
                else
                {
                    m_xValue -= 1;      // else increment - 1;
                }
            }
            catch (CommunicationException ce)
            {
                throw new FaultException(ce.Message);
            }

            lblX.Content = m_xValue;

            //LoadTile();     // reload the tile
        }

        private void BtnNorth_Click(object sender, RoutedEventArgs e)
        {
            try
            { 
                if (m_yValue == m_tmData.GetNumTilesDown(m_zoom)-1)      // if y is at upper limit 
                {
                    m_yValue = 0;       // roll back to start
                }
                else
                {
                    m_yValue += 1;      // else increment 2
                }
            }   
            catch (CommunicationException ce)
            {
                throw new FaultException(ce.Message);
            }
            lblY.Content = m_yValue;

            //LoadTile();     // reload the tile
        }

        private void BtnEast_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (m_xValue == m_tmData.GetNumTilesAcross(m_zoom)-1)      // if x is at upper limit
                {
                    m_xValue = 0;       // roll back to start
                }
                else
                {
                    m_xValue += 1;         // else increment by 2
                }
            }
            catch (CommunicationException ce)
            {
                throw new FaultException(ce.Message);
            }

            lblX.Content = m_xValue;

            // LoadTile();     // reload the tile
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            // don't know what this is for but will not run without it ??
        }

        private void LoadTile()
        {
            if (m_tmData != null)
            {
                JpegBitmapDecoder decoder;
                MemoryStream memoryStream;

                try
                {
                    memoryStream = new MemoryStream(m_tmData.LoadTile(m_zoom, m_xValue, m_yValue));
                }
                catch (CommunicationException ce)
                {
                    throw new FaultException(ce.Message);
                }

                try
                {
                    decoder = new JpegBitmapDecoder(memoryStream, BitmapCreateOptions.None, BitmapCacheOption.None);
                }
                catch (FileFormatException fe)
                {
                    throw new FaultException(fe.Message);
                }

                imgTile.Source = decoder.Frames[0];
            }
        }
    }
}
 