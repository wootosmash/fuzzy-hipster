/*
 * Created by SharpDevelop.
 * User: Al
 * Date: 10/01/2015
 * Time: 5:02 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading;
using NUnit.Framework;
using RWTorrent.Catalog;
using RWTorrent.Network;

namespace RWTorrent.Tests
{
  [TestFixture]
  public class NetworkTest
  {
    Peer localHostPeer = new Peer()
    {
      IPAddress = "127.0.0.1",
      Port = RWNetwork.RWDefaultPort
    };
    
    Peer localHostPeer2 = new Peer()
    {
      IPAddress = "127.0.0.1",
      Port = RWNetwork.RWDefaultPort + 1
    };

    
    [Test]
    public void ConnectTest()
    {
      var client = new RWNetwork();
      client.Connect(localHostPeer);
    }
    
    [Test]
    public void HelloMessageTest()
    {
      var client = new RWNetwork();
      client.Connect(localHostPeer);
      
      var msg = new NetMessage();
      msg.Type = MessageType.Hello;
      msg.Length = Marshal.SizeOf(msg);
      
      client.Send( localHostPeer, msg );
      //      client.Disconnect();
      
    }
    
    [Test]
    public void PeerStatusMessageTest()
    {
      var network = new RWNetwork();
      
      for ( int i=1;i<=10;i++)
      {
        network.Connect(localHostPeer);
        
        var peer = new Peer()
        {
          CatalogRecency = 0,
          Guid = Guid.NewGuid(),
          PeerCount = 0,
          Uptime = 1234,
          Name = "AND BINGO WAS HIS NAMEO",
          IPAddress = "127.0.0.1",
          Port = RWNetwork.RWDefaultPort + i
        };
        
        network.SendPeerList(localHostPeer, new Peer[]{peer});
        
        //network.Disconnect();
      }
      Thread.Sleep(20000);
    }
    
    [Test]
    public void WadsMessageTest()
    {
      var network = new RWNetwork();
      
      network.NewStack += delegate(object sender, GenericEventArgs<Stack> e) {
        Console.WriteLine(e.Value);
      };
      
      network.Connect(localHostPeer);
      Thread.Sleep(10000);
      
      foreach( var peer in network.ActivePeers )
        network.RequestStacks( peer, 0, 10);
      
      Thread.Sleep(10000);
      //network.Disconnect();
      
    }
    
    [Test]
    public void UPnPTest()
    {
      try
      {
        Console.WriteLine(UPnP.Discover());
        Console.WriteLine("You have an UPnP-enabled router and your IP is: "+UPnP.GetExternalIP());
      }
      catch
      {
        Console.WriteLine("You do not have an UPnP-enabled router.");
      }
    }
    
  }
}
