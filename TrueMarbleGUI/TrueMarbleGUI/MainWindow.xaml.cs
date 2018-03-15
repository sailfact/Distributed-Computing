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
            m_zoom = 4;
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
            
            channelFactory = new ChannelFactory<ITMDataController>(tcpBinding, url);

            m_tmData = channelFactory.CreateChannel();
        }

        private void BtnLoad_Click(object sender, RoutedEventArgs e)
        {
            LoadTile();
        }

        private void SldZoom_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            m_zoom = (int)sldZoom.Value;
            LoadTile();
        }

        private void BtnSouth_Click(object sender, RoutedEventArgs e)
        {
            if (m_yValue <= 0)
            {
                m_yValue = 0;
            }
            else
            {
                m_yValue--;
            }

            LoadTile();
        }

        private void BtnWest_Click(object sender, RoutedEventArgs e)
        {
            if (m_xValue <= 0)
            {
                m_xValue = 0;
            }
            else
            {
                m_xValue--;
            }

            LoadTile();
        }

        private void BtnNorth_Click(object sender, RoutedEventArgs e)
        {
            m_yValue++;

            LoadTile();
        }

        private void BtnEast_Click(object sender, RoutedEventArgs e)
        {
            m_xValue++;

            LoadTile();
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void LoadTile()
        {
            if (m_tmData != null)
            {
                JpegBitmapDecoder decoder;
                MemoryStream memoryStream;

                memoryStream = new MemoryStream(m_tmData.LoadTile(m_zoom, m_xValue, m_yValue));

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
 