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
using RWTorrent.Catalog;
using RWTorrent.Network;

namespace RWTorrent
{
  
  
  public class RWTorrent
  {
    public static RWTorrent Singleton { get; protected set; }
    
    public RWNetwork Network { get; set; }
    public Catalog.Catalog Catalog { get; set; }
    public Settings Settings { get; set; }
    public Peer Me { get; set; }
    public PeerCollection Peers { get; set; }

    Timer HeartbeatTimer = new Timer();
    
    public RWTorrent( Catalog.Catalog catalog )
    {
      Settings = Settings.Load("settings.xml");

      Me = new Peer();
      
      Me.IPAddress = IPSniffer.GetPublicIP();
      Me.Port = Settings.Port;
      
      Peers = new PeerCollection();
      Peers.Add(Me);

      Network = new RWNetwork();
      
      Network.PeerConnected += NetworkPeerConnected;
      
      Network.NewPeer += NetworkNewPeer;
      Network.NewStack += NetworkNewStack;
      Network.NewWad += NetworkNewWad;
      
      Network.PeersRequested += NetworkPeersRequested;
      Network.StacksRequested += NetworkStacksRequested;
      Network.WadsRequested += NetworkWadsRequested;
      
      Singleton = this;
      Catalog = catalog;
      HeartbeatTimer = new Timer(Settings.HeartbeatInterval);
      HeartbeatTimer.Elapsed +=  HeartbeatElapsed;
      HeartbeatTimer.Start();
    }
    
    void NetworkPeerConnected( object sender, GenericEventArgs<Peer> e)
    {
      Network.SendMyStatus(e.Value, Me);
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
      
      Network.SendWads( e.Peer, Catalog.Stacks[e.Value.StackGuid].Wads.ToArray());
    }

    
    void NetworkNewPeer( object sender, GenericEventArgs<Peer> e)
    {
      // ignore an update to my own peer record
      if ( e.Value.Guid == Me.Guid )
        return;
      
      Peers.RefreshPeer(e.Value);
      
      if ( e.Value.IPAddress != null && e.Value.Port > 0 )
      {
        if ( Network.ActivePeers.Count < Settings.MaxActivePeers )
        {
          Network.Connect(e.Value);
        }
      }
    }
    
    void NetworkNewStack( object sender, GenericEventArgs<Stack> e)
    {
      Catalog.Stacks.RefreshStack(e.Value);
      
      foreach( Peer peer in Network.ActivePeers )
        Network.RequestWads( peer, 0, 30, e.Value.Id);
    }
    
    void NetworkNewWad( object sender, GenericEventArgs<FileWad> e)
    {
      Catalog.Stacks[e.Value.StackId].RefreshWad(e.Value);
    }
    
    void HeartbeatElapsed(object sender, ElapsedEventArgs e)
    {
      foreach( var peer in Network.ActivePeers )
      {
        Network.RequestPeers(peer, Settings.HeartbeatPeerRequestCount);
        Network.RequestStacks(peer, Catalog.LastUpdated, Settings.HeartbeatStackRequestCount);
      }
    }
    
    public void Start()
    {
      Network.StartListening(Settings.Port);
    }
  }

}

