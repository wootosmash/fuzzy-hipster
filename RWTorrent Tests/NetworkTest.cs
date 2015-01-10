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
using RWTorrent.Network;

namespace RWTorrent.Tests
{
  [TestFixture]
  public class NetworkTest
  {
    [Test]
    public void ConnectTest()
    {
      var client = new RWClient();
      client.Connect("localhost", RWServer.RWServerPort);
      client.Disconnect();
    }
    
    [Test]
    public void HelloMessageTest()
    {
      var client = new RWClient();
      client.Connect("localhost", RWServer.RWServerPort);
      
      var msg = new NetMessage();
      msg.Type = MessageType.Hello;
      msg.Length = Marshal.SizeOf(msg);
      
      client.Send( msg );
      client.Disconnect();
      
    }
    
    [Test]
    public void PeerStatusMessageTest()
    {
      var network = new RWServer();
      network.Connect("localhost", RWServer.RWServerPort);
      
      var peer = new Peer()
      {
        CatalogRecency = 1,
        Guid = Guid.NewGuid(),
        PeerCount = 1,
        Uptime = 1234,
        Name = "AND BINGO WAS HIS NAMEO"
      };
      
      Thread.Sleep(10000);
      
      network.SendMyStatus(network.Sockets[0]);
      network.RequestPeers(network.Sockets[0], 30);
      
      Thread.Sleep(10000);
      //network.Disconnect();
      
    }
    
    [Test]
    public void WadsMessageTest()
    {
      var network = new RWServer();
      network.Connect("localhost", RWServer.RWServerPort);
      
      Thread.Sleep(10000);
      
      foreach( var socket in network.Sockets )
        network.RequestStacks( socket, 0, 10);
      
      Thread.Sleep(10000);
      //network.Disconnect();
      
    }
    
    
  }
}
