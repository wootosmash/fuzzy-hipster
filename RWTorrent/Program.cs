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

namespace FuzzyHipster
{
  class Program
  {
    public static void Main(string[] args)
    {
      var threads = new List<Thread>();
      
      string port = args.Length > 0 ? args[0] : RWNetwork.RWDefaultPort.ToString();
      
      var catalog = Catalog.Catalog.Load(string.Format(@".\Localhost-{0}\", port));
      var torrent = new RWTorrent(catalog);
      torrent.Settings.Port = int.Parse(port);
      torrent.Me.Port = torrent.Settings.Port;
      
      threads.Add(new Thread(new ThreadStart(delegate {
                                               torrent.Start();
                                             })));
      threads[0].Start();
      
      ConsoleKeyInfo key = Console.ReadKey(true);
      while ( key.Key != ConsoleKey.Q )
      {
        key = Console.ReadKey(true);
        
        if ( key.Key == ConsoleKey.T )
          torrent.Think();
        
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
          
          Console.WriteLine("Peers " + torrent.Me.Id);
          foreach( Peer p in torrent.Peers.Values )
            Console.WriteLine(p);
        }
        
      }
    }
  }
}