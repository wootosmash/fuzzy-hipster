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
using System.IO;
using System.Net.Sockets;
using System.Threading;
using RWTorrent.Catalog;

namespace RWTorrent
{
  class Program
  {
    public static void Main(string[] args)
    {
      List<RWTorrent> torrents = new List<RWTorrent>();
      List<Thread> threads = new List<Thread>();
      
      for ( int i=0;i<10;i++)
      {
        var catalog = Catalog.Catalog.Load(@".\Localhost" + i + @"\");
        var torrent = new RWTorrent(catalog);
        torrents.Add(torrent);
        torrent.Settings.Port += i;
        torrent.Me.Port = torrent.Settings.Port;
        
        threads.Add(new Thread(new ThreadStart(delegate {
                                                 torrent.Start();
                                               })));
        threads[i].Start();
      }
      
      ConsoleKeyInfo key = Console.ReadKey(true);
      while ( key.Key != ConsoleKey.Q )
      {
        key = Console.ReadKey(true);
        
        if ( key.Key == ConsoleKey.S )
        {
          Console.WriteLine("---------------------------------");
        }
        
        if ( key.Key == ConsoleKey.P )
        {
          foreach( RWTorrent t in torrents )
          {
            Console.WriteLine("Peers " + t.Me.Guid);
            foreach( Peer p in t.Peers.Values )
              Console.WriteLine(p);
          }
        }
        
      }
    }
  }
}