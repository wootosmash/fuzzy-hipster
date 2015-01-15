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
      return string.Format("[PeerListNetMessage Peers={0}]", Peers);
    }
  }
  
  [Serializable()]
  [StructLayout(LayoutKind.Sequential, Pack=1)]
  public class RequestStacksNetMessage : NetMessage
  {
    public long Recency { get; set; }
    public int Count { get; set; }

    public RequestStacksNetMessage()
    {
      Type = MessageType.RequestStacks;
    }
    
    public override string ToString()
    {
      return string.Format("[RequestStacksNetMessage Recency={0}, Count={1}]", Recency, Count);
    }
  }
  
  [Serializable()]
  [StructLayout(LayoutKind.Sequential, Pack=1)]
  public class StacksNetMessage : NetMessage
  {
    public Stack [] Stacks { get; set; }
    
    public StacksNetMessage()
    {
      Type = MessageType.Stacks;
      Stacks = new Stack[0];
    }
    
    public override string ToString()
    {
      if ( Stacks.Length == 0 )
        return "[StacksNetMessage Stacks.Count=0]";
      else
        return string.Format("[StacksNetMessage Stacks={0}]", Stacks);
    }

  }
  
  [Serializable()]
  [StructLayout(LayoutKind.Sequential, Pack=1)]
  public class RequestWadsNetMessage : NetMessage
  {
    public long Recency { get; set; }
    public int Count { get; set; }
    public Guid StackGuid { get; set; }

    public RequestWadsNetMessage()
    {
      Type = MessageType.RequestWads;
    }
    
    public override string ToString()
    {
      return string.Format("[RequestWadsNetMessage Recency={0}, Count={1}, StackGuid={2}]", Recency, Count, StackGuid);
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

  public enum MessageType
  {
    Unknown = 0,
    Hello = 1,
    Goodbye = 2,
    PeerStatus = 3,
    
    RequestPeers = 10,
    StartPeers = 11,
    Peers = 12,
    
    RequestStacks = 18,    
    StartStacks = 19,
    Stacks = 20,
    
    RequestWads = 25,
    StartWads = 26,
    Wads = 27,
    
    RequestBlock = 30,
    Block = 31,
    
    RequestBlocksAvailable = 32,
    BlocksAvailable = 33,
    
    StartBlockTransfer = 35,
    BlockTransferPacket = 36
  }
}
