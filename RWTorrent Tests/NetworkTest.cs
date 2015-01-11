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
      IPAddress = IPAddress.Parse("127.0.0.1"),
      Port = RWNetwork.RWDefaultPort
    };
    
    Peer localHostPeer2 = new Peer()
    {
      IPAddress = IPAddress.Parse("127.0.0.1"),
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
      network.Connect(localHostPeer2);
      
      var peer = new Peer()
      {
        CatalogRecency = 0,
        Guid = Guid.Empty,
        PeerCount = 0,
        Uptime = 1234,
        Name = "AND BINGO WAS HIS NAMEO",
        IPAddress = IPAddress.Parse("127.0.0.1"),
        Port = RWNetwork.RWDefaultPort
      };
      
      Thread.Sleep(10000);
      
      network.SendPeerList(localHostPeer2, new Peer[]{peer});
      
      Thread.Sleep(20000);
      //network.Disconnect();
      
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
    
    
  }
}
