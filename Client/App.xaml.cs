using RWTorrent.Catalog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using RWTorrent.Catalog;

namespace Client
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            MainWindow app = new MainWindow();
            MainWindowModel context = new MainWindowModel();
            app.DataContext = context;

            var catalog = Catalog.Load(".");
            var torrent = new RWTorrent.RWTorrent(catalog);
            app.Show();
            
        
        }



    }
}
