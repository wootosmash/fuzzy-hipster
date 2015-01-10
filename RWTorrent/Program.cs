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
      
      
      var catalog = Catalog.Catalog.Load(".");
      var torrent = new RWTorrent(catalog);
      torrent.Start();
      
      Console.Write("Press any key to continue . . . ");
      Console.ReadKey(true);
    }
  }
  
  
}