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
        public delegate void EventHandler(object Sender);
        public event EventHandler RWTorrentLoaded;


        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            //RWTorrentLoaded += App_RWTorrentLoaded;

            MainWindow app = new MainWindow();
            MainWindowModel context = new MainWindowModel();
            app.DataContext = context;
            app.Show();

            TaskOfTResult_MethodAsync();
            
            
        
        }

    

        async Task<RWTorrent.RWTorrent> TaskOfTResult_MethodAsync()
        {
            var catalog = Catalog.Load(".");
            var rwt = new RWTorrent.RWTorrent(catalog);

            //RWTorrentLoaded(null);
            MainWindowModel.ChangeModel(new CatalogViewModel());
            return rwt;
        }



    }
}
