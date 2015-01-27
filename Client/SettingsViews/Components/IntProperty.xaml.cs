using FuzzyHipster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Client.Components
{
    /// <summary>
    /// Interaction logic for Label_Field_Description.xaml
    /// </summary>
    public partial class IntProperty : UserControl
    {
        public IntProperty()
        {
            InitializeComponent();
       
        }

        public static readonly DependencyProperty LabelDep = DependencyProperty.Register("Label", typeof(string), typeof(IntProperty), new PropertyMetadata(default(string), new PropertyChangedCallback(OnCurrentReadingChanged)));
        public static readonly DependencyProperty FieldDep = DependencyProperty.Register("Field", typeof(Nullable<int>), typeof(IntProperty), new PropertyMetadata(default(Nullable<int>), new PropertyChangedCallback(OnCurrentReadingChanged)));
        public static readonly DependencyProperty DescDep = DependencyProperty.Register("Desc", typeof(string), typeof(IntProperty), new PropertyMetadata(default(string), new PropertyChangedCallback(OnCurrentReadingChanged)));



        private string _label = "";
        public string Label
        {
            get { return _label; }
            set { _label = value; }
        }

        private string _setProperty = "";
        public string SetProperty
        {
            get { return _setProperty; }
            set { _setProperty = value; }
        }


        
        public int TextArea
        {
            get {

                if (String.IsNullOrEmpty(SetProperty))
                {
                    return 0;
                }
                PropertyInfo _setting = MoustacheLayer.Singleton.Settings.GetType().GetProperty(SetProperty);
                int value = (int)_setting.GetValue(MoustacheLayer.Singleton.Settings);
                return value;
            }
            
            set {
                PropertyInfo _setting = MoustacheLayer.Singleton.Settings.GetType().GetProperty(SetProperty);
                _setting.SetValue(MoustacheLayer.Singleton.Settings, value , null);
                MoustacheLayer.Singleton.Settings.Save();
                
            }
        }


        private string _desc = "";
        public string Desc
        {
            get { return _desc; }
            set { _desc = value; }
        }

        
        
        private static void OnCurrentReadingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
           
        }

        private void LabelFieldDescription_Loaded(object sender, RoutedEventArgs e)
        {

            if (String.IsNullOrEmpty(SetProperty))
            {
                return;
            }

            PropertyInfo _setting = MoustacheLayer.Singleton.Settings.GetType().GetProperty(SetProperty);
            int value = (int)_setting.GetValue(MoustacheLayer.Singleton.Settings);
            controlInput.Text = value.ToString();
        }

        
    }
}
