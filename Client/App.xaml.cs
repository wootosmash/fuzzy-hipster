﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

using FuzzyHipster.Catalog;
using FuzzyHipster.Catalog;
using FuzzyHipster;


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
            var catalog = Catalog.Load(".");
            var rwt = new FuzzyHipster.MoustacheLayer(catalog);
            OnMoustacheLayerLoaded(new EventArgs());
            rwt.Start();
          }
         )
       );
      

      thread.IsBackground = true;
      thread.Start();
      Thread.Sleep(10000);



      MainWindow app = new MainWindow();
      MainWindowModel context = new MainWindowModel();
      app.DataContext = context;
      app.Show();

      MainWindowModel.ChangeModel(new CatalogViewModel());



    }

  }
}
