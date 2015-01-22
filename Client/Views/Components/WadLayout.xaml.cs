using FuzzyHipster.Catalog;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Client
{
    /// <summary>
    /// Interaction logic for WadLayout.xaml
    /// </summary>
    public partial class WadLayout : UserControl 
    {
        public WadLayout()
        {
            InitializeComponent();
        }


        public static readonly DependencyProperty WadsProp = DependencyProperty.Register("Wads", typeof(List<FileWad>), typeof(WadLayout), new PropertyMetadata(default(List<FileWad>), new PropertyChangedCallback(OnCurrentReadingChanged)));

        private static void OnCurrentReadingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //as
        }

        public List<FileWad> Wads
        {
            get { return GetValue(WadsProp) as List<FileWad>; }
            set { SetValue(WadsProp, value); }
        }



    }
}
