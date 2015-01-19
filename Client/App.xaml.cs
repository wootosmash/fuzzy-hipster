using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

using FuzzyHipster.Catalog;
using FuzzyHipster;
using FuzzyHipster.Network;


namespace Client
{
  /// <summary>
  /// Interaction logic for App.xaml
  /// </summary>
  public partial class App : Application
  {
    public event EventHandler MoustacheLayerLoaded;

    protected virtual void OnMoustacheLayerLoaded(EventArgs e)
    {
      var handler = MoustacheLayerLoaded;
      if (handler != null)
        handler(this, e);
    }

    protected override void OnStartup(StartupEventArgs e)
    {      
      base.OnStartup(e);

      var thread = new Thread(
        new ThreadStart(
          delegate
          {
            int port = RWNetwork.RWDefaultPort;
            var catalog = Catalog.Load(ConfigurationManager.AppSettings["CatalogPath"]);
            var rwt = new FuzzyHipster.MoustacheLayer(catalog);
            if ( !int.TryParse(ConfigurationManager.AppSettings["Port"], out port))
              port = RWNetwork.RWDefaultPort;
            
            rwt.Settings.Port = port;
            OnMoustacheLayerLoaded(new EventArgs());
            rwt.Start();
          }
         )
       );     


      MoustacheLayerLoaded += App_MoustacheLayerLoaded;

      MainWindow app = new MainWindow();
      MainWindowModel context = new MainWindowModel();
      app.DataContext = context;
      app.Show();

      thread.IsBackground = true;
      thread.Start();
    }

    void App_MoustacheLayerLoaded(object sender, EventArgs e)
    {
        Dispatcher.Invoke(new Action(delegate()
        {
            MainWindowModel mw = (MainWindowModel)Application.Current.MainWindow.DataContext;
            mw.Setup();
            //MainWindowModel.ChangeModel(typeof(CatalogViewModel));
            CatalogViewModel.Instance.Render();
        }));
    }  

  }
}
