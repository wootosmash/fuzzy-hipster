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
      
      var threadOne = new Thread(new ThreadStart(delegate {
                                                   
                                                   var catalog = Catalog.Catalog.Load(".");
                                                   var torrent = new RWTorrent(catalog);
                                                   torrent.Start();
                                                   
                                                 }));
      threadOne.Start();
      
      var threadTwo = new Thread(new ThreadStart(delegate {
                                                   
                                                   string testdir = @"C:\temp\testcatalog";
                                                   var emptyCatalog = Catalog.Catalog.Load(testdir);
                                                   var testTorrent = new RWTorrent(emptyCatalog);
                                                   testTorrent.Settings.Port ++;
                                                   testTorrent.Start();
                                                 }));
      
      threadTwo.Start();
      
      Console.ReadKey(true);
    }
  }
  
  
}