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
      

      var catalog = new Catalog.Catalog();
      catalog.BasePath = Environment.CurrentDirectory;
      catalog.Namespace = "Base Programs";
      catalog.Description = "Test Catalog for Base Programs";
      
      Stack faxes = new Stack() { 
        Id = Guid.NewGuid(),
        Name = "Faxes Shit", 
        Description = "All fax wangs shit", 
        PublicKey = "..." 
      };
      FileWad blazingSaddles = new FileWad() { BlockSize = 1024, Name = "My Program", Description = "A hilarious Program" };
      blazingSaddles.BuildFromPath( @".");
      
      faxes.Wads.Add(blazingSaddles);      

      catalog.Stacks.Add( faxes );
      catalog.Save();
      
//      var serverThread = new Thread(new ThreadStart(delegate{
//                                                      var server = new RWServer();
//                                                      server.StartListening();
//                                                    }));
//      
//      var clientThread = new Thread(new ThreadStart(delegate{
//                                                      var client = new TcpClient();
//                                                      client.Connect("localhost", RWServer.RWServerPort);
//                                                      using (var stream = new StreamWriter(client.GetStream()))
//                                                      {
//                                                        stream.Write("HELLO<EOF>");
//                                                      }
//                                                    }));
//      
//      serverThread.Start();
//      Thread.Sleep(5000);
//      clientThread.Start();
//      
      Console.Write("Press any key to continue . . . ");
      Console.ReadKey(true);
    }
  }
  
  
}