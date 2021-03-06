﻿

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using FuzzyHipster.Catalog;
namespace FuzzyHipster.Network
{
  public class ReceiveStateObject
  {
    public const int BufferSize = 65536 * 32;

    public Peer Peer = null;

    public bool WaitingLengthFrame = true;

    public int ExpectedLength = 4;
    
    public MessageType ExpectedMessage = MessageType.Unknown;

    public MemoryStream Buffer = new MemoryStream(BufferSize);
    
    public bool Handshaking = true;
    
    public void WaitForLength()
    {
      Buffer.Seek(0, SeekOrigin.Begin);
      if ( Peer == null )
        ExpectedLength = 4;
      else if ( Peer.SymmetricKey != null )
        ExpectedLength = 16;
      else 
        ExpectedLength = 4;
      WaitingLengthFrame = true;
    }
    
    public void WaitForData()
    {
      Buffer.Seek(0, SeekOrigin.Begin);
      WaitingLengthFrame = false;
    }
    
    public override string ToString()
    {
      return string.Format("[ReceiveStateObject Peer={0}, WaitingLengthFrame={1}, ExpectedLength={2}, Buffer={3}]", Peer, WaitingLengthFrame, ExpectedLength, Buffer);
    }

  }
}


