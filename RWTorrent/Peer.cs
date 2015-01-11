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
    
    public string IPAddress { get; set; }
    public int Port { get; set; }
    
    [XmlIgnore()]
    public bool IsConnected { get {
        if ( Socket == null )
          return false;
        return Socket.Connected;
      }
    }
    
    [XmlIgnore()]
    [NonSerialized()]
    Socket socket;

    [XmlIgnore()]
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
    
    public void UpdateFromCopy( Peer peer )
    {
      Guid = peer.Guid;
      Name = peer.Name;
      PeerCount = peer.PeerCount;
      CatalogRecency = peer.CatalogRecency;
      Uptime = peer.Uptime;
    }
    
    public override string ToString()
    {
      return string.Format("[Peer Socket={0}, Guid={1}, Name={2}, PeerCount={3}, CatalogRecency={4}, Uptime={5}, IPAddress={6}, Port={7}]", socket, Guid, Name, PeerCount, CatalogRecency, Uptime, IPAddress, Port);
    }
  }
}


