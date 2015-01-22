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
    /// Interaction logic for MoustacheClient.xaml
    /// </summary>
    public partial class MoustacheClient : Window
    {
        private static MoustacheClient instance;
        

        private SettingsWindow _settings;
        public SettingsWindow Settings
        {
            get { return _settings; }
        }


        private MoustacheClientModel _model;
        public MoustacheClientModel Model
        {
            get { return _model; }
        }




        public static MoustacheClient Instance
        {
            get
            {
                if (instance == null)
                {   
                    instance = new MoustacheClient();
                    
                }
                return instance;
            }
        }

        //ObservableCollection<Channel> _channels;
        //UserSettings settings;

        public MoustacheClient()
        {
            InitializeComponent();

            _model = new MoustacheClientModel();
            DataContext = _model;
            _settings = new SettingsWindow();
            
        }

    }
}
