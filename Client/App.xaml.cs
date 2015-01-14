using RWTorrent.Catalog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using RWTorrent.Catalog;
using System.Threading;

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

            RWTorrentLoaded += App_RWTorrentLoaded;

            MainWindow app = new MainWindow();
            MainWindowModel context = new MainWindowModel();
            app.DataContext = context;
            app.Show();


           Thread th = new Thread(new ThreadStart(delegate
            {
                var catalog = Catalog.Load(".");
                var rwt = new RWTorrent.RWTorrent(catalog);
                

                rwt.Start();

                RWTorrentLoaded(rwt);
            }));

           th.Start();
            
        
        }

        void App_RWTorrentLoaded(object Sender)
        {
            MainWindowModel.ChangeModel(new CatalogViewModel());
        }

    




    }
}
