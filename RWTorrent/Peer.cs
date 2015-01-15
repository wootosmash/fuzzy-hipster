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

    // network properties
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
    public DateTime NextConnectionAttempt { get; set; }
    public int FailedConnectionAttempts { get; set; }
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
      return string.Format("[Peer Socket={0}, BytesSent={1}, BytesReceived={2}, Id={3}, Name={4}, IPAddress={5}, Port={6}, NextConnectionAttempt={7}, FailedConnectionAttempts={8}, PeerCount={9}, Uptime={10}, MaxBlockPacketSize={11}, EstimatedBandwidth={12}, CatalogRecency={13}]", socket, bytesSent, bytesReceived, Id, Name, IPAddress, Port, NextConnectionAttempt, FailedConnectionAttempts, PeerCount, Uptime, MaxBlockPacketSize, EstimatedBandwidth, CatalogRecency);
    }

    
    #region IEquatable implementation
    public bool Equals(Peer other)
    {
      return other.Id == Id;
    }
    #endregion
    
  }
}


