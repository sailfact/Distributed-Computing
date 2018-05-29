using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TrueMarbleBiz;

namespace TrueMarbleGUI
{
    /// <summary>
    /// Interaction logic for DisplayHistory.xaml
    /// </summary>
    public partial class DisplayHistory : Window
    {
        private BrowseHistory m_history;
        private HistEntry m_entry;

        public HistEntry HistEntry { get { return m_entry; } }

        public DisplayHistory(BrowseHistory hist)
        {
            m_history = hist;
            m_entry = null;
            InitializeComponent();
        }

        /// <summary>
        /// BtnClose_Click
        /// closes window sends back false
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;            // future functionality
            this.Close();
        }

        /// <summary>
        /// Window_Loaded
        /// assigns history to items source
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            IvwHistory.ItemsSource = m_history.History;
        }
        /// <summary>
        /// IvwHistory_SelectionChanged
        /// does nothing yet
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void IvwHistory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            m_entry = (HistEntry)e.AddedItems[0];
        }

        /// <summary>
        /// BtnOk_Click
        /// closes window sends back true
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;            // future functionality
            this.Close();
        }
    }
}
