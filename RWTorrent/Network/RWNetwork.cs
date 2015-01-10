

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using RWTorrent.Catalog;

namespace RWTorrent.Network
{
  // State object for reading client data asynchronously
  public class ReceiveStateObject {

    public Socket WorkSocket = null;

    public const int BufferSize = 1024;

    public byte[] buffer = new byte[BufferSize];

    public StringBuilder sb = new StringBuilder();
  }
  
  public class SendState
  {
    public NetMessage Message;
    public Socket Socket;
  }
  
  public class GenericEventArgs<T> : EventArgs
  {
    public T Value { get; set; }
    
    public GenericEventArgs(T value)
    {
      Value = value;
    }
  }

  
  
  public class RWNetwork
  {
    public const int RWDefaultPort = 7892;
    
    public List<Socket> Sockets { get; set; }
    public IPEndPoint LocalEndPoint { get; set; }
    public Socket Listener { get; set; }
    
    Random random = new Random(DateTime.Now.Millisecond);
    ManualResetEvent allDone = new ManualResetEvent(false);
    
    #region events
    public event EventHandler<GenericEventArgs<Stack>> NewStack;

    protected virtual void OnNewStack(GenericEventArgs<Stack> e)
    {
      var handler = NewStack;
      if (handler != null)
        handler(this, e);
    }

    public event EventHandler<GenericEventArgs<FileWad>> NewWad;

    protected virtual void OnNewWad(GenericEventArgs<FileWad> e)
    {
      var handler = NewWad;
      if (handler != null)
        handler(this, e);
    }

    public event EventHandler<GenericEventArgs<Peer>> NewPeer;

    protected virtual void OnNewPeer(GenericEventArgs<Peer> e)
    {
      var handler = NewPeer;
      if (handler != null)
        handler(this, e);
    }


    
    public event EventHandler<GenericEventArgs<RequestPeersNetMessage>> PeersRequested;

    protected virtual void OnPeersRequested(GenericEventArgs<RequestPeersNetMessage> e)
    {
      var handler = PeersRequested;
      if (handler != null)
        handler(this, e);
    }

    public event EventHandler<GenericEventArgs<RequestStacksNetMessage>> StacksRequested;

    protected virtual void OnStacksRequested(GenericEventArgs<RequestStacksNetMessage> e)
    {
      var handler = StacksRequested;
      if (handler != null)
        handler(this, e);
    }

    public event EventHandler<GenericEventArgs<RequestWadsNetMessage>> WadsRequested;

    protected virtual void OnWadsRequested(GenericEventArgs<RequestWadsNetMessage> e)
    {
      var handler = WadsRequested;
      if (handler != null)
        handler(this, e);
    }

    #endregion
    
    public RWNetwork()
    {
      Sockets = new List<Socket>();
    }

    public void StartListening()
    {
      // Data buffer for incoming data.
      byte[] bytes = new Byte[1024];

      LocalEndPoint = new IPEndPoint(IPAddress.Any, RWDefaultPort);
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

      Console.WriteLine("\nPress ENTER to continue...");
      Console.Read();
      
    }

    void AcceptCallback(IAsyncResult ar)
    {
      // Signal the main thread to continue.
      allDone.Set();

      // Get the socket that handles the client request.
      Socket listener = ar.AsyncState as Socket;
      Socket handler = listener.EndAccept(ar);
      
      Sockets.Add(handler);
      
      Console.WriteLine("WE GOT ONE...");

      // Create the state object.
      var state = new ReceiveStateObject();
      state.WorkSocket = handler;
      handler.BeginReceive( state.buffer, 0, state.buffer.Length, 0,
                           new AsyncCallback(WaitMessageCallback), state);
    }

    void WaitMessageCallback(IAsyncResult ar)
    {
      try
      {
        var state =  ar.AsyncState as ReceiveStateObject;
        Socket handler = state.WorkSocket;

        int bytesRead = handler.EndReceive(ar);
        
        if ( bytesRead > 0 )
        {
          Console.WriteLine("WaitMessage Recev");

          NetMessage message = NetMessage.FromBytes(state.buffer);
          
          ProcessMessage(message, state);
        }

        handler.BeginReceive( state.buffer, 0, state.buffer.Length, 0,
                             new AsyncCallback(WaitMessageCallback), state);
      }
      catch( Exception ex )
      {
        Console.WriteLine(ex);
      }
    }
    
    void ProcessMessage(NetMessage msg, ReceiveStateObject state)
    {
      Console.WriteLine("MSG: {0}", msg);
      
      switch( msg.Type )
      {
        case MessageType.Hello:
          break;
          
        case MessageType.RequestPeers:
          var request = msg as RequestPeersNetMessage;
          OnPeersRequested(new GenericEventArgs<RequestPeersNetMessage>(request));
          break;
          
        case MessageType.RequestStacks:
          var stacks = msg as RequestStacksNetMessage;
          OnStacksRequested(new GenericEventArgs<RequestStacksNetMessage>(stacks));
          break;
          
        case MessageType.RequestWads:
          var wads = msg as RequestWadsNetMessage;
          OnWadsRequested(new GenericEventArgs<RequestWadsNetMessage>(wads));
          break;
          
        case MessageType.PeerStatus:
        case MessageType.Peers:
          var status = msg as PeerListNetMessage;
          
          foreach( var peer in status.Peers )
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
          
          
      }
    }
    
    public void Connect( string hostname, int port )
    {
      var state = new ReceiveStateObject();
      Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
      state.WorkSocket = socket;
      
      IPHostEntry ipHostInfo = Dns.Resolve(hostname);
      IPAddress ipAddress = ipHostInfo.AddressList[0];
      IPEndPoint remoteEP = new IPEndPoint( ipAddress, port);
      socket.BeginConnect( remoteEP, ConnectCallback, state);
    }
    
    void ConnectCallback( IAsyncResult result )
    {
      var connectState = result.AsyncState  as ReceiveStateObject;
      connectState.WorkSocket.EndConnect(result);
      Sockets.Add(connectState.WorkSocket);
      
      // Create the state object.
      var recvState = new ReceiveStateObject();
      recvState.WorkSocket = connectState.WorkSocket;
      recvState.WorkSocket.BeginReceive( recvState.buffer, 0, recvState.buffer.Length, 0,
                           new AsyncCallback(WaitMessageCallback), recvState);
      
    }
    
    public void RequestStacks( Socket workSocket, long recency, int count )
    {
      var msg = new RequestStacksNetMessage();
      msg.Recency = recency;
      msg.Count = count;
      Send(workSocket, msg);
    }

    
    public void RequestWads(Socket workSocket, long recency, int count, Guid stackGuid )
    {
      var msg = new RequestWadsNetMessage();
      msg.Recency = recency;
      msg.Count = count;
      msg.StackGuid = stackGuid;
      Send(workSocket, msg);
    }
    
    public void RequestPeers( Socket workSocket, int count )
    {
      var msg = new RequestPeersNetMessage();
      msg.Count = count;
      Send( workSocket, msg);
    }
    
        
    public void SendPeerList(Socket workSocket, Peer[] peers )
    {
      var msg = new PeerListNetMessage();
      msg.Peers = peers;
      Send(workSocket, msg);
    }
    
    public void SendStacks( Socket workSocket, Stack[] stacks )
    {
      var msg = new StacksNetMessage();
      msg.Stacks = stacks;
      Send(workSocket, msg);
    }
    
    public void SendWads( Socket workSocket, FileWad[] wads )
    {
      var msg = new WadsNetMessage();
      msg.Wads = wads;
      Send(workSocket, msg);
    }

    public void Send( Socket workSocket, NetMessage msg )
    {
      var state = new SendState();
      
      state.Message = msg;
      state.Socket = workSocket;
      
      byte[] buffer = msg.ToBytes();
      
      workSocket.BeginSend(buffer, 0, buffer.Length, 0, EndSendCallback, state);
    }

    void EndSendCallback(IAsyncResult ar)
    {
      try
      {
        var state = ar.AsyncState as SendState;
        
        int bytesSent = state.Socket.EndSend(ar);
        Console.WriteLine("Sent {0} bytes to client.", bytesSent);
      }
      catch (Exception e)
      {
        Console.WriteLine(e.ToString());
      }
    }
  }
}
