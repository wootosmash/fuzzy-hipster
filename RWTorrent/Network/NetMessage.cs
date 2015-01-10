/*
 * Created by SharpDevelop.
 * User: Al
 * Date: 10/01/2015
 * Time: 5:11 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Runtime.InteropServices;

namespace RWTorrent.Network
{
  [StructLayout(LayoutKind.Sequential, Pack=1)]
  public class NetMessage
  {
    public MessageType Type { get; set; }
    public int Length { get; set; }
    
    public static NetMessage FromBytes( byte[] buffer, Type t)
    {
      byte[] myBuff = new byte[Marshal.SizeOf(t)];
      Array.Copy(buffer, myBuff, myBuff.Length );

      GCHandle handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
      Object temp = Marshal.PtrToStructure(handle.AddrOfPinnedObject(), t);
      handle.Free();
      return (NetMessage)temp;
    }
    
    public byte[] ToBytes()
    {
      var size = Marshal.SizeOf(this);
      // Both managed and unmanaged buffers required.
      var bytes = new byte[size];
      var ptr = Marshal.AllocHGlobal(size);
      // Copy object byte-to-byte to unmanaged memory.
      Marshal.StructureToPtr(this, ptr, false);
      // Copy data from unmanaged memory to managed buffer.
      Marshal.Copy(ptr, bytes, 0, size);
      // Release unmanaged memory.
      Marshal.FreeHGlobal(ptr);
      
      return bytes;
      
    }
  }
  
  [StructLayout(LayoutKind.Sequential, Pack=1)]
  public class RequestPeers : NetMessage
  {
    public int Count { get; set; }
  }
  
  [StructLayout(LayoutKind.Sequential, Pack=1)]
  public class PeerStatusNetMessage : NetMessage
  {
    public Guid Guid { get; set; }
    public string Name { get; set; }
    public int PeerCount { get; set; }
    public int CatalogRecency { get; set; }
    public long Uptime { get; set; }
  }
  
//  public struct PeerListNetMessage : NetMessage
//  {
//    public Peer[] Peers { get; set; }
//    
//    public override int GetMessageSize()
//    {
//      
//    }
//    
//    public override NetMessage FromBytes(byte[] buffer)
//    {
//      byte[] baseBuffer = base.FromBytes(buffer);
//      
//      int size = 0;
//      byte[] newbuffer = new byte[baseBuffer.Length + (Peers.Length * Marshal.SizeOf(typeof(Peer)))];
//      
//      
//      
//      return newbuffer;
//    }
//    
//    public override byte[] ToBytes()
//    {
//      return base.ToBytes();
//    }
//  }

  public enum MessageType
  {
    Unknown = 0,
    Hello = 1,
    Goodbye = 2,
    PeerStatus = 3,
    
    RequestPeers = 10,
    PeerList = 11,
    
    RequestCatalogItems = 20,
    CatalogItem = 21,
    
    RequestBlock = 30,
    Block = 31
  }
}
