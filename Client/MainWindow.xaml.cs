using FuzzyHipster.Catalog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {


        ObservableCollection<Channel> _channels;
        UserSettings settings;

        public MainWindow()
        {

            settings = new UserSettings();
            //settings.Keys 

            var catalog = Catalog.Load(@".");
            //_channels = new ObservableCollection<Channel>(catalog.Channels);
            InitializeComponent();
        }

        public ObservableCollection<Channel> channels
        { get { return _channels; } }


        private void OpenChannelOption_Click(object sender, RoutedEventArgs e)
        {
            var path = Utils.GetFolderPath();
        }
       
    }
}
