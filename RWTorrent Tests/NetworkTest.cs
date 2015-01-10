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
  }
}
