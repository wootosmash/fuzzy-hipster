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
using FuzzyHipster;
using FuzzyHipster.Catalog;
using FuzzyHipster.Network;
using NUnit.Framework;

namespace FuzzyHipster.Tests
{
  [TestFixture]
  public class NetworkTest
  {
    Peer localHostPeer = new Peer()
    {
      HostAddress = "127.0.0.1",
      Port = RWNetwork.RWDefaultPort, Name = "FAXX!!"
    };
    
    Peer localHostPeer2 = new Peer()
    {
      HostAddress = "127.0.0.1",
      Port = RWNetwork.RWDefaultPort+1, Name = "Al!!"
    };

    
    [Test]
    public void ConnectTest()
    {
      var client = new RWNetwork(localHostPeer2);
      client.Connect(localHostPeer);
    }
    
    [Test]
    public void HelloMessageTest()
    {
      var client = new RWNetwork(localHostPeer2);
      client.Connect(localHostPeer);
      
      var msg = new NetMessage();
      msg.Type = MessageType.Hello;
      
      client.Send( msg, localHostPeer );
    }
    
    [Test]
    public void PeerStatusMessageTest()
    {
      var catalog = Catalog.Catalog.Load(".");
      var stache = new MoustacheLayer(catalog);
      var network = new RWNetwork(localHostPeer2);
      
      network.Connect(localHostPeer);
      
      network.SendPeerList( new Peer[]{localHostPeer2}, localHostPeer);
      
      //network.Disconnect();
      Thread.Sleep(20000);
    }
    
    [Test]
    public void WadsMessageTest()
    {
      var network = new RWNetwork(localHostPeer2);
      
      network.NewChannel += delegate(object sender, GenericEventArgs<Channel> e) {
        Console.WriteLine(e.Value);
      };
      
      network.Connect(localHostPeer);
      Thread.Sleep(10000);
      
      foreach( var peer in network.ActivePeers )
        network.RequestChannels( peer, 0, 10);
      
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
    
    [Test]
    public void CreateRouteTest()
    {
      UPnP.Discover();
      UPnP.ForwardPort(7893, System.Net.Sockets.ProtocolType.Tcp, "MY AWESOMENESS");
    }
  }
}
