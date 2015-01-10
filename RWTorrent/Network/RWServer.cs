

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
  public class RWServerStateObject {
    // Client  socket.
    public Socket WorkSocket = null;
    // Size of receive buffer.
    public const int BufferSize = 1024;
    // Receive buffer.
    public byte[] buffer = new byte[BufferSize];
    // Received data string.
    public StringBuilder sb = new StringBuilder();
  }

  public class RWServer
  {
    public const int RWServerPort = 7892;
    
    public Peer Me { get; set; }
    public SortedList<Guid, Peer> Peers { get; set; }
    public List<Socket> Sockets { get; set; }
    public IPEndPoint LocalEndPoint { get; set; }
    public Socket Listener { get; set; }
    
    Random random = new Random(DateTime.Now.Millisecond);
    ManualResetEvent allDone = new ManualResetEvent(false);
    
    public RWServer()
    {
      Me = new Peer();
      Peers = new SortedList<Guid, Peer>();
      Peers.Add(Me.Guid, Me);
      Sockets = new List<Socket>();
    }

    public void StartListening()
    {
      // Data buffer for incoming data.
      byte[] bytes = new Byte[1024];

      LocalEndPoint = new IPEndPoint(IPAddress.Any, RWServerPort);
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
      var state = new RWServerStateObject();
      state.WorkSocket = handler;
      handler.BeginReceive( state.buffer, 0, state.buffer.Length, 0,
                           new AsyncCallback(WaitMessageCallback), state);
    }

    void WaitMessageCallback(IAsyncResult ar)
    {
      var state =  ar.AsyncState as RWServerStateObject;
      Socket handler = state.WorkSocket;

      int bytesRead = handler.EndReceive(ar);
      
      if ( bytesRead > 0 )
      {
        Console.WriteLine("WaitMessage Recev");

        NetMessage message = NetMessage.FromBytes(state.buffer);
        
        ProcessMessage(message, state);
      }
    }
    
    void ProcessMessage(NetMessage msg, RWServerStateObject state)
    {
      Console.WriteLine("MSG: {0}", msg);
      
      switch( msg.Type )
      {
        case MessageType.Hello:
          break;
          
        case MessageType.PeerStatus:
        case MessageType.PeerList:
          var status = msg as PeerListNetMessage;
          
          if ( Peers.ContainsKey(status.Peers[0].Guid))
            Peers.Remove(status.Peers[0].Guid);
          
          Peers.Add(status.Peers[0].Guid, status.Peers[0]);
          break;
          
        case MessageType.RequestPeers:
          var request = msg as RequestPeersNetMessage;
          SendPeerList(state.WorkSocket, request.Count);
          break;
          
        case MessageType.RequestStacks:
          var stacks = msg as RequestStacksNetMessage;
          SendStacks( state.WorkSocket, stacks.Recency, stacks.Count );
          break;
          
        case MessageType.RequestWads:
          var wads = msg as RequestWadsNetMessage;
          SendWads( state.WorkSocket, wads.Recency, wads.Count, wads.StackGuid );
          break;
          
      }
    }
    
    public void Connect( string hostname, int port )
    {
      var state = new RWServerStateObject();
      Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
      state.WorkSocket = socket;
      
      IPHostEntry ipHostInfo = Dns.Resolve(hostname);
      IPAddress ipAddress = ipHostInfo.AddressList[0];
      IPEndPoint remoteEP = new IPEndPoint( ipAddress, port);
      socket.BeginConnect( remoteEP, ConnectCallback, state);
    }
    
    void ConnectCallback( IAsyncResult result )
    {
      var state = result.AsyncState  as RWServerStateObject;
      state.WorkSocket.EndConnect(result);
      Sockets.Add(state.WorkSocket);
      
      SendMyStatus( state.WorkSocket );
    }
    
    public void RequestStacks( Socket workSocket, long recency, int count )
    {
      var msg = new RequestStacksNetMessage();
      msg.Recency = recency;
      msg.Count = count;
      Send(workSocket, msg);      
    }

    public void SendStacks( Socket workSocket, long recency, int count )
    {
      var msg = new StacksNetMessage();
      msg.Stacks = RWTorrent.Singleton.Catalog.Stacks.ToArray();      
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
    
    public void SendWads( Socket workSocket, long recency, int count, Guid stackGuid )
    {
      if ( RWTorrent.Singleton.Catalog.Stacks[stackGuid] == null )
        return;

      var wads = new List<FileWad>();
      var msg = new WadsNetMessage();
      msg.Wads = RWTorrent.Singleton.Catalog.Stacks[stackGuid].GetWadsByRecency( recency, count );
      
      Send(workSocket, msg);
    }
    
    public void SendMyStatus(Socket workSocket )
    {
      var msg = new PeerListNetMessage();

      msg.Peers = new Peer[]{ Me };
      
      Send(workSocket, msg);
      
    }
    
    public void SendPeerList(Socket workSocket, int count)
    {
      var peers = new List<Peer>();
      var msg = new PeerListNetMessage();

      if ( Peers.Count < count )
      {
        foreach( Peer p in Peers.Values )
          peers.Add(p);
      }
      else
      {
        int randomPeers = count / 2;
        int seqentialPeers = count / 2;
        
        for ( int i=0;i<randomPeers;i++)
          peers.Add(Peers.Values[random.Next(0, Peers.Count)]);
        
        for ( int i=0;i<seqentialPeers;i++)
          peers.Add(Peers.Values[i]);
        
      }
      
      msg.Peers = peers.ToArray();
      
      Send(workSocket, msg);
    }
    
    public void RequestPeers( Socket workSocket, int count )
    {
      var msg = new RequestPeersNetMessage();
      Send( workSocket, msg);
    }
    
    public void SendPeerStatus( Socket workSocket, Peer peer )
    {
      var msg = new PeerListNetMessage();
      msg.Peers = new Peer[] {peer};
      Send( workSocket, msg);
    }
    
    
    
    public void Send( Socket workSocket, NetMessage msg )
    {
      var state = new SendState();
      
      state.Message = msg;
      
      byte[] buffer = msg.ToBytes();
      
      workSocket.BeginSend(buffer, 0, buffer.Length, 0, EndSendCallback, state);
    }

    void EndSendCallback(IAsyncResult ar)
    {
      try
      {
        var handler = ar.AsyncState as Socket;

        int bytesSent = handler.EndSend(ar);
        Console.WriteLine("Sent {0} bytes to client.", bytesSent);
      }
      catch (Exception e)
      {
        Console.WriteLine(e.ToString());
      }
    }
  }
}
