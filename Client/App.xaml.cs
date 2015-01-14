using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using FuzzyHipster.Catalog;
using FuzzyHipster.Catalog;

namespace Client
{
  /// <summary>
  /// Interaction logic for App.xaml
  /// </summary>
  public partial class App : Application
  {
    public event EventHandler RWTorrentLoaded;

    protected virtual void OnRWTorrentLoaded(EventArgs e)
    {
      var handler = RWTorrentLoaded;
      if (handler != null)
        handler(this, e);
    }

    protected override void OnStartup(StartupEventArgs e)
    {
      base.OnStartup(e);

      //RWTorrentLoaded += App_RWTorrentLoaded;

      MainWindow app = new MainWindow();
      MainWindowModel context = new MainWindowModel();
      app.DataContext = context;
      app.Show();

      RWTorrentLoaded += delegate {
        
        Dispatcher.BeginInvoke((Action)(() => {
                                          MainWindowModel.ChangeModel(new CatalogViewModel());
                                        }));
      };
      
      //TaskOfTResult_MethodAsync();
      
      var thread = new Thread( new ThreadStart( delegate {
                                                 var catalog = Catalog.Load(".");
                                                 var rwt = new FuzzyHipster.RWTorrent(catalog);
                                                 OnRWTorrentLoaded(new EventArgs());
                                                 rwt.Start();
                                               }));
      thread.Start();


    }

    

    async Task<FuzzyHipster.RWTorrent> TaskOfTResult_MethodAsync()
    {
      var catalog = Catalog.Load(".");
      var rwt = new FuzzyHipster.RWTorrent(catalog);

      //RWTorrentLoaded(null);
      MainWindowModel.ChangeModel(new CatalogViewModel());
      return rwt;
    }



  }
}
