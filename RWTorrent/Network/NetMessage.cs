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
    public int Length { get; set; }
    
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
        byte[] buffer = stream.ToArray();
        
        Length = buffer.Length;
        
        stream.Position = 0;
        serializer.Serialize(stream, this);
        
        return stream.ToArray();
      }
    }
    
    public override string ToString()
    {
      return string.Format("[NetMessage Type={0}, Length={1}]", Type, Length);
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
    
  }
  
  [Serializable()]
  [StructLayout(LayoutKind.Sequential, Pack=1)]
  public class StartBlockTransferNetMessage : NetMessage
  {
    public Guid TransferId { get; set; }
    public Guid FileWadId { get; set; }
    public int Block { get; set; }
    public int TotalPackets { get; set; }
    public int BlockSize { get; set; }
    
    public StartBlockTransferNetMessage()
    {
      Type = MessageType.StartBlockTransfer;
    }
    
    public override string ToString()
    {
      return string.Format("[StartBlockTransferNetMessage TransferId={0}, FileWadId={1}, Block={2}, TotalPackets={3}, BlockSize={4}]", TransferId, FileWadId, Block, TotalPackets, BlockSize);
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
  }

  [Serializable()]
  [StructLayout(LayoutKind.Sequential, Pack=1)]
  public class KeyNetMessage : NetMessage
  {
    public Key Key { get; set; }
    
    public KeyNetMessage()
    {
      Type = MessageType.Key;
    }
  }
  
  public enum MessageType
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
    
    StartBlockTransfer = 35,
    BlockTransferPacket = 36,
    
    Key = 50
  }
}
