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
  public abstract class MoustacheAction
  {
    public DateTime NextThink { get; set; }
    public abstract void Think();
  }
  
  public class KeepAliveMoustacheAction : MoustacheAction
  {
    public override void Think()
    {
      MoustacheLayer.Singleton.Network.SendMyStatus(MoustacheLayer.Singleton.Network.ActivePeers.ToArray());
      NextThink = DateTime.Now.AddMilliseconds(MoustacheLayer.Singleton.Settings.KeepAliveInterval);
    }
  }
  // MoustacheAction
  //   NextThink
  //   Execute()
  
  // KeepAliveMoustacheAction
  // RequestCatalogItems
  // 

  
  
  
  
  
  
  
  
  public class MoustacheLayer
  {
    public static MoustacheLayer Singleton { get; protected set; }
    
    public RWNetwork Network { get; set; }
    public Catalog.Catalog Catalog { get; set; }
    public Settings Settings { get; set; }
    public Peer Me { get; set; }
    public PeerCollection Peers { get; set; }
    public bool IsConnectedToGrid { get { return Network.ActivePeers.Count > 0; } }
    public Random Random { get; set; }
    
    public List<MoustacheStrategy> Strategies { get; set; }

    Timer HeartbeatTimer = new Timer();

    public Timer HeartBeat { get{ return HeartbeatTimer; }}
    
    public MoustacheLayer( Catalog.Catalog catalog )
    {
      Singleton = this;
      Random = new Random(DateTime.Now.Millisecond);
      Catalog = catalog;
      Settings = Settings.Load("settings.xml");
      Strategies = new List<MoustacheStrategy>();

      Me = new Peer();
      Me.Port = Settings.Port;
      Me.Id = catalog.Id;
      Me.Name = Environment.MachineName;
      
      Peers = PeerCollection.Load(Catalog.BasePath);
      Peers.ResetConnectionAttempts();

      Network = new RWNetwork(Me);
      
      Network.PeerConnected += NetworkPeerConnected;
      Network.PeerConnectFailed += NetworkPeerConnectFailed;
      
      Network.NewPeer += NetworkNewPeer;
      
      Strategies.Add( new InformationServiceMoustacheStrategy());
      Strategies.Add( new CatalogManagementMoustacheStrategy());
      Strategies.Add( new BasicBlockAquisitionStrategy());
      
      foreach( MoustacheStrategy strategy in Strategies )
        strategy.Install();
      
      HeartbeatTimer = new Timer(Settings.HeartbeatInterval);
      HeartbeatTimer.Elapsed +=  HeartbeatElapsed;
      HeartbeatTimer.Start();
    }
    
    
    
    void NetworkPeerConnected( object sender, GenericEventArgs<Peer> e)
    {
      e.Value.FailedConnectionAttempts = 0;
    }
    
    void NetworkPeerConnectFailed( object sender, GenericEventArgs<Peer> e)
    {
      // when a peer fails to connect we use an exponential backoff algorithm to connect next time
      e.Value.FailedConnectionAttempts++;
      e.Value.NextConnectionAttempt = DateTime.Now.AddSeconds(Settings.ConnectAttemptWaitTime * Math.Pow(2, e.Value.FailedConnectionAttempts));
    }
    

    
    void NetworkNewPeer( object sender, GenericEventArgs<Peer> e)
    {
      // ignore an update to my own peer record
      if ( e.Value.Id == Me.Id )
        return;
      if ( e.Value.Id == Guid.Empty )
        return;
      
      Peers.RefreshPeer(e.Value);
      
      if ( Network.ActivePeers.Count < Settings.MaxActivePeers )
        if ( !Peers.Find(x => x.Id == e.Value.Id).IsConnected )
          if ( e.Value.IPAddress != null && e.Value.Port > 0 )
            Network.Connect(e.Value);
    }
    
    
    void HeartbeatElapsed(object sender, ElapsedEventArgs e)
    {
      Think();
    }
    
    public void Think()
    {
      try
      {
        // talk to connected peers and see if they've got anything for us
        var peers = Network.ActivePeers.ToArray();
        foreach( var peer in peers )
        {
          if ( peer.OkToSendAt <  DateTime.Now )
          {
            Network.SendMyStatus( peer );
            //            Network.RequestPeers(peer, Settings.HeartbeatPeerRequestCount);
            Network.RequestChannels(peer, Catalog.LastUpdated, Settings.HeartbeatChannelRequestCount);
          }
          else
            Console.WriteLine("NOT OK TO SEND: " + peer.OkToSendAt);
        }
        
        // see if we need some new peers
        if ( Network.ActivePeers.Count < Settings.MaxActivePeers )
        {
          foreach( var peer in Peers.ToArray() ) // for each peer that we're not connected to
            if ( !peer.IsConnected ) // we're not connected
              if ( peer.NextConnectionAttempt < DateTime.Now ) // wait is over
                Network.Connect(peer);
        }
        
        foreach( var strategy in Strategies )
          strategy.Think();
      }
      catch( Exception ex )
      {
        Console.WriteLine(ex);
      }
    }
    
    public void Start()
    {
      Network.StartListening(Settings.Port);
    }
  }

}

