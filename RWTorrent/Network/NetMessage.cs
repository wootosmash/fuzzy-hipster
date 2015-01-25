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
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using FuzzyHipster.Catalog;
using FuzzyHipster.Crypto;

namespace FuzzyHipster.Network
{

  
  [Serializable()]
  [StructLayout(LayoutKind.Sequential, Pack=1)]
  public class NetMessage
  {
    public MessageType Type { get; set; }
    
    public static NetMessage FromBytes( byte[] buffer)
    {
      var serializer = new BinaryFormatter();
      NetMessage msg = null;
      
      using (var stream = new MemoryStream(buffer))
        msg = serializer.Deserialize(stream) as NetMessage;
      
      return msg;
    }
    
    public byte[] ToBytes()
    {
      var serializer = new BinaryFormatter();
      using ( var stream = new MemoryStream())
      {
        serializer.Serialize(stream, this);
        return stream.ToArray();
      }
    }
    
    public override string ToString()
    {
      return string.Format("[NetMessage Type={0}]", Type);
    }
  }
  
  [Serializable()]
  [StructLayout(LayoutKind.Sequential, Pack=1)]
  public class RequestPeersNetMessage : NetMessage
  {
    public int Count { get; set; }
    
    public RequestPeersNetMessage()
    {
      Type = MessageType.RequestPeers;
    }
    
    public override string ToString()
    {
      return string.Format("[RequestPeersNetMessage Count={0}]", Count);
    }

  }
  
  
  
  [Serializable()]
  [StructLayout(LayoutKind.Sequential, Pack=1)]
  public class PeerListNetMessage : NetMessage
  {
    public Peer[] Peers { get; set; }
    
    public PeerListNetMessage()
    {
      Type = MessageType.Peers;
    }
    
    public override string ToString()
    {
      return string.Format("[PeerListNetMessage Type={0} Peers={1}]", Type, Peers);
    }
  }
  
  [Serializable()]
  [StructLayout(LayoutKind.Sequential, Pack=1)]
  public class RequestChannelsNetMessage : NetMessage
  {
    public long Recency { get; set; }
    public int Count { get; set; }
    public Guid[] Guids { get; set; }

    public RequestChannelsNetMessage()
    {
      Type = MessageType.RequestChannels;
    }
    
    public override string ToString()
    {
      return string.Format("[RequestChannelsNetMessage Recency={0}, Count={1}]", Recency, Count);
    }
  }
  
  
  
  [Serializable()]
  [StructLayout(LayoutKind.Sequential, Pack=1)]
  public class RequestWadsNetMessage : NetMessage
  {
    public long Recency { get; set; }
    public int Count { get; set; }
    public Guid ChannelGuid { get; set; }

    public RequestWadsNetMessage()
    {
      Type = MessageType.RequestWads;
    }
    
    public override string ToString()
    {
      return string.Format("[RequestWadsNetMessage Recency={0}, Count={1}, ChannelGuid={2}]", Recency, Count, ChannelGuid);
    }
  }
  
  [Serializable()]
  [StructLayout(LayoutKind.Sequential, Pack=1)]
  public class WadsNetMessage : NetMessage
  {
    public FileWad [] Wads { get; set; }
    
    public WadsNetMessage()
    {
      Type = MessageType.Wads;
    }
    
    public override string ToString()
    {
      if ( Wads == null || Wads.Length == 0 )
        return "[WadsNetMessage]";
      else
        return string.Format("[WadsNetMessage Wads={0}]", Wads);
    }

  }
  
  [Serializable()]
  [StructLayout(LayoutKind.Sequential, Pack=1)]
  public class RequestBlockNetMessage : NetMessage
  {
    public Guid FileWadId { get; set; }
    public int Block { get; set; }
    
    public RequestBlockNetMessage()
    {
      Type = MessageType.RequestBlock;
    }
    
    public override string ToString()
    {
      return string.Format("[RequestBlockNetMessage FileWadId={0}, Block={1}]", FileWadId, Block);
    }
  }
  
  [Serializable()]
  [StructLayout(LayoutKind.Sequential, Pack=1)]
  public class StartBlockTransferRequestNetMessage : NetMessage
  {
    public Guid TransferId { get; set; }
    public Guid FileWadId { get; set; }
    public int Block { get; set; }
    public int TotalPackets { get; set; }
    public int BlockSize { get; set; }
    
    public StartBlockTransferRequestNetMessage()
    {
      Type = MessageType.StartBlockTransferRequest;
    }
    
    public override string ToString()
    {
      return string.Format("[StartBlockTransferRequestNetMessage TransferId={0}, FileWadId={1}, Block={2}, TotalPackets={3}, BlockSize={4}]", TransferId, FileWadId, Block, TotalPackets, BlockSize);
    }

  }
  
  [Serializable()]
  [StructLayout(LayoutKind.Sequential, Pack=1)]
  public class StartBlockTransferAcknowledgementNetMessage : NetMessage
  {
    public Guid TransferId { get; set; }
    public Guid FileWadId { get; set; }
    public int Block { get; set; }
    public bool Accept { get; set; }
    
    public StartBlockTransferAcknowledgementNetMessage()
    {
      Type = MessageType.StartBlockTransferAck;
    }
    
    public override string ToString()
    {
      return string.Format("[StartBlockAcknowledgementTransferNetMessage TransferId={0}, FileWadId={1}, Block={2}]", TransferId, FileWadId, Block );
    }

  }  
  
  [Serializable()]
  [StructLayout(LayoutKind.Sequential, Pack=1)]
  public class BlockPacketNetMessage : NetMessage
  {
    public Guid TransferId { get; set; }
    public int DataLength { get; set; }
    public byte[] Data { get; set; }
    
    public BlockPacketNetMessage()
    {
      Type = MessageType.BlockTransferPacket;
    }
    
    public override string ToString()
    {
      return string.Format("[BlockPacketNetMessage TransferId={0}, DataLength={1}, Data=DATA]", TransferId, DataLength);
    }

  }
  
  [Serializable()]
  [StructLayout(LayoutKind.Sequential, Pack=1)]
  public class RequestBlocksAvailableNetMessage : NetMessage
  {
    public Guid FileWadId { get; set; }
    public RequestBlocksAvailableNetMessage()
    {
      Type = MessageType.RequestBlocksAvailable;
    }

    public override string ToString()
    {
      return string.Format("[RequestBlocksAvailableNetMessage FileWadId={0}]", FileWadId);
    }
  }
  
  [Serializable()]
  [StructLayout(LayoutKind.Sequential, Pack=1)]
  public class BlocksAvailableNetMessage : NetMessage
  {
    public Guid FileWadId { get; set; }
    public bool[] BlocksAvailable { get; set; }
    
    public BlocksAvailableNetMessage()
    {
      Type = MessageType.BlocksAvailable;
    }
    
    public override string ToString()
    {
      return string.Format("[BlocksAvailableNetMessage FileWadId={0}, BlocksAvailable={1}]", FileWadId, BlocksAvailable);
    }

  }

  [Serializable()]
  [StructLayout(LayoutKind.Sequential, Pack=1)]
  public class KeyNetMessage : NetMessage
  {
    public Key Key { get; set; }
    
    public KeyNetMessage()
    {
      Type = MessageType.AsymmetricKeyHello;
    }
  }
  
  /// <summary>
  /// Can used to relay messages to other peers
  /// </summary>
  [Serializable()]
  [StructLayout(LayoutKind.Sequential, Pack=1)]
  public class RelayNetMessage : NetMessage
  {
    public Peer To { get; set; }
    public Peer From { get; set; }
    public byte[] Data { get; set; }
    public int TimeToLive { get; set; }
    
    public RelayNetMessage()
    {
      Type = MessageType.Relay;
      TimeToLive = MoustacheLayer.Singleton.Settings.DefaultRelayTimeToLive;
    }
  }
  
  
  public enum MessageType : byte
  {
    Unknown = 0,
    Hello = 1,
    Goodbye = 2,
    PeerStatus = 3,
    
    RequestPeers = 10,
    StartPeers = 11,
    Peers = 12,
    
    RequestChannels = 18,
    StartChannels = 19,
    Channels = 20,
    
    RequestWads = 25,
    StartWads = 26,
    Wads = 27,
    
    RequestBlock = 30,
    Block = 31,
    
    RequestBlocksAvailable = 32,
    BlocksAvailable = 33,
    
    StartBlockTransferRequest = 35,
    StartBlockTransferAck = 36,
    BlockTransferPacket = 37,
    
    AsymmetricKeyHello = 50,
    AsymmetricKeyAck = 51,
    SymmetricKey = 52,
    
    Relay = 100,
    Max = 255
  }
}
