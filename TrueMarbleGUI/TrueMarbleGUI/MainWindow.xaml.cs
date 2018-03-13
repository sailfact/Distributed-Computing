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

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ChannelFactory<ITMDataController> channelFactory;
            JpegBitmapDecoder decoder;
            Stream memoryStream;

            NetTcpBinding tcpBinding = new NetTcpBinding();
            string url = "net.tcp://localhost:50001/TMData";

            try
            {
                channelFactory = new ChannelFactory<ITMDataController>(tcpBinding, url);

                m_tmData = channelFactory.CreateChannel();

                memoryStream = new MemoryStream(m_tmData.LoadTile(4, 0, 0));
            }
            catch (ArgumentNullException ae)
            {
                throw new FaultException(ae.Message);
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

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}
 