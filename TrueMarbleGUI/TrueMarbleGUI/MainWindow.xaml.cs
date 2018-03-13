using System.ServiceModel;
using System.Windows;
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

        private void BtnLoad_Loaded(object sender, RoutedEventArgs e)
        {
            ChannelFactory<ITMDataController> channelFactory;

            NetTcpBinding tcpBinding = new NetTcpBinding();
            string url = "net.tcp://localhost:50001/TMData";

            channelFactory = new ChannelFactory<ITMDataController>(tcpBinding, url);

            m_tmData = channelFactory.CreateChannel();

            m_tmData.LoadTile(4, 0,0);
        }
    }
}
