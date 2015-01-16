

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
  public class NewConnectionEventArgs : EventArgs
  {
    public Socket Socket = null;
    public bool Accept = true;
    
    public NewConnectionEventArgs(Socket socket)
    {
      Socket = socket;
    }
  }
  
  public class RWNetwork
  {
    public const int RWDefaultPort = 7892;
    
    public List<Peer> ActivePeers { get; set; }
    public IPEndPoint LocalEndPoint { get; set; }
    public Socket Listener { get; set; }
    public Peer Me { get; set; }
    public SortedList<Guid, TransferManager> InProgressTransfers { get; set; }
    
    public long BytesReceived { get; set; }
    public long BytesSent { get; set; }
    
    Random random = new Random(DateTime.Now.Millisecond);
    ManualResetEvent allDone = new ManualResetEvent(false);
    
    #region events

    public event EventHandler<NewConnectionEventArgs> NewConnection;
    protected virtual void OnNewConnection(NewConnectionEventArgs e)
    {
      var handler = NewConnection;
      if (handler != null)
        handler(this, e);
    }
    
    /// <summary>
    /// Fires when a peer disconnects
    /// </summary>
    public event EventHandler<GenericEventArgs<Peer>> PeerDisconnected;
    protected virtual void OnPeerDisconnected(GenericEventArgs<Peer> e)
    {
      var handler = PeerDisconnected;
      if (handler != null)
        handler(this, e);
    }
    
    /// <summary>
    /// Fires when a peer connects
    /// </summary>
    public event EventHandler<GenericEventArgs<Peer>> PeerConnected;
    protected virtual void OnPeerConnected(GenericEventArgs<Peer> e)
    {
      var handler = PeerConnected;
      if (handler != null)
        handler(this, e);
    }
    
    /// <summary>
    /// Fire when we fail to connect to a peer
    /// </summary>
    public event EventHandler<GenericEventArgs<Peer>> PeerConnectFailed;
    protected virtual void OnPeerConnectFailed(GenericEventArgs<Peer> e)
    {
      var handler = PeerConnectFailed;
      if (handler != null)
        handler(this, e);
    }
    
    /// <summary>
    /// Fires when there's a new stack
    /// </summary>
    public event EventHandler<GenericEventArgs<Stack>> NewStack;
    protected virtual void OnNewStack(GenericEventArgs<Stack> e)
    {
      var handler = NewStack;
      if (handler != null)
        handler(this, e);
    }

    /// <summary>
    /// Fires when there's a new WAD
    /// </summary>
    public event EventHandler<GenericEventArgs<FileWad>> NewWad;
    protected virtual void OnNewWad(GenericEventArgs<FileWad> e)
    {
      var handler = NewWad;
      if (handler != null)
        handler(this, e);
    }

    /// <summary>
    /// Fires when we hear about a new Peer
    /// </summary>
    public event EventHandler<GenericEventArgs<Peer>> NewPeer;
    protected virtual void OnNewPeer(GenericEventArgs<Peer> e)
    {
      var handler = NewPeer;
      if (handler != null)
        handler(this, e);
    }

    /// <summary>
    /// Fires when a request for peers is received
    /// </summary>
    public event EventHandler<MessageComposite<RequestPeersNetMessage>> PeersRequested;
    protected virtual void OnPeersRequested(MessageComposite<RequestPeersNetMessage> e)
    {
      var handler = PeersRequested;
      if (handler != null)
        handler(this, e);
    }

    /// <summary>
    /// Fires when we received a request for stacks
    /// </summary>
    public event EventHandler<MessageComposite<RequestStacksNetMessage>> StacksRequested;

    protected virtual void OnStacksRequested(MessageComposite<RequestStacksNetMessage> e)
    {
      var handler = StacksRequested;
      if (handler != null)
        handler(this, e);
    }

    /// <summary>
    /// Fires when we receive a request for Wads
    /// </summary>
    public event EventHandler<MessageComposite<RequestWadsNetMessage>> WadsRequested;

    protected virtual void OnWadsRequested(MessageComposite<RequestWadsNetMessage> e)
    {
      var handler = WadsRequested;
      if (handler != null)
        handler(this, e);
    }
    
    /// <summary>
    /// Fires when we receive a request for the blocks we have available
    /// </summary>
    public event EventHandler<MessageComposite<RequestBlocksAvailableNetMessage>> BlocksAvailableRequested;

    protected virtual void OnBlocksAvailableRequested(MessageComposite<RequestBlocksAvailableNetMessage> e)
    {
      var handler = BlocksAvailableRequested;
      if (handler != null)
        handler(this, e);
    }

    /// <summary>
    /// Fires when we receive a list of blocks available for a peer
    /// </summary>
    public event EventHandler<MessageComposite<BlocksAvailableNetMessage>> BlocksAvailableReceived;

    protected virtual void OnBlocksAvailableReceived(MessageComposite<BlocksAvailableNetMessage> e)
    {
      var handler = BlocksAvailableReceived;
      if (handler != null)
        handler(this, e);
    }
    
    /// <summary>
    /// Fires when we receive a request to transfer a block
    /// </summary>
    public event EventHandler<BlockRequestedEventArgs> BlockRequested;

    protected virtual void OnBlockRequested(BlockRequestedEventArgs e)
    {
      var handler = BlockRequested;
      if (handler != null)
        handler(this, e);
    }
    
    /// <summary>
    /// Fires when we start receiving a block
    /// </summary>
    public event EventHandler<BlockTransferStartedEventArgs> BlockTransferStarted;

    protected virtual void OnBlockTransferStarted(BlockTransferStartedEventArgs e)
    {
      var handler = BlockTransferStarted;
      if (handler != null)
        handler(this, e);
    }

    /// <summary>
    /// Fires when a block has completing transfering to us
    /// </summary>
    public event EventHandler<BlockReceivedEventArgs> BlockReceived;

    protected virtual void OnBlockReceived(BlockReceivedEventArgs e)
    {
      var handler = BlockReceived;
      if (handler != null)
        handler(this, e);
    }
    
    /// <summary>
    /// Fires when we receive a raw net message -- usually dont need to use this
    /// </summary>
    public event EventHandler<GenericEventArgs<NetMessage>> NetMessageReceived;

    protected virtual void OnNetMessageReceived(GenericEventArgs<NetMessage> e)
    {
      var handler = NetMessageReceived;
      if (handler != null)
        handler(this, e);
    }

    #endregion
    
    public RWNetwork(Peer me)
    {
      Me = me;
      ActivePeers = new List<Peer>();
      InProgressTransfers = new SortedList<Guid, TransferManager>();
    }

    /// <summary>
    /// Starts the Network listening for incoming connections
    /// </summary>
    /// <param name="port"></param>
    public void StartListening( int port )
    {
      BytesReceived = 0;
      BytesSent = 0;
      
      // Data buffer for incoming data.
      byte[] bytes = new Byte[1024];

      LocalEndPoint = new IPEndPoint(IPAddress.Any, port);
      Listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp );

      // Bind the socket to the local endpoint and listen for incoming connections.
      try {
        Listener.Bind(LocalEndPoint);
        Listener.Listen(100);

        while (true)
        {
          // Set the event to nonsignaled state.
          allDone.Reset();

          Listener.BeginAccept(
            new AsyncCallback(AcceptCallback),
            Listener );

          // Wait until a connection is made before continuing.
          allDone.WaitOne();
        }

      } catch (Exception e) {
        Console.WriteLine(e.ToString());
      }
    }

    void AcceptCallback(IAsyncResult ar)
    {
      // Signal the main thread to continue.
      allDone.Set();

      // Get the socket that handles the client request.
      Socket listener = ar.AsyncState as Socket;
      Socket handler = listener.EndAccept(ar);
      
      var args = new NewConnectionEventArgs( handler );
      OnNewConnection( args);
      
      if ( !args.Accept ) // have the oppourtunity to not accept the connection
      {
        handler.Disconnect(false);
        handler.Dispose();
      }
      else
      {        
        var peer = new Peer()
        {
          Id = Guid.Empty,
          Socket = handler,
          IPAddress = (handler.RemoteEndPoint as IPEndPoint).Address.ToString(),
          Port = 0,
          CatalogRecency = 0,
          PeerCount = 0,
          Name = "",
          Uptime = 0
        };
        
        // Create the state object.
        var state = new ReceiveStateObject();
        state.Peer = peer;
        state.ExpectedLength = sizeof(int);
        state.WaitingLengthFrame = true;
        state.ExpectedMessage = MessageType.PeerStatus;
        try
        {
          handler.BeginReceive( state.Buffer.GetBuffer(), 0, state.ExpectedLength, 0,
                               new AsyncCallback(WaitMessageCallback), state); // get the message size first
          
          SendMyStatus(state.Peer, Me);
        }
        catch( Exception ex )
        {
          Console.WriteLine(ex);
        }
      }
    }

    void WaitMessageCallback(IAsyncResult ar)
    {
      var state =  ar.AsyncState as ReceiveStateObject;
      Peer peer = state.Peer;
      
      try
      {

        int bytesRead = peer.Socket.EndReceive(ar);
        peer.BytesReceived += bytesRead;
        BytesReceived += bytesRead;
        
        if ( bytesRead == 0 )
          Disconnect(peer, "Connection closed");
        else if ( bytesRead > 0 && bytesRead < state.ExpectedLength ) // partial receive
        {
          // buffering
          state.Buffer.Seek(bytesRead, SeekOrigin.Current);
          state.ExpectedLength -= bytesRead;
          peer.Socket.BeginReceive( state.Buffer.GetBuffer(), (int)state.Buffer.Position, state.ExpectedLength, 0,
                                   new AsyncCallback(WaitMessageCallback), state);
        }
        else if ( bytesRead == state.ExpectedLength ) // receive finished
        {
          state.Buffer.Seek(0, SeekOrigin.Begin);
          
          if ( state.WaitingLengthFrame )
          {
            state.ExpectedLength = BitConverter.ToInt32(state.Buffer.GetBuffer(), 0);
            state.WaitingLengthFrame = false;
            if ( state.ExpectedLength < state.Buffer.Capacity )
            {
              peer.Socket.BeginReceive( state.Buffer.GetBuffer(), 0, state.ExpectedLength, 0,
                                       new AsyncCallback(WaitMessageCallback), state);
            }
          }
          else
          {
            NetMessage message = NetMessage.FromBytes(state.Buffer.GetBuffer());
            
            if ( state.ExpectedMessage != MessageType.Unknown && message.Type != state.ExpectedMessage) // crappy connection
              Disconnect(state.Peer, string.Format("Didn't receive the message we expected {0}. Received {1}", state.ExpectedMessage, message.Type));
            else
            {
              ProcessMessage(message, state);
              OnNetMessageReceived(new GenericEventArgs<NetMessage>(message));
              
              state.ExpectedMessage = MessageType.Unknown;
              state.ExpectedLength = sizeof(int);
              state.WaitingLengthFrame = true; // waiting for the message size
              peer.Socket.BeginReceive( state.Buffer.GetBuffer(), 0, state.ExpectedLength, 0,
                                       new AsyncCallback(WaitMessageCallback), state); // read the message size first
            }
          }
        }
        else // outside the expected?
        {
          throw new Exception(string.Format("Unexpected length received read={0} expected={1}", bytesRead, state.ExpectedLength));
        }

      }
      catch( Exception ex )
      {
        Disconnect(peer, "Exception in WaitMessageCallback");
        Console.WriteLine(ex);
      }
    }
    
    void ProcessMessage(NetMessage msg, ReceiveStateObject state)
    {
      Log("RECV: {0}", msg);
      
      switch( msg.Type )
      {
        case MessageType.Hello:
          break;
          
        case MessageType.RequestPeers:
          var request = msg as RequestPeersNetMessage;
          OnPeersRequested(new MessageComposite<RequestPeersNetMessage>(state.Peer, request));
          break;
          
        case MessageType.RequestStacks:
          var stacks = msg as RequestStacksNetMessage;
          OnStacksRequested(new MessageComposite<RequestStacksNetMessage>(state.Peer, stacks));
          break;
          
        case MessageType.RequestWads:
          var wads = msg as RequestWadsNetMessage;
          OnWadsRequested(new MessageComposite<RequestWadsNetMessage>(state.Peer, wads));
          break;
          
        case MessageType.PeerStatus:
          var status = msg as PeerListNetMessage;
          
          state.Peer.CatalogRecency = status.Peers[0].CatalogRecency;
          state.Peer.Id = status.Peers[0].Id;
          state.Peer.Name = status.Peers[0].Name;
          state.Peer.PeerCount = status.Peers[0].PeerCount;
          state.Peer.Uptime = status.Peers[0].Uptime;
          state.Peer.IPAddress = (state.Peer.Socket.RemoteEndPoint as IPEndPoint).Address.ToString();
          state.Peer.Port = status.Peers[0].Port;
          state.Peer.OkToSend = true;
          
          if ( !ActivePeers.Contains(state.Peer))
            ActivePeers.Add(state.Peer);
          
          OnNewPeer(new GenericEventArgs<Peer>(state.Peer));
          
          break;
          
        case MessageType.Peers:
          var peerList = msg as PeerListNetMessage;
          
          foreach( var peer in peerList.Peers )
            if ( !String.IsNullOrWhiteSpace(peer.IPAddress))
              OnNewPeer(new GenericEventArgs<Peer>(peer));
          break;
          
        case MessageType.Stacks:
          var stacksList = msg as StacksNetMessage;
          foreach( var stack in stacksList.Stacks )
            OnNewStack( new GenericEventArgs<Stack>(stack));
          break;
          
        case MessageType.Wads:
          var wadsList = msg as WadsNetMessage;
          foreach( var wad in wadsList.Wads )
            OnNewWad( new GenericEventArgs<FileWad>(wad));
          break;
          
        case MessageType.RequestBlocksAvailable:
          var requestBlockAvailability = msg as RequestBlocksAvailableNetMessage;
          OnBlocksAvailableRequested(new MessageComposite<RequestBlocksAvailableNetMessage>(state.Peer, requestBlockAvailability));
          break;
          
        case MessageType.BlocksAvailable:
          var blockAvailablity = msg as BlocksAvailableNetMessage;
          OnBlocksAvailableReceived( new MessageComposite<BlocksAvailableNetMessage>(state.Peer, blockAvailablity));
          break;
          
        case MessageType.RequestBlock:
          var requestBlock = msg as RequestBlockNetMessage;
          OnBlockRequested( new BlockRequestedEventArgs(state.Peer, requestBlock.FileWadId, requestBlock.Block));
          break;
          
        case MessageType.StartBlockTransfer:
          
          var startTransfer = msg as StartBlockTransferNetMessage;

          var mgr = new BlockTransferManager();
          mgr.ExpectedPackets = startTransfer.TotalPackets;
          mgr.TransferId = startTransfer.TransferId;
          mgr.FileWadId = startTransfer.FileWadId;
          mgr.TotalLength = startTransfer.BlockSize;

          InProgressTransfers.Add(startTransfer.TransferId, mgr);
          
          break;

        case MessageType.BlockTransferPacket:
          var packet = msg as BlockPacketNetMessage;
          var transferManager = InProgressTransfers[packet.TransferId];
          transferManager.SavePacket(packet);
          if ( transferManager.IsCompleted )
          {
            InProgressTransfers.Remove(transferManager.TransferId);
            
            OnBlockReceived( new BlockReceivedEventArgs( state.Peer, transferManager.FileWadId, transferManager.Block ));
          }

          break;
      }
    }
    
    public void Connect( Peer peer )
    {
      if ( String.IsNullOrWhiteSpace(peer.IPAddress) )
        return;
      
      try {
        Log(string.Format("CONNECT: {0}", peer));
        var state = new ReceiveStateObject();
        peer.Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        state.Peer = peer;
        
        IPEndPoint remoteEP = new IPEndPoint( IPAddress.Parse(peer.IPAddress), peer.Port);
        peer.Socket.BeginConnect( remoteEP, ConnectCallback, state);
      }
      catch (Exception)
      {
        Log("CONNECT FAILED: {0}", peer);
        peer.Socket.Dispose();
        peer.Socket = null;
        OnPeerConnectFailed( new GenericEventArgs<Peer>(peer) );
      }
    }
    
    void ConnectCallback( IAsyncResult result )
    {
      var connectState = result.AsyncState  as ReceiveStateObject;

      try
      {
        connectState.Peer.Socket.EndConnect(result);
        
        OnPeerConnected(new GenericEventArgs<Peer>(connectState.Peer));
        
        // Create the state object.
        var recvState = new ReceiveStateObject();
        recvState.Peer = connectState.Peer;
        recvState.ExpectedLength = sizeof(int);
        recvState.WaitingLengthFrame = true;
        recvState.Peer.Socket.BeginReceive( recvState.Buffer.GetBuffer(), 0, recvState.ExpectedLength, 0,
                                           new AsyncCallback(WaitMessageCallback), recvState);
      }
      catch( Exception ex )
      {
        Disconnect(connectState.Peer, "Connect failed to " + connectState.Peer.IPAddress + " "  + connectState.Peer.Port);
        OnPeerConnectFailed( new GenericEventArgs<Peer>( connectState.Peer) );
        
      }
    }
    
    public void Disconnect( Peer peer, string why )
    {
      Log(string.Format("DISCONNECT: {0} {1} {2} {3}", why, peer.Name, peer.IPAddress, peer.Id));
      if ( peer.Socket != null )
      {
        peer.Socket.Dispose();
        peer.Socket = null;
      }
      ActivePeers.Remove(peer);
    }
    
    
    
    public void RequestStacks( Peer peer, long recency, int count )
    {
      var msg = new RequestStacksNetMessage();
      msg.Recency = recency;
      msg.Count = count;
      Send(msg, peer);

    }

    
    public void RequestWads(Peer peer,  long recency, int count, Guid stackGuid )
    {
      var msg = new RequestWadsNetMessage();
      msg.Recency = recency;
      msg.Count = count;
      msg.StackGuid = stackGuid;
      Send(msg, peer);

    }
    
    public void RequestPeers( Peer peer,  int count )
    {
      var msg = new RequestPeersNetMessage();
      msg.Count = count;
      Send(msg, peer);

    }
    
    public void RequestBlocksAvailable( Peer peer, FileWad wad )
    {
      var msg = new RequestBlocksAvailableNetMessage();
      msg.FileWadId = wad.Id;
      Send(msg, peer);

    }
    
    public void SendPeerList(Peer to, Peer[] peers )
    {
      var msg = new PeerListNetMessage();
      msg.Peers = peers;
      Send(msg, to);

    }

    public void SendMyStatus(Peer peer, Peer me )
    {
      var msg = new PeerListNetMessage();
      msg.Type = MessageType.PeerStatus;
      msg.Peers = new []{me};
      Send(msg, peer);

    }

    
    public void SendStacks( Peer peer, Stack[] stacks )
    {
      var msg = new StacksNetMessage();
      msg.Stacks = stacks;
      Send(msg, peer);

    }
    
    public void SendWads( Peer peer, FileWad[] wads )
    {
      var msg = new WadsNetMessage();
      msg.Wads = wads;
      Send(msg, peer);

    }
    
    public void SendBlock( Peer peer, FileWad fileWad, int block )
    {
      int totalPackets = (int)Math.Ceiling((decimal)fileWad.BlockIndex[block].Length / peer.MaxBlockPacketSize);
      
      var msg = new StartBlockTransferNetMessage();
      msg.Block = block;
      msg.BlockSize = (int)fileWad.BlockIndex[block].Length;
      msg.TotalPackets = totalPackets;
      msg.TransferId = Guid.NewGuid();
      Send(msg, peer);
      
      
    }
    
    public void RequestBlock( Peer peer, FileWad fileWad, int block )
    {
      var msg = new RequestBlockNetMessage();
      msg.Block = block;
      msg.FileWadId = fileWad.Id;
      Send(msg, peer);
    }
    
    public void SendBlocksAvailable( Peer peer, FileWad fileWad )
    {
      var msg = new BlocksAvailableNetMessage();
      msg.BlocksAvailable = new bool[fileWad.BlockIndex.Count];
      for(int i=0;i<fileWad.BlockIndex.Count;i++)
        msg.BlocksAvailable[i] = fileWad.BlockIndex[i].Downloaded;
      msg.FileWadId = fileWad.Id;
      Send(msg, peer);
    }

    public void Send( NetMessage msg, params Peer[] peers )
    {
      foreach( var peer in peers )
      {
        if ( !peer.IsConnected )
          return;
        
        var state = new SendState();
        
        state.Message = msg;
        state.Peer = peer;
        
        using( var stream = new MemoryStream() )
        {
          
          byte[] buffer = msg.ToBytes();
          byte[] lengthBuffer = BitConverter.GetBytes(buffer.Length);
          
          stream.Write(lengthBuffer, 0, lengthBuffer.Length);
          stream.Write(buffer, 0, buffer.Length);
          
          try
          {
            peer.Socket.BeginSend(stream.GetBuffer(), 0, (int)stream.Length, 0, EndSendCallback, state);
            
          }
          catch( Exception ex )
          {
            Disconnect(peer, "Beginning to send to failed");
          }
        }
      }
    }

    void EndSendCallback(IAsyncResult ar)
    {
      var state = ar.AsyncState as SendState;
      try
      {
        if ( state.Peer.IsConnected )
        {
          int bytesSent = state.Peer.Socket.EndSend(ar);
          state.Peer.BytesSent += bytesSent;
          BytesSent += bytesSent;
          Log("SENT: {0} {1} bytes to client.", state.Message, bytesSent);
        }
      }
      catch (Exception e)
      {
        Disconnect(state.Peer, "Send failed");
        Console.WriteLine(e.ToString());
      }
    }
    
    void Log( string format, params object [] args )
    {
      string str = string.Format(format, args );
      Console.WriteLine("{0} {1}", Me.Id, str);
    }
  }
  
  public class BlockTransferManager : TransferManager
  {
    public override void Execute()
    {
      var fileWad = MoustacheLayer.Singleton.Catalog.GetFileWad(FileWadId);
      
      fileWad.VerifyBlock(TempFile);
      fileWad.CatalogBlock(Block, TempFile);
      fileWad.BlockIndex[Block].Downloaded = true;
    }
  }
  
  public abstract class TransferManager
  {
    public Guid TransferId { get; set; }
    public Guid FileWadId { get; set; }
    public int Block { get; set; }
    public int ExpectedPackets { get; set; }
    public int TotalLength { get; set; }
    public int NextPacket { get; set; }
    public bool IsCompleted { get { return ExpectedPackets <= NextPacket; } }
    public string TempFile { get; set; }
    
    public TransferManager()
    {
      NextPacket = 0;
    }
    
    public void SavePacket( BlockPacketNetMessage msg )
    {
      if ( String.IsNullOrWhiteSpace(TempFile))
        TempFile = Path.Combine(Path.GetTempPath(), TransferId + ".dat");
      
      Console.WriteLine("Saving block to " + TempFile);
      
      using ( var stream = new FileStream(TempFile, FileMode.OpenOrCreate))
      {
        stream.Write(msg.Data, 0, msg.DataLength);
      }
      
      NextPacket++;
      if ( IsCompleted )
        Execute();
    }
    
    public abstract void Execute();
  }
}
