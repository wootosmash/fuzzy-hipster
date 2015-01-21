/*
 * Created by SharpDevelop.
 * User: Al
 * Date: 10/01/2015
 * Time: 5:11 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Xml.Serialization;
using FuzzyHipster.Crypto;
using FuzzyHipster.Network;

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
    public NetworkSocket Socket {
      get { return socket; }
      set { socket = value; }
    }
    [NonSerialized()]
    NetworkSocket socket;
        
    // network statistics
    public DateTime NextConnectionAttempt { get; set; }
    public int FailedConnectionAttempts { get; set; }
    public int PeerCount { get; set; }
    public long Uptime { get; set; }
    public int MaxBlockPacketSize { get; set; }
    
    /// <summary>
    /// Whats the current estimate of bytes per second this peer can receive at
    /// </summary>
    public int EstimatedRxBandwidth { get; set; }
    
    /// <summary>
    /// Whats the current estimate of bytes per second this peer can send at
    /// </summary>
    public int EstimatedTxBandwidth { get; set; }
    public DateTime LastConnection { get; set; }
    public bool IsLocal { get; set; }
    
    public bool IsHandshaking 
    {
      get
      {
        return (Id == Guid.Empty);
      }
    }
    
    [NonSerialized()]
    RateLimiter rateLimiter;
    [XmlIgnore]
    public RateLimiter RateLimiter {
      get { return rateLimiter; }
      set { rateLimiter = value; }
    }
    
    [NonSerialized()]
    DateTime okToSend;
    [XmlIgnore()]
    public DateTime OkToSendAt {
      get { return okToSend; }
      set { okToSend = value; }
    }
    
    [NonSerialized()]
    long bytesSent;
    public long BytesSent {
      get { return bytesSent; }
      set { bytesSent = value; }
    }
    
    [NonSerialized()]
    long bytesReceived;
    public long BytesReceived {
      get { return bytesReceived; }
      set { bytesReceived = value; }
    }
    
    // catalog statistics
    public int CatalogRecency { get; set; }
    
    // crypto
    [NonSerialized()]
    SymmetricKey symmetricKey;

    public SymmetricKey SymmetricKey {
      get {
        return symmetricKey;
      }
      set {
        symmetricKey = value;
      }
    }
    
    [NonSerialized()]
    AsymmetricKey asymmetricKey;

    public AsymmetricKey AsymmetricKey {
      get {
        return asymmetricKey;
      }
      set {
        asymmetricKey = value;
      }
    }
    
    
    public Peer()
    {
      Id = Guid.NewGuid();
      NextConnectionAttempt = DateTime.MinValue;
      FailedConnectionAttempts = 0;
      OkToSendAt = DateTime.MaxValue;
      
      if ( MoustacheLayer.Singleton != null )
        MaxBlockPacketSize = MoustacheLayer.Singleton.Settings.DefaultMaxBlockPacketSize;
      else
        MaxBlockPacketSize = 40000;
      
      IsLocal = false;
      LastConnection = DateTime.MinValue;
      CatalogRecency = 0;
      
      if ( MoustacheLayer.Singleton != null )
        RateLimiter = new RateLimiter(MoustacheLayer.Singleton.Settings.MaxReceiveRate);
      else
        RateLimiter = new RateLimiter(RateLimiter.UnlimitedRate);
      
      AsymmetricKey = AsymmetricKey.Create();
      MoustacheLayer.Singleton.Catalog.AddKey(AsymmetricKey);
    }

    
    public void UpdateFromCopy( Peer peer )
    {
      Id = peer.Id;
      Name = peer.Name;
      PeerCount = peer.PeerCount;
      CatalogRecency = peer.CatalogRecency;
      Uptime = peer.Uptime;
      EstimatedRxBandwidth = peer.EstimatedRxBandwidth;
      EstimatedTxBandwidth = peer.EstimatedTxBandwidth;
      MaxBlockPacketSize = peer.MaxBlockPacketSize;
      IPAddress = peer.IPAddress;
      Port = peer.Port;
      OkToSendAt = peer.OkToSendAt;
      LastConnection = peer.LastConnection;
      IsLocal = peer.IsLocal;
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
      return string.Format("[Peer Id={0}, Socket={5}, Name={1}, IPAddress={2}, Port={3} RxRate={4}]", Id, Name, IPAddress, Port, RateLimiter.CurrentRate, Socket);
    }

    
    #region IEquatable implementation
    public bool Equals(Peer other)
    {
      return other.Id == Id;
    }
    #endregion
    
  }
}


