﻿

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using FuzzyHipster.Catalog;
using FuzzyHipster.Crypto;

namespace FuzzyHipster.Network
{
  public class StreamTransferManagerCollection : SortedDictionary<Guid, TransferManager>
  {
    public void Remove( TransferManager mgr )
    {
      Remove(mgr.TransferId);
    }
    
    public void Cancel( Guid id )
    {
      Remove(id);
    }
    
    public void Cancel( TransferManager mgr )
    {
      Remove(mgr);
    }
    
    public TransferManager FindTransfer( FileWad wad, int block, Peer peer )
    {
      foreach( var transfer in Values )
        if ( transfer.FileWad.Id == wad.Id && transfer.Block == block && transfer.Peer == peer )
          return transfer;
      return null;
    }
  }
  
  public class RWNetwork
  {
    public const int RWDefaultPort = 7892;
    
    public PeerCollection ActivePeers { get; set; }
    public IPEndPoint LocalEndPoint { get; set; }
    public Socket Listener { get; set; }
    public Peer Me { get; set; }
    public StreamTransferManagerCollection InProgressTransfers { get; set; }
    public RateLimiter RateLimiter { get; set; }
    
    public long BytesReceived { get; set; }
    public long BytesSent { get; set; }
    
    Random random = new Random(DateTime.Now.Millisecond);
    ManualResetEvent acceptSemaphore = new ManualResetEvent(false);
    
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
    /// Fires when there's a new channel
    /// </summary>
    public event EventHandler<GenericEventArgs<Channel>> NewChannel;
    protected virtual void OnNewChannel(GenericEventArgs<Channel> e)
    {
      var handler = NewChannel;
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
    
    public event EventHandler<MessageComposite<Key>> KeyReceived;
    protected virtual void OnKeyReceived(MessageComposite<Key> e)
    {
      var handler = KeyReceived;
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
    /// Fires when we received a request for channels
    /// </summary>
    public event EventHandler<MessageComposite<RequestChannelsNetMessage>> ChannelsRequested;

    protected virtual void OnChannelsRequested(MessageComposite<RequestChannelsNetMessage> e)
    {
      var handler = ChannelsRequested;
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

    protected virtual void OnBlockTransferStarting(BlockTransferStartedEventArgs e)
    {
      var handler = BlockTransferStarted;
      if (handler != null)
        handler(this, e);
    }
    
    public event EventHandler<BlockTransferStartedEventArgs> BlockTransferFailed;

    protected virtual void OnBlockTransferFailed(BlockTransferStartedEventArgs e)
    {
      var handler = BlockTransferFailed;
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
      ActivePeers = new PeerCollection();
      InProgressTransfers = new StreamTransferManagerCollection();
      
      if ( MoustacheLayer.Singleton != null )
        RateLimiter = new RateLimiter(MoustacheLayer.Singleton.Settings.MaxReceiveRate);
      else
        RateLimiter = new RateLimiter(RateLimiter.UnlimitedRate);

    }
    
    /// <summary>
    /// Connect to a peer
    /// </summary>
    /// <param name="peer"></param>
    public void Connect( Peer peer )
    {
      if ( String.IsNullOrWhiteSpace(peer.HostAddress) )
        return;
      
      try {
        Log(string.Format("CONNECT: {0}", peer));
        
        IPAddress [] addresses = Dns.GetHostAddresses(peer.HostAddress);
        
        if ( addresses.Length == 0 )
          throw new Exception("No address");
        
        IPEndPoint remoteEP = new IPEndPoint( addresses[0], peer.Port);
        
        var state = new ReceiveStateObject();
        peer.Socket = new NetworkSocket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        peer.SymmetricKey = null; // we have no agreement!
        state.Peer = peer;
        
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
        
        SendHello(connectState.Peer, false); // server will send us a Key
        OnPeerConnected(new GenericEventArgs<Peer>(connectState.Peer));
        
        // Create the state object.
        var recvState = new ReceiveStateObject();
        recvState.Peer = connectState.Peer;
        recvState.Peer.LastConnection = DateTime.Now;
        recvState.ExpectedLength = sizeof(int);
        recvState.ExpectedMessage = MessageType.AsymmetricKeyAck;
        recvState.WaitingLengthFrame = true;

        //receiveSemaphore.Reset();
        recvState.Peer.Socket.BeginReceive( recvState.Buffer.GetBuffer(), 0, recvState.ExpectedLength, 0,
                                           new AsyncCallback(WaitMessageCallback), recvState);
      }
      catch( Exception ex )
      {
        Log(ex.ToString());
        Disconnect(connectState.Peer, "Connect failed to " + connectState.Peer.HostAddress + " "  + connectState.Peer.Port);
        OnPeerConnectFailed( new GenericEventArgs<Peer>( connectState.Peer) );
      }
    }
    
    public void Disconnect( Peer peer, string why )
    {
      Log(string.Format("DISCONNECT: {0} {1} {2} {3}", why, peer.Name, peer.HostAddress, peer.Id));
      if ( peer.Socket != null )
      {
        peer.Socket.Disconnect();
        peer.Socket.Dispose();
        peer.Socket = null;
      }
      ActivePeers.Remove(peer);
      
      var list = new List<TransferManager>();
      
      foreach( var mgr in InProgressTransfers.Values )
        if ( mgr.Peer == peer )
          list.Add(mgr);
      
      foreach( var mgr in list )
      {
        InProgressTransfers.Remove(mgr);
        OnBlockTransferFailed(new BlockTransferStartedEventArgs(mgr.Peer, mgr.FileWad, mgr.Block));
      }
      
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
          acceptSemaphore.Reset();

          Listener.BeginAccept(
            new AsyncCallback(AcceptCallback),
            Listener );

          // Wait until a connection is made before continuing.
          acceptSemaphore.WaitOne();
        }

      } catch (Exception e) {
        Log(e.ToString());
      }
    }

    void AcceptCallback(IAsyncResult ar)
    {
      // Signal the main thread to continue.
      acceptSemaphore.Set();

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
          Socket = new NetworkSocket(handler),
          HostAddress = (handler.RemoteEndPoint as IPEndPoint).Address.ToString(),
          Port = 0,
          CatalogRecency = 0,
          PeerCount = 0,
          Name = "",
          Uptime = 0
        };
        
        // Create the state object.
        var state = new ReceiveStateObject();
        state.Peer = peer;
        state.Peer.LastConnection = DateTime.Now;
        state.ExpectedLength = sizeof(int);
        state.WaitingLengthFrame = true;
        state.ExpectedMessage = MessageType.AsymmetricKeyHello;
        
        try
        {
          //receiveSemaphore.Reset();
          handler.BeginReceive( state.Buffer.GetBuffer(), 0, state.ExpectedLength, 0,
                               new AsyncCallback(WaitMessageCallback), state); // get the message size first
          
          
          //SendHello(state.Peer, true);
        }
        catch( Exception ex )
        {
          Log(ex);
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
        //receiveSemaphore.Set();
        peer.BytesReceived += bytesRead;
        BytesReceived += bytesRead;
        
        // record the length so we can use it to calculate statistics
        peer.RateLimiter.GotPacket( bytesRead );
        // see if we need to limit ourselves
        RateLimiter.GotPacket( bytesRead );

        state.Buffer.Seek(bytesRead, SeekOrigin.Current);
        
        if ( bytesRead == 0 )
          Disconnect(peer, "Connection closed");
        else if ( bytesRead > 0 && bytesRead < state.ExpectedLength ) // partial receive
        {
          // buffering
          state.ExpectedLength -= bytesRead;
          BeginReceive( peer, state.Buffer.GetBuffer(), (int)state.Buffer.Position, state.ExpectedLength, state);
        }
        else if ( bytesRead == state.ExpectedLength ) // receive finished
        {
          //state.Buffer.Seek(0, SeekOrigin.Begin);
          
          if ( state.WaitingLengthFrame )
          {
            state.ExpectedLength = BitConverter.ToInt32(ReceiveBytes(state), 0);
            state.WaitForData();
            if ( state.ExpectedLength <= 0 )
              Disconnect(state.Peer, "Expected Length was a garbage value!");
            else if ( state.ExpectedLength > state.Buffer.Capacity )
              Disconnect(state.Peer, "Expected Length is greater than the buffer capacity!");
            else
              BeginReceive( peer, state.Buffer.GetBuffer(), 0, state.ExpectedLength, state);
          }
          else
          {
            NetMessage message = NetMessage.FromBytes(ReceiveBytes(state));
            
            if ( state.ExpectedMessage != MessageType.Unknown && message.Type != state.ExpectedMessage) // crappy connection
              Disconnect(state.Peer, string.Format("Didn't receive the message we expected {0}. Received {1}", state.ExpectedMessage, message.Type));
            else
            {
              state.ExpectedMessage = MessageType.Unknown;
              state.WaitForLength(); // waiting for the message size
              
              OnNetMessageReceived(new GenericEventArgs<NetMessage>(message));
              ProcessMessage(message, state);
              BeginReceive( peer, state.Buffer.GetBuffer(), 0, state.ExpectedLength, state); // read the message size first
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
        Log(ex);
      }
    }
    
    void BeginReceive( Peer peer, byte[] buffer, int position, int count, ReceiveStateObject state )
    {
      // will halt if the data is coming in too fast
      peer.RateLimiter.Limit();
      RateLimiter.Limit();
      peer.Socket.BeginReceive( buffer, position, count, 0,
                               new AsyncCallback(WaitMessageCallback), state);
    }
    
    void ProcessMessage(NetMessage msg, ReceiveStateObject state)
    {
      Log("RECV: {0}", msg);
      
      switch( msg.Type )
      {
        case MessageType.AsymmetricKeyHello:
          var hellokey = msg as KeyNetMessage;
          
          var asymmetricKey = hellokey.Key as AsymmetricKey;
          if (asymmetricKey == null)
            return;
          
          state.Peer.AsymmetricKey = asymmetricKey;
          state.ExpectedMessage = MessageType.SymmetricKey;
          
          Send( new KeyNetMessage()
               {
                 Type = MessageType.AsymmetricKeyAck,
                 Key = Me.AsymmetricKey
               }, state.Peer);
          
          
          OnKeyReceived( new MessageComposite<Key>(state.Peer, hellokey.Key));
          break;
          
        case MessageType.AsymmetricKeyAck:
          var ackkey = msg as KeyNetMessage;
          
          if ( ackkey.Key is AsymmetricKey )
            state.Peer.AsymmetricKey = ackkey.Key as AsymmetricKey;
          else
            throw new Exception(string.Format("Received Asymmetric KeyNetMessage but the key was not Asymmetric. Key type is '{0}'", ackkey.Key));
          
          if ( Me.SymmetricKey == null )
            Me.SymmetricKey = SymmetricKey.Create();
          
          state.ExpectedMessage = MessageType.PeerStatus;
          Send( new KeyNetMessage()
               {
                 Type = MessageType.SymmetricKey,
                 Key = Me.SymmetricKey
               }, state.Peer);
          
          state.Peer.SymmetricKey = Me.SymmetricKey;
          state.WaitForLength();
          OnKeyReceived( new MessageComposite<Key>(state.Peer, ackkey.Key));
          break;
          
        case MessageType.SymmetricKey:
          var symkey = msg as KeyNetMessage;
          
          if ( symkey.Key is SymmetricKey )
          {
            state.Peer.SymmetricKey = symkey.Key as SymmetricKey;
            state.WaitForLength(); // resets the length size to 16 bytes. Min int size encrypted!
          }
          else
            throw new Exception(string.Format("Received SymmetricKey KeyNetMessage but the key was not Symmetric. Key type is '{0}'", symkey.Key));
          
          if ( state.Handshaking )
          {
            state.ExpectedMessage = MessageType.PeerStatus;
            state.Handshaking = false;
            SendMyStatus(state.Peer);
          }
          
          OnKeyReceived( new MessageComposite<Key>(state.Peer, symkey.Key));
          break;
          
        case MessageType.PeerStatus:
          var status = msg as PeerListNetMessage;
          
          if ( state.Handshaking )
          {
            state.Handshaking = false;
            SendMyStatus(state.Peer);
          }
          
          state.Peer.CatalogRecency = status.Peers[0].CatalogRecency;
          state.Peer.Id = status.Peers[0].Id;
          state.Peer.Name = status.Peers[0].Name;
          state.Peer.PeerCount = status.Peers[0].PeerCount;
          state.Peer.Uptime = status.Peers[0].Uptime;
          state.Peer.HostAddress = (state.Peer.Socket.RemoteEndPoint as IPEndPoint).Address.ToString();
          state.Peer.Port = status.Peers[0].Port;
          
          DateTime okToSend = DateTime.Now.AddMilliseconds(MoustacheLayer.Singleton.Settings.ThinkTimeGraceMilliseconds);
          if ( state.Peer.OkToSendAt > okToSend )
            state.Peer.OkToSendAt = okToSend;
          
          if ( !ActivePeers.Contains(state.Peer))
            ActivePeers.Add(state.Peer);
          
          OnNewPeer(new GenericEventArgs<Peer>(state.Peer));
          
          break;
          
        case MessageType.Hello:
          break;
          
        case MessageType.Goodbye:
          Disconnect(state.Peer, "Disconnected gracefully");
          break;
          
        case MessageType.RequestPeers:
          var request = msg as RequestPeersNetMessage;
          OnPeersRequested(new MessageComposite<RequestPeersNetMessage>(state.Peer, request));
          break;
          
        case MessageType.RequestChannels:
          var channels = msg as RequestChannelsNetMessage;
          OnChannelsRequested(new MessageComposite<RequestChannelsNetMessage>(state.Peer, channels));
          break;
          
        case MessageType.RequestWads:
          var wads = msg as RequestWadsNetMessage;
          OnWadsRequested(new MessageComposite<RequestWadsNetMessage>(state.Peer, wads));
          break;
          
          
        case MessageType.Peers:
          var peerList = msg as PeerListNetMessage;
          
          foreach( var peer in peerList.Peers )
            if ( !String.IsNullOrWhiteSpace(peer.HostAddress))
              OnNewPeer(new GenericEventArgs<Peer>(peer));
          break;
          
        case MessageType.Channels:
          var channelsList = msg as ChannelsNetMessage;
          foreach( var channel in channelsList.Channels )
            OnNewChannel( new GenericEventArgs<Channel>(channel));
          break;
          
        case MessageType.Wads:
          var wadsList = msg as WadsNetMessage;
          if ( wadsList.Wads != null )
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
          OnBlockRequested( new BlockRequestedEventArgs(state.Peer, MoustacheLayer.Singleton.Catalog.GetFileWad(requestBlock.FileWadId), requestBlock.Block));
          break;
          
        case MessageType.StartBlockTransferRequest:
          
          var startTransfer = msg as StartBlockTransferRequestNetMessage;

          var mgr = new BlockTransferManager();
          mgr.Peer = state.Peer;
          mgr.ExpectedPackets = startTransfer.TotalPackets;
          mgr.TransferId = startTransfer.TransferId;
          mgr.TotalLength = startTransfer.BlockSize;
          mgr.FileWad = MoustacheLayer.Singleton.Catalog.GetFileWad(startTransfer.FileWadId);
          mgr.Block = startTransfer.Block;
          mgr.FileWad.BlockIndex[mgr.Block].Downloading = true;
          
          InProgressTransfers.Add(startTransfer.TransferId, mgr);
          
          
          var btsevt = new BlockTransferStartedEventArgs(state.Peer, mgr.FileWad, mgr.Block);
          OnBlockTransferStarting(btsevt);
          SendBlockTransferAcknowledgement( mgr, btsevt.Accept, state.Peer );
          
          break;
          
        case MessageType.StartBlockTransferAck:
          
          var startTransferAck = msg as StartBlockTransferAcknowledgementNetMessage;
          
          if ( InProgressTransfers.ContainsKey(startTransferAck.TransferId))
          {
            if ( startTransferAck.Accept )
            {
              SendBlock( InProgressTransfers[startTransferAck.TransferId], state.Peer );
              InProgressTransfers.Remove(InProgressTransfers[startTransferAck.TransferId].TransferId);
            }
            else
              InProgressTransfers.Cancel(startTransferAck.TransferId);
          }
          else
          {
            Log("RECV: Ack for not existant transfer: {0}", startTransferAck.TransferId);
          }
          
          break;

        case MessageType.BlockTransferPacket:
          
          var packet = msg as BlockPacketNetMessage;
          
          var transferManager = InProgressTransfers[packet.TransferId];
          
          transferManager.SavePacket(packet);
          if ( transferManager.IsCompleted )
          {
            InProgressTransfers.Remove(transferManager.TransferId);
            
            OnBlockReceived( new BlockReceivedEventArgs( state.Peer, transferManager.FileWad, transferManager.Block ));
          }

          break;
      }
    }
    
    public void RequestChannels( Peer peer, long recency, int count )
    {
      var msg = new RequestChannelsNetMessage();
      msg.Recency = recency;
      msg.Count = count;
      Send(msg, peer);

    }
    
    public void RequestChannel( Peer peer, Guid guid )
    {
      var msg = new RequestChannelsNetMessage();
      msg.Recency = 0;
      msg.Count = 1;
      msg.Guids = new [] {guid};
      Send(msg, peer);
    }

    
    public void RequestWads(Peer peer,  long recency, int count, Guid channelGuid )
    {
      var msg = new RequestWadsNetMessage();
      msg.Recency = recency;
      msg.Count = count;
      msg.ChannelGuid = channelGuid;
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
    
    public void SendPeerList(Peer[] listToSend, params Peer[] to )
    {
      var msg = new PeerListNetMessage();
      msg.Peers = listToSend;
      Send(msg, to);

    }
    
    public void SendHello( Peer to, bool sendSymmetricKey )
    {
      SendKey(to, Me.AsymmetricKey.GetPublicKey());
      
      if ( sendSymmetricKey )
      {
        if ( Me.SymmetricKey == null )
          Me.SymmetricKey = SymmetricKey.Create();
        Send( new KeyNetMessage()
             {
               Type = MessageType.SymmetricKey,
               Key = Me.SymmetricKey
             }, to);
        to.SymmetricKey = Me.SymmetricKey;
      }
    }

    public void SendMyStatus( params Peer[] to )
    {
      var msg = new PeerListNetMessage();
      msg.Type = MessageType.PeerStatus;
      msg.Peers = new []{Me};
      Send(msg, to);
    }

    
    public void SendChannels( Channel[] channels, params Peer[] to )
    {
      var msg = new ChannelsNetMessage();
      msg.Channels = channels;
      Send(msg, to);
    }
    
    public void SendWads( FileWad[] wads, params Peer[] to )
    {
      var msg = new WadsNetMessage();
      msg.Wads = wads;
      Send(msg, to);
    }
    
    public void SendBlockTransferStartRequest( FileWad fileWad, int block, params Peer [] to )
    {
      foreach( Peer p in to )
      {
        int maxBlockPacketSize = p.MaxBlockPacketSize;
        if ( maxBlockPacketSize == 0 )
          maxBlockPacketSize = MoustacheLayer.Singleton.Settings.DefaultMaxBlockPacketSize;
        
        int totalPackets = (int)Math.Ceiling((decimal)fileWad.BlockIndex[block].Length / (decimal)maxBlockPacketSize);
        
        var msg = new StartBlockTransferRequestNetMessage();
        msg.Block = block;
        msg.BlockSize = (int)fileWad.BlockIndex[block].Length;
        msg.TotalPackets = totalPackets;
        msg.TransferId = Guid.NewGuid();
        msg.FileWadId = fileWad.Id;
        
        InProgressTransfers.Add(msg.TransferId, new BlockTransferManager()
                                {
                                  TransferId = msg.TransferId,
                                  TotalLength = (int)fileWad.BlockIndex[block].Length,
                                  NextPacket = 0,
                                  CurrentPosition = 0,
                                  FileWad = fileWad,
                                  Block = block,
                                  ExpectedPackets = totalPackets,
                                  MaxPacketSize = maxBlockPacketSize,
                                  TempFile = "",
                                  Peer = p
                                    
                                });
        
        Send(msg, p);
      }
    }
    
    public void SendBlockTransferAcknowledgement( TransferManager mgr, bool accept, Peer to )
    {
      var msg = new StartBlockTransferAcknowledgementNetMessage();
      msg.Accept = accept;
      msg.Block = mgr.Block;
      msg.FileWadId = mgr.FileWad.Id;
      msg.TransferId = mgr.TransferId;
      Send(msg, to);
    }

    
    public void SendBlock( TransferManager mgr, params Peer[] to )
    {
      foreach( Peer p in to )
      {
        using ( var stream = BlockStream.Create( mgr.FileWad, mgr.Block ))
        {
          long length = mgr.FileWad.BlockIndex[mgr.Block].Length;
          for ( int i=0;i<mgr.ExpectedPackets;i++)
          {
            int packetSize = mgr.MaxPacketSize;
            if ( packetSize > length )
              packetSize = (int)length;
            
            var blockMsg = new BlockPacketNetMessage();
            blockMsg.Data = new byte[packetSize];
            blockMsg.DataLength = packetSize;
            blockMsg.TransferId = mgr.TransferId;
            
            stream.Read(blockMsg.Data, 0, packetSize);
            
            length -= packetSize;
            
            Send(blockMsg, p);
          }
        }
      }
    }
    
    public void RequestBlock( Peer peer, FileWad fileWad, int block )
    {
      if ( peer == null ) throw new Exception("peer is null for RequestBlock({0})" + block);
      if ( fileWad == null ) return;
      if ( block < 0 ) return;
      if ( fileWad.BlockIndex.Count <= block ) return;
      
      var msg = new RequestBlockNetMessage();
      msg.Block = block;
      msg.FileWadId = fileWad.Id;
      Send(msg, peer);
    }
    
    public void SendBlocksAvailable( Peer peer, FileWad fileWad )
    {
      if ( fileWad.BlockIndex == null )
      {
        Log(string.Format("BUILD: Failed, {0} has an empty BlockIndex", fileWad.Id));
        return;
      }
      
      var msg = new BlocksAvailableNetMessage();
      msg.BlocksAvailable = fileWad.GetBlockAvailability();
      msg.FileWadId = fileWad.Id;
      Send(msg, peer);
    }
    
    public void SendKey( Peer to, Key key )
    {
      var msg = new KeyNetMessage();
      msg.Key = key;
      Send(msg,to);
    }

    
    public void Send( NetMessage msg, params Peer[] peers )
    {
      foreach( var peer in peers )
      {
        if ( peer == null )
          continue;
        
        if ( !peer.IsConnected )
          continue;
        
        if ( peer == Me )
          continue;
        
        //          SendMessageViaRelay(msg, peer);
        
        var state = new SendState();
        
        state.Message = msg;
        state.Peer = peer;
        
        using( var stream = new MemoryStream() )
        {
          
          byte[] buffer = msg.ToBytes();
          buffer = SendBytes(buffer, state);
          
          byte[] lengthBuffer = BitConverter.GetBytes(buffer.Length);
          lengthBuffer = SendBytes(lengthBuffer, state);
          
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
        Log(e);
      }
    }
    
    void Log( Exception ex )
    {
      Log(ex.ToString());
    }
    
    void Log( string format, params object [] args )
    {
      string str = string.Format(format, args );
      Console.WriteLine(str);
      Debug.Print(str);
    }
    
    byte[] ReceiveBytes( ReceiveStateObject state )
    {
      if ( state.Peer.SymmetricKey != null )
        return state.Peer.SymmetricKey.Decrypt(state.Buffer.GetBuffer(), (int)state.Buffer.Position);
      else
        return state.Buffer.GetBuffer();
    }
    
    byte[] SendBytes( byte[] buffer, SendState state )
    {
      byte [] sendBytes;
      
      if ( state.Peer.SymmetricKey != null )
      {
        sendBytes = state.Peer.SymmetricKey.Encrypt(buffer, buffer.Length);
      }
      else
        sendBytes = buffer;
      
      return sendBytes;
    }
  }
  
  
  
  
}
