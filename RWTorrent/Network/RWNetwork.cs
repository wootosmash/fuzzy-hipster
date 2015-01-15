

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
  // State object for reading client data asynchronously
  public class ReceiveStateObject
  {
    public const int BufferSize = 65536;

    public Peer Peer = null;
    public bool WaitingFrame = true;

    public byte[] Buffer = new byte[BufferSize];
  }
  
  public class SendState
  {
    public NetMessage Message;
    public Peer Peer;
  }
  
  public class GenericEventArgs<T> : EventArgs
  {
    public T Value { get; set; }
    
    public GenericEventArgs(T value)
    {
      Value = value;
    }
  }

  public class MessageComposite<T> : EventArgs
  {
    public Peer Peer { get; set; }
    public T Value { get; set; }
    
    public MessageComposite( Peer peer, T value )
    {
      Peer = peer;
      Value = value;
    }
  }
  
  public class BlockReceivedEventArgs : EventArgs
  {
    public int Block { get; set; }
    public Guid FileWadId { get; set; }
    public Peer Peer { get; set; }
    
    public BlockReceivedEventArgs( Peer peer, Guid fileWadId, int block )
    {
      this.Peer = peer;
      this.Block = block;
      this.FileWadId = fileWadId;
    }
  }
  
  public class BlockRequestedEventArgs : EventArgs
  {
    public int Block { get; set; }
    public Guid FileWadId { get; set; }
    public Peer Peer { get; set; }
    
    public BlockRequestedEventArgs( Peer peer, Guid fileWadId, int block )
    {
      this.Peer = peer;
      this.Block = block;
      this.FileWadId = fileWadId;
    }
  }
  
  public class BlockTransferStartedEventArgs : EventArgs 
  {
    public int Block { get; set; }
    public Guid FileWadId { get; set; }
    public Peer Peer { get; set; }
    
    public BlockTransferStartedEventArgs( Peer peer, Guid fileWadId, int block )
    {
      this.Peer = peer;
      this.Block = block;
      this.FileWadId = fileWadId;
    }
  }
  
  public class RWNetwork
  {
    public const int RWDefaultPort = 7892;
    
    public List<Peer> ActivePeers { get; set; }
    public IPEndPoint LocalEndPoint { get; set; }
    public Socket Listener { get; set; }
    public Guid Id { get; set; }
    public SortedList<Guid, TransferManager> InProgressTransfers { get; set; }
    
    Random random = new Random(DateTime.Now.Millisecond);
    ManualResetEvent allDone = new ManualResetEvent(false);
    
    #region events
    
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
    
    public RWNetwork()
    {
      ActivePeers = new List<Peer>();
      Id = Guid.NewGuid();
      InProgressTransfers = new SortedList<Guid, TransferManager>();
    }

    /// <summary>
    /// Starts the Network listening for incoming connections
    /// </summary>
    /// <param name="port"></param>
    public void StartListening( int port )
    {
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
      
      var peer = new Peer()
      {
        Guid = Guid.Empty,
        Socket = handler,
        IPAddress = (handler.RemoteEndPoint as IPEndPoint).Address.ToString(),
        Port = (handler.RemoteEndPoint as IPEndPoint).Port,
        CatalogRecency = 0,
        PeerCount = 0,
        Name = "",
        Uptime = 0
      };
      
      ActivePeers.Add(peer);
      
      // Create the state object.
      var state = new ReceiveStateObject();
      state.Peer = peer;
      handler.BeginReceive( state.Buffer, 0, sizeof(int), 0,
                           new AsyncCallback(WaitMessageCallback), state); // get the message size first
    }

    void WaitMessageCallback(IAsyncResult ar)
    {
      var state =  ar.AsyncState as ReceiveStateObject;
      Peer peer = state.Peer;
      
      try
      {

        int bytesRead = peer.Socket.EndReceive(ar);
        
        if ( bytesRead > 0 )
        {
          if ( state.WaitingFrame )
          {
            int length = BitConverter.ToInt32(state.Buffer, 0);
            if ( length < state.Buffer.Length )
              peer.Socket.BeginReceive( state.Buffer, 0, length, 0,
                                       new AsyncCallback(WaitMessageCallback), state);
            state.WaitingFrame = false;
          }
          else
          {
            NetMessage message = NetMessage.FromBytes(state.Buffer);
            
            ProcessMessage(message, state);
            OnNetMessageReceived(new GenericEventArgs<NetMessage>(message));
            peer.Socket.BeginReceive( state.Buffer, 0, sizeof(int), 0,
                                     new AsyncCallback(WaitMessageCallback), state); // read the message size first
            state.WaitingFrame = true; // waiting for the message size
          }
        }

      }
      catch( Exception ex )
      {
        ActivePeers.Remove(peer);
        peer.Socket.Dispose();
        OnPeerDisconnected( new GenericEventArgs<Peer>(peer));
        
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
          state.Peer.Guid = status.Peers[0].Guid;
          state.Peer.Name = status.Peers[0].Name;
          state.Peer.PeerCount = status.Peers[0].PeerCount;
          state.Peer.Uptime = status.Peers[0].Uptime;
          
          break;
          
        case MessageType.Peers:
          var peerList = msg as PeerListNetMessage;
          
          foreach( var peer in peerList.Peers )
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
        ActivePeers.Add(connectState.Peer);
        
        OnPeerConnected(new GenericEventArgs<Peer>(connectState.Peer));
        
        // Create the state object.
        var recvState = new ReceiveStateObject();
        recvState.Peer = connectState.Peer;
        recvState.Peer.Socket.BeginReceive( recvState.Buffer, 0, recvState.Buffer.Length, 0,
                                           new AsyncCallback(WaitMessageCallback), recvState);
      }
      catch( Exception ex )
      {
        Log("CONNECT FAILED: {0}", connectState.Peer);
        connectState.Peer.Socket.Dispose();
        connectState.Peer.Socket = null;
        ActivePeers.Remove(connectState.Peer);
        OnPeerConnectFailed( new GenericEventArgs<Peer>( connectState.Peer) );
        
      }
    }
    
    public void RequestStacks( Peer peer, long recency, int count )
    {
      var msg = new RequestStacksNetMessage();
      msg.Recency = recency;
      msg.Count = count;
      Send(peer, msg);
    }

    
    public void RequestWads(Peer peer,  long recency, int count, Guid stackGuid )
    {
      var msg = new RequestWadsNetMessage();
      msg.Recency = recency;
      msg.Count = count;
      msg.StackGuid = stackGuid;
      Send(peer, msg);
    }
    
    public void RequestPeers( Peer peer,  int count )
    {
      var msg = new RequestPeersNetMessage();
      msg.Count = count;
      Send( peer, msg);
    }
    
    
    public void SendPeerList(Peer peer, Peer[] peers )
    {
      var msg = new PeerListNetMessage();
      msg.Peers = peers;
      Send(peer, msg);
    }

    public void SendMyStatus(Peer peer, Peer me )
    {
      var msg = new PeerListNetMessage();
      msg.Type = MessageType.PeerStatus;
      msg.Peers = new []{me};
      Send(peer, msg);
    }

    
    public void SendStacks( Peer peer, Stack[] stacks )
    {
      var msg = new StacksNetMessage();
      msg.Stacks = stacks;
      Send(peer, msg);
    }
    
    public void SendWads( Peer peer, FileWad[] wads )
    {
      var msg = new WadsNetMessage();
      msg.Wads = wads;
      Send(peer, msg);
    }
    
    public void SendBlock( Peer peer, FileWad fileWad, int block )
    {
      var msg = new StartBlockTransferNetMessage();
      msg.Block = block;
      msg.BlockSize = (int)fileWad.BlockIndex[block].Length;
      msg.TotalPackets = (int)Math.Ceiling((decimal)fileWad.BlockIndex[block].Length / 40000);
      msg.TransferId = Guid.NewGuid();
      Send(peer, msg);
    }
    
    public void RequestBlock( Peer peer, FileWad fileWad, int block )
    {
      var msg = new RequestBlockNetMessage();
      msg.Block = block;
      msg.FileWadId = fileWad.Id;
      Send(peer, msg);
    }
    
    public void SendBlocksAvailable( Peer peer, FileWad fileWad )
    {
      var msg = new BlocksAvailableNetMessage();
      msg.BlocksAvailable = new bool[fileWad.BlockIndex.Count];
      for(int i=0;i<fileWad.BlockIndex.Count;i++)
        msg.BlocksAvailable[i] = fileWad.BlockIndex[i].Downloaded;
      msg.FileWadId = fileWad.Id;
      Send(peer, msg);
    }

    public void Send( Peer peer, NetMessage msg )
    {
      Log("SEND: {0} {1}", Id, msg);
      
      var state = new SendState();
      
      state.Message = msg;
      state.Peer = peer;
      
      byte[] buffer = msg.ToBytes();
      byte[] lengthBuffer = BitConverter.GetBytes(buffer.Length);
      
      peer.Socket.Send(lengthBuffer);
      peer.Socket.BeginSend(buffer, 0, buffer.Length, 0, EndSendCallback, state);
    }
    
    void Log( string format, params object [] args )
    {
      string str = string.Format(format, args );
      Console.WriteLine("{0} {1}", Id, str);
    }

    void EndSendCallback(IAsyncResult ar)
    {
      try
      {
        var state = ar.AsyncState as SendState;
        
        int bytesSent = state.Peer.Socket.EndSend(ar);
        Log("SEND: Sent {0} bytes to client.", bytesSent);
      }
      catch (Exception e)
      {
        Console.WriteLine(e.ToString());
      }
    }
  }
  
  public class BlockTransferManager : TransferManager
  {
    public override void Execute()
    {
      var fileWad = RWTorrent.Singleton.Catalog.GetFileWad(FileWadId);
      
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
      
      using ( FileStream stream = new FileStream(TempFile, FileMode.OpenOrCreate))
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
