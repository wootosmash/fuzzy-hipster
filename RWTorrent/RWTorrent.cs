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
      Me = new Peer();
      Peers = new PeerCollection();
      Peers.Add(Me);
      Settings = Settings.Load("settings.xml");
      Network = new RWNetwork();
      
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
    
    void NetworkPeersRequested( object sender, GenericEventArgs<RequestPeersNetMessage> e )
    {
      
    }

    void NetworkStacksRequested( object sender, GenericEventArgs<RequestStacksNetMessage> e )
    {
      
    }

    void NetworkWadsRequested( object sender, GenericEventArgs<RequestWadsNetMessage> e )
    {
      
    }

    
    void NetworkNewPeer( object sender, GenericEventArgs<Peer> e) {
      
      // ignore an update to my own peer record
      if ( e.Value.Guid == Me.Guid )
        return;
      
      Peers.RefreshPeer(e.Value);
    }
    
    void NetworkNewStack( object sender, GenericEventArgs<Stack> e) {
      Catalog.Stacks.RefreshStack(e.Value);
    }
    
    void NetworkNewWad( object sender, GenericEventArgs<FileWad> e){
      Catalog.Stacks[e.Value.StackId].RefreshWad(e.Value);
    }
    
    void HeartbeatElapsed(object sender, ElapsedEventArgs e)
    {
      foreach( var socket in Network.Sockets )
      {
        Network.RequestPeers(socket, Settings.HeartbeatPeerRequestCount);
        Network.RequestStacks(socket, Catalog.LastUpdated, Settings.HeartbeatStackRequestCount);
      }
    }
    
    public void Start()
    {
      Network.StartListening();
    }
  }

}

