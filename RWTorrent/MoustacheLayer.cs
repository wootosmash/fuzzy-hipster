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
using FuzzyHipster.Strategy;


namespace FuzzyHipster
{
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

      Network = new RWNetwork(Me);
      
      // respond to requests
      Strategies.Add( new InformationServiceMoustacheStrategy());
      
      // get new channels and updates to existing ones
      Strategies.Add( new CatalogManagementMoustacheStrategy());
      
      // get files
      Strategies.Add( new BasicBlockAquisitionStrategy());
      
      // get peers and connect to new ones
      Strategies.Add( new BasicPeerManagementMoustacheStrategy());
      
      // send out our status
      Strategies.Add( new KeepAliveMoustacheStrategy());
            
      foreach( MoustacheStrategy strategy in Strategies )
        strategy.Enable();
      
      HeartbeatTimer = new Timer(Settings.HeartbeatInterval);
      HeartbeatTimer.Elapsed +=  HeartbeatElapsed;
      HeartbeatTimer.Start();
    }
    
    
    
    
    
    void HeartbeatElapsed(object sender, ElapsedEventArgs e)
    {
      Think();
    }
    
    public void Think()
    {
      try
      {
        foreach( var strategy in Strategies )
          if ( strategy.Enabled )
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

