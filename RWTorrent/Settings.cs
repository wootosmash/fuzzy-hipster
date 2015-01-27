/*
 * Created by SharpDevelop.
 * User: Al
 * Date: 10/01/2015
 * Time: 11:53 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.IO;
using System.Xml.Serialization;
using FuzzyHipster.Network;

namespace FuzzyHipster
{
  public class Settings
  {
    public int MaxActivePeers { get; set; }
    
    /// <summary>
    /// Amount of seconds to wait between connect attempts to the same peer. Will be used in exponential backoff.
    /// </summary>
    public int ConnectAttemptWaitTime { get; set; }
    
    /// <summary>
    /// How often to beat the heart
    /// </summary>
    public int HeartbeatInterval {get; set; }
    
    /// <summary>
    /// What port to run the network traffic on
    /// </summary>
    public int Port { get; set; }
    
    /// <summary>
    /// Maximum number of active block transfers going on
    /// </summary>
    public int MaxActiveBlockTransfers { get; set; }
    
    /// <summary>
    /// Maximum block packet size
    /// </summary>
    public int DefaultMaxBlockPacketSize { get; set; }
    
    /// <summary>
    /// How much time to wait before thinking for a peer - gives them an oppourtunity to auth
    /// </summary>
    public int ThinkTimeGraceMilliseconds { get; set; }
    
    /// <summary>
    /// Default number of blocks to try and create when creating a WAD
    /// </summary>
    public int DefaultBlockQuantity { get; set; }
    
    /// <summary>
    /// How long between keep alive messages
    /// </summary>
    public int KeepAliveInterval { get; set; }
    
    /// <summary>
    /// Maximum rate to transmit packets at
    /// </summary>
    public int MaxTransmitRate { get; set; }
    
    /// <summary>
    /// Maximum rate to receive packets at
    /// </summary>
    public int MaxReceiveRate { get; set; }
    
    /// <summary>
    /// Minimum block size to use when creating WADs
    /// </summary>
    public int MinBlockSize { get; set; }
    
    /// <summary>
    /// How big do we want the known peers list to be?
    /// </summary>
    public int DesiredPeerListSize { get; set; }
    
    /// <summary>
    /// What should be the maximum age of the peer list?
    /// </summary>
    public int MaximumAgeOfPeerList { get; set; }
    
    /// <summary>
    /// Default number of seconds before the the catalog item can be advertised again
    /// </summary>
    public int DefaultAdvertisementMoratorium { get; set; }
    
    /// <summary>
    /// Number of milliseconds  between catalog management tasks
    /// </summary>
    public int CatalogThinkInterval { get; set; }

    /// <summary>
    /// Number of items to request on a catalog think
    /// </summary>
    public int CatalogThinkRequestSize { get; set; }
    
    /// <summary>
    /// TTL for relay packets
    /// </summary>
    public int DefaultRelayTimeToLive { get; set; }

    /// <summary>
    /// Use encryption on network comms
    /// </summary>
    public bool UseEncryption { get; set; }
    
    public Settings()
    {
      MaxActivePeers = 1;
      HeartbeatInterval = 10000;
      Port = RWNetwork.RWDefaultPort;
      ConnectAttemptWaitTime = 60;
      MaxActiveBlockTransfers = 1;
      DefaultMaxBlockPacketSize = 40000;
      ThinkTimeGraceMilliseconds = 1000;
      DefaultBlockQuantity = 100;
      KeepAliveInterval = 1000 * 60 * 1; // 1 minute
      MaxTransmitRate = RateLimiter.UnlimitedRate; //RateLimiter.UnlimitedRate;
      MaxReceiveRate = RateLimiter.UnlimitedRate; //RateLimiter.UnlimitedRate;
      MinBlockSize = 65536;
      DesiredPeerListSize = MaxActivePeers * 2; // 2 times maximum active peers
      DefaultAdvertisementMoratorium = 100;
      CatalogThinkInterval = 10000;
      CatalogThinkRequestSize = 10;
      DefaultRelayTimeToLive = 1; // can send it one hop
      UseEncryption = true;
    }
    
    public static Settings Load(string path)
    {
      Settings settings = null;

      if ( !File.Exists(path))
        return new Settings();
      
      var serialiser = new XmlSerializer(typeof(Settings));
      using (var reader = new StreamReader(path))
        settings = (Settings)serialiser.Deserialize(reader);

      return settings;
    }

    public void Save()
    {
      var serialiser = new XmlSerializer(typeof(Settings));
      using (var writer = new StreamWriter(@"settings.xml"))
        serialiser.Serialize(writer, this);
    }
  }
}



