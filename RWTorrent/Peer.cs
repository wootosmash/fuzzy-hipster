/*
 * Created by SharpDevelop.
 * User: Al
 * Date: 10/01/2015
 * Time: 5:11 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Xml.Serialization;
using RWTorrent.Network;

namespace RWTorrent
{
  [Serializable()]
  [StructLayout(LayoutKind.Sequential, Pack = 1)]
  public class Peer
  {
    public Guid Guid { get; set; }
    public string Name { get; set; }
    public int PeerCount { get; set; }
    public int CatalogRecency { get; set; }
    public long Uptime { get; set; }
    public IPAddress IPAddress { get; set; }
    public int Port { get; set; }
    
    public bool IsConnected {
      get {
        if (Socket == null)
          return false;
        return Socket.Connected;
      }
    }
    
    [XmlIgnore()]
    [NonSerialized()]
    Socket socket;
    public Socket Socket {
      get {
        return socket;
      }
      set {
        socket = value;
      }
    }

    public Peer()
    {
      Guid = Guid.NewGuid();
    }
    
    public override string ToString()
    {
      return string.Format("[Peer Guid={0}, Name={1}, PeerCount={2}, CatalogRecency={3}, Uptime={4}, IPAddress={5}, Port={6}]", Guid, Name, PeerCount, CatalogRecency, Uptime, IPAddress, Port);
    }
  }
}


