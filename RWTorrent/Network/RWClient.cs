/*
 * Created by SharpDevelop.
 * User: Al
 * Date: 10/01/2015
 * Time: 11:53 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Xml.Serialization;
using RWTorrent.Catalog;
namespace RWTorrent.Network
{
  public class SendBlockState
  {
    public Block Block { get; set; }
  }
  
  public class RWClient
  {
    public TcpClient Client { get; set; }
    public NetworkStream Stream { get; set; }
    
    public RWClient()
    {
      Client = new TcpClient();
    }
    
    public void Connect( string host, int port )
    {
      Client.Connect(host, port);
      Stream = Client.GetStream();
    }
    
    public void SendBlock( Block block )
    {
      var state = new SendBlockState();
      state.Block = block;
      Stream.BeginWrite(Block.GetBytes(block), 0, (int)block.Length, EndSendBlock, state);
    }
    
    public void Disconnect()
    {
      Client.Close();
    }

    void EndSendBlock( IAsyncResult result )
    {
      Stream.EndWrite(result);
    }
    
  }
  
  
  
  
  
  
  
  
  
  
  
  
  
  
  
  
}

