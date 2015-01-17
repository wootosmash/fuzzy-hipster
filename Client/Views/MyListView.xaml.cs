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
    /// Interaction logic for MyListView.xaml
    /// </summary>
    public partial class MyListView : UserControl
    {
        public MyListView()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MyListViewModel svm = (MyListViewModel)MainWindowModel.CurrentView();
            svm.Description = "ASdddddddddddddddd";
        }

        private void CreateChannel_Click(object sender, RoutedEventArgs e)
        {
            MainWindowModel.ChangeModel(typeof(NewChannelViewModel));
        }
    }
}
