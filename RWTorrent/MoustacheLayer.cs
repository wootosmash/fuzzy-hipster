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
  public class StrategyManager
  {
    List<Record> list = new List<Record>();
    
    public MoustacheStrategy Find( Type type )
    {
      foreach( var record in list )
        if ( type.IsInstanceOfType(record.Strategy) )
          return record.Strategy;
      
      return null;
    }
    
    public void Add( MoustacheStrategy strategy, bool enabledOnStartup )
    {
      list.Add(new Record() {Strategy = strategy, EnabledOnStartup = enabledOnStartup});
    }
    
    public void Startup()
    {
      foreach( var record in list )
        if ( record.EnabledOnStartup )
          record.Strategy.Enable();
      
    }
    
    public void Enable( Type type )
    {
      var strategy = Find(type);
      strategy.Enable();
    }
    
    public void Disable( Type type )
    {
      var strategy = Find(type);
      strategy.Disable();
    }
    
    public void Think()
    {
      foreach( var m in list )
        if ( m.Strategy.Enabled )
          m.Strategy.Think();
    }
    
    public class Record
    {
      public MoustacheStrategy Strategy;
      public bool EnabledOnStartup;
    }
  }
  
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
    public BlockAvailabilityList BlockAvailability { get; set; }
    
    public StrategyManager Strategies { get; set; }

    Timer HeartbeatTimer = new Timer();

    public Timer HeartBeat { get{ return HeartbeatTimer; }}
    
    public MoustacheLayer( Catalog.Catalog catalog )
    {
      Singleton = this;
      Random = new Random(DateTime.Now.Millisecond);
      Catalog = catalog;
      Settings = Settings.Load("settings.xml");
      Strategies = new StrategyManager();
      BlockAvailability = new BlockAvailabilityList();

      Me = new Peer();
      Me.Port = Settings.Port;
      Me.Id = catalog.Id;
      Me.Name = Environment.MachineName;
      
      Peers = PeerCollection.Load(Catalog.BasePath);

      Network = new RWNetwork(Me);
      
      // respond to requests
      Strategies.Add( new InformationServiceMoustacheStrategy(), true);
      
      // keep availability of blocks
      Strategies.Add( new BlockAvailabilityStrategy(), true);
      
      // get new channels and updates to existing ones
      Strategies.Add( new CatalogManagementMoustacheStrategy(), true);
      
      // get files
      Strategies.Add( new BasicBlockAquisitionStrategy(), true);
      
      // get peers and connect to new ones
      Strategies.Add( new BasicPeerManagementMoustacheStrategy(), true);
      
      // send out our status
      Strategies.Add( new KeepAliveMoustacheStrategy(), true);
      
      // streaming blocks strategy - shut down initially
      Strategies.Add( new StreamingBlockAquisitionStrategy(), false);
      
      Strategies.Startup();
      
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
        Strategies.Think();
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

