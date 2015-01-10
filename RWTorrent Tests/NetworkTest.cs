/*
 * Created by SharpDevelop.
 * User: Al
 * Date: 10/01/2015
 * Time: 5:02 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
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
    [Test]
    public void ConnectTest()
    {
      var client = new RWNetwork();
      client.Connect("localhost", RWNetwork.RWDefaultPort);
//      client.Disconnect();
    }
    
    [Test]
    public void HelloMessageTest()
    {
      var client = new RWNetwork();
      client.Connect("localhost", RWNetwork.RWDefaultPort);
      
      var msg = new NetMessage();
      msg.Type = MessageType.Hello;
      msg.Length = Marshal.SizeOf(msg);
      
      client.Send( client.Sockets[0], msg );
//      client.Disconnect();
      
    }
    
    [Test]
    public void PeerStatusMessageTest()
    {
      var network = new RWNetwork();
      network.Connect("localhost", RWNetwork.RWDefaultPort);
      
      var peer = new Peer()
      {
        CatalogRecency = 1,
        Guid = Guid.NewGuid(),
        PeerCount = 1,
        Uptime = 1234,
        Name = "AND BINGO WAS HIS NAMEO"
      };
      
      Thread.Sleep(10000);
      
      network.SendPeerList(network.Sockets[0], new Peer[]{peer});
      network.RequestPeers(network.Sockets[0], 30);
      
      Thread.Sleep(10000);
      //network.Disconnect();
      
    }
    
    [Test]
    public void WadsMessageTest()
    {
      var network = new RWNetwork();
      
      network.NewStack += delegate(object sender, GenericEventArgs<Stack> e) {
        Console.WriteLine(e.Value);
      };
      
      network.Connect("localhost", RWNetwork.RWDefaultPort);
      
      Thread.Sleep(10000);
      
      foreach( var socket in network.Sockets )
        network.RequestStacks( socket, 0, 10);
      
      Thread.Sleep(10000);
      //network.Disconnect();
      
    }
    
    
  }
}
