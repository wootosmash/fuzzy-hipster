/*
 * Created by SharpDevelop.
 * User: Al
 * Date: 10/01/2015
 * Time: 11:53 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Threading;
using FuzzyHipster.Catalog;
using FuzzyHipster.Network;
using FuzzyHipster.Strategy;

namespace FuzzyHipster
{
  class Program
  {
    public static void Main(string[] args)
    {
      var threads = new List<Thread>();
      
      string catalogBasePath = args.Length > 1 ? args[1] : @".\Localhost-{0}\";
      string port = args.Length > 0 ? args[0] : RWNetwork.RWDefaultPort.ToString();
      
      var catalog = Catalog.Catalog.Load(string.Format(catalogBasePath, port));
      var stache = new MoustacheLayer(catalog);
      stache.Settings.Port = int.Parse(port);
      stache.Me.Port = stache.Settings.Port;
      
      threads.Add(new Thread(new ThreadStart(delegate {
                                               stache.Start();
                                             })));
      threads[0].IsBackground = true;
      threads[0].Start();
      
      ConsoleKeyInfo key = Console.ReadKey(true);
      while ( key.Key != ConsoleKey.Q )
      {
        if ( key.Key == ConsoleKey.B )
        {
          var wad = new FileWad();
          wad.Name = "Test WAD-" + DateTime.Now;
          wad.Description = "Built from B";
          wad.BuildFromPath( @"C:\temp\mendozaaaa");
          
          if ( catalog.Channels.Count == 0 )
          {
            var channel = new Channel();
            channel.Name = "Rof Chan";
            channel.Description = "A Rof Chan Channel";
            catalog.AddChannel(channel);
            
            wad.ChannelId = channel.Id;
            catalog.AddFileWad(wad);
          }
          
        }
        
        if ( key.Key == ConsoleKey.D )
        {
          var wad = new FileWad();
          wad.Name = "Test WAD-" + DateTime.Now;
          wad.Description = "Built from D";
          wad.BuildFromPath( @"E:\temp\HIMYM");
          
          if ( catalog.Channels.Count == 0 )
          {
            var channel = new Channel();
            channel.Name = "Rof Chan";
            channel.Description = "A Rof Chan Channel";
            catalog.AddChannel(channel);
          }
          
          wad.ChannelId = catalog.Channels[0].Id;
          catalog.AddFileWad(wad);
        }
        
        if ( key.Key == ConsoleKey.T )
          stache.Think();
        
        if ( key.Key == ConsoleKey.C )
        {
          Console.WriteLine(catalog.ToString());
        }
        
        if ( key.Key == ConsoleKey.S )
        {
          Console.WriteLine("---------------------------------");
        }
        
        if ( key.Key == ConsoleKey.P )
        {
          
          Console.WriteLine("Peers " + stache.Me.Id);
          foreach( Peer p in stache.Network.ActivePeers )
            Console.WriteLine(p);
        }
        
        if ( key.Key == ConsoleKey.O )
        {
          Console.WriteLine("Begin Streaming");
          MoustacheLayer.Singleton.Strategies.Enable(typeof(StreamingBlockAquisitionStrategy));
          (MoustacheLayer.Singleton.Strategies.Find(typeof(StreamingBlockAquisitionStrategy)) as StreamingBlockAquisitionStrategy).Setup(catalog.FileWads.Values[0], catalog.FileWads.Values[0].Files[0]);

        }
        
        key = Console.ReadKey(true);
      }
    }
  }
}