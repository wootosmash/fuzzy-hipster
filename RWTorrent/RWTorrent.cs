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
using System.Timers;
using FuzzyHipster.Catalog;
using FuzzyHipster.Network;


namespace FuzzyHipster
{
  
  
  public class RWTorrent
  {
    public static RWTorrent Singleton { get; protected set; }
    
    public RWNetwork Network { get; set; }
    public Catalog.Catalog Catalog { get; set; }
    public Settings Settings { get; set; }
    public Peer Me { get; set; }
    public PeerCollection Peers { get; set; }
    public bool IsConnectedToGrid { get { return Network.ActivePeers.Count > 0; } }
    public Random Random { get; set; }

    Timer HeartbeatTimer = new Timer();
    
    public RWTorrent( Catalog.Catalog catalog )
    {
      Singleton = this;
      Random = new Random(DateTime.Now.Millisecond);
      Catalog = catalog;
      Settings = Settings.Load("settings.xml");

      Me = new Peer();
      Me.Port = Settings.Port;
      
      Peers = PeerCollection.Load(Catalog.BasePath);
      Peers.RefreshPeer(Me);
      Peers.ResetConnectionAttempts();

      Network = new RWNetwork();
      
      Network.PeerConnected += NetworkPeerConnected;
      Network.PeerConnectFailed += NetworkPeerConnectFailed;
      
      Network.NewPeer += NetworkNewPeer;
      Network.NewStack += NetworkNewStack;
      Network.NewWad += NetworkNewWad;
      
      Network.PeersRequested += NetworkPeersRequested;
      Network.StacksRequested += NetworkStacksRequested;
      Network.WadsRequested += NetworkWadsRequested;
      
      Network.BlocksAvailableReceived += NetworkBlocksAvailableReceived;
      Network.BlocksAvailableRequested += NetworkBlocksAvailableRequested;
      
      Network.BlockRequested += NetworkBlockRequested;
      Network.BlockReceived += NetworkBlockReceived;
      
      HeartbeatTimer = new Timer(Settings.HeartbeatInterval);
      HeartbeatTimer.Elapsed +=  HeartbeatElapsed;
      HeartbeatTimer.Start();
    }
    
    void NetworkBlocksAvailableReceived( object sender, MessageComposite<BlocksAvailableNetMessage> e)
    {
      var wad = RWTorrent.Singleton.Catalog.GetFileWad(e.Value.FileWadId);
      
      var list = new List<int>();
      
      for( int i=0;i<e.Value.BlocksAvailable.Length;i++)
        if ( e.Value.BlocksAvailable[i] )
          list.Add(i);
      
      int index = list[RWTorrent.Singleton.Random.Next(0,list.Count)];
      
      Network.RequestBlock(e.Peer, wad, index);
    }
    
    void NetworkBlocksAvailableRequested( object sender, MessageComposite<RequestBlocksAvailableNetMessage> e)
    {
      Network.SendBlocksAvailable( e.Peer, Catalog.GetFileWad(e.Value.FileWadId) );
    }    
    
    void NetworkBlockRequested( object sender, BlockRequestedEventArgs e)
    {
      Network.SendBlock( e.Peer, Catalog.GetFileWad(e.FileWadId), e.Block );
    }
    
    void NetworkBlockReceived( object sender, BlockReceivedEventArgs e)
    {
      // do nothing
    }
    
    void NetworkPeerConnected( object sender, GenericEventArgs<Peer> e)
    {
      e.Value.FailedConnectionAttempts = 0;
      Network.SendMyStatus(e.Value, Me);
    }
    
    void NetworkPeerConnectFailed( object sender, GenericEventArgs<Peer> e)
    {
      e.Value.FailedConnectionAttempts++;
      e.Value.NextConnectionAttempt = DateTime.Now.AddSeconds(Math.Pow(Settings.ConnectAttemptWaitTime, e.Value.FailedConnectionAttempts));
    }
    
    void NetworkPeersRequested( object sender, MessageComposite<RequestPeersNetMessage> e )
    {
      var peers = new List<Peer>();
      
      foreach( var peer in Peers.Values )
        peers.Add(peer);
      
      Network.SendPeerList(e.Peer, peers.ToArray());
    }

    void NetworkStacksRequested( object sender, MessageComposite<RequestStacksNetMessage> e )
    {
      Network.SendStacks(e.Peer, Catalog.Stacks.ToArray());
    }

    void NetworkWadsRequested( object sender, MessageComposite<RequestWadsNetMessage> e )
    {
      if ( Catalog.Stacks[e.Value.StackGuid] == null )
        return;
      if ( Catalog.Stacks[e.Value.StackGuid].Wads == null )
        return;
      
      Network.SendWads( e.Peer, Catalog.Stacks[e.Value.StackGuid].Wads.ToArray());
    }

    
    void NetworkNewPeer( object sender, GenericEventArgs<Peer> e)
    {
      // ignore an update to my own peer record
      if ( e.Value.Guid == Me.Guid )
        return;
      if ( e.Value.Guid == Guid.Empty )
        return;
      
      Peers.RefreshPeer(e.Value);
      
      if ( Network.ActivePeers.Count < Settings.MaxActivePeers )
        if ( !Peers[e.Value.Guid].IsConnected )
          if ( e.Value.IPAddress != null && e.Value.Port > 0 )
            Network.Connect(e.Value);
    }
    
    void NetworkNewStack( object sender, GenericEventArgs<Stack> e)
    {
      Catalog.Stacks.RefreshStack(e.Value);
      
      Peer[] peers = Network.ActivePeers.ToArray();
      
      foreach( Peer peer in peers )
        Network.RequestWads( peer, 0, 30, e.Value.Id);
    }
    
    void NetworkNewWad( object sender, GenericEventArgs<FileWad> e)
    {
      Catalog.Stacks[e.Value.StackId].RefreshWad(e.Value);
    }
    
    void HeartbeatElapsed(object sender, ElapsedEventArgs e)
    {
      // talk to connected peers and see if they've got anything for us
      foreach( var peer in Network.ActivePeers )
      {
        Network.RequestPeers(peer, Settings.HeartbeatPeerRequestCount);
        Network.RequestStacks(peer, Catalog.LastUpdated, Settings.HeartbeatStackRequestCount);
      }
      
      // see if we need some new peers
      if ( Network.ActivePeers.Count < Settings.MaxActivePeers )
      {
        foreach( var peer in Peers.Values ) // for each peer that we're not connected to 
          if ( !peer.IsConnected ) // we're not connected
            if ( peer.NextConnectionAttempt < DateTime.Now ) // wait is over
              Network.Connect(peer);
      }
    }
    
    public void Start()
    {
      Console.Write("Starting " + Settings.Port);
      Network.StartListening(Settings.Port);
    }
  }

}

