/*
 * Created by SharpDevelop.
 * User: Al
 * Date: 10/01/2015
 * Time: 5:11 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Xml.Serialization;

namespace FuzzyHipster
{
  [Serializable()]
  [StructLayout(LayoutKind.Sequential, Pack = 1)]
  public class Peer : IEquatable<Peer>
  {
    public Guid Id { get; set; }
    public string Name { get; set; }

    public DateTime NextConnectionAttempt { get; set; }
    public int FailedConnectionAttempts { get; set; }

    public string IPAddress { get; set; }
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

    [XmlIgnore()]
    public Socket Socket {
      get {
        return socket;
      }
      set {
        socket = value;
      }
    }


    // network statistics
    public int PeerCount { get; set; }
    public long Uptime { get; set; }
    public int MaxBlockPacketSize { get; set; }
    public int EstimatedBandwidth { get; set; }
    
    [NonSerialized()]
    long bytesSent;
    public long BytesSent {
      get {
        return bytesSent;
      }
      set {
        bytesSent = value;
      }
    }
    
    [NonSerialized()]
    long bytesReceived;
    public long BytesReceived {
      get {
        return bytesReceived;
      }
      set {
        bytesReceived = value;
      }
    }
    
    // catalog statistics
    public int CatalogRecency { get; set; }
    
    
    public Peer()
    {
      Id = Guid.NewGuid();
      NextConnectionAttempt = DateTime.MinValue;
      FailedConnectionAttempts = 0;
      //MaxBlockPacketSize = RWTorrent.Singleton.Settings.DefaultMaxBlockPacketSize;
    }
    
    public void UpdateFromCopy( Peer peer )
    {
      Id = peer.Id;
      Name = peer.Name;
      PeerCount = peer.PeerCount;
      CatalogRecency = peer.CatalogRecency;
      Uptime = peer.Uptime;
      EstimatedBandwidth = peer.EstimatedBandwidth;
      MaxBlockPacketSize = peer.MaxBlockPacketSize;
      
    }
    
    /// <summary>
    /// Tells the program to attempt to connect to this peer as soon as its ready
    /// </summary>
    public void ResetConnectionAttempts()
    {
      NextConnectionAttempt = DateTime.MinValue;
      FailedConnectionAttempts = 0;
    }
    
    public override string ToString()
    {
      return string.Format("[Peer Socket={0}, Id={1}, Name={2}, PeerCount={3}, CatalogRecency={4}, Uptime={5}, IPAddress={6}, Port={7}]", socket, Id, Name, PeerCount, CatalogRecency, Uptime, IPAddress, Port);
    }
    
    #region IEquatable implementation
    public bool Equals(Peer other)
    {
      return other.Id == Id;
    }
    #endregion
    
  }
}


