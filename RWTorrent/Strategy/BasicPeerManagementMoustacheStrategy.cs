/*
 * Created by SharpDevelop.
 * User: Al
 * Date: 20/01/2015
 * Time: 5:04 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Net.Sockets;

namespace FuzzyHipster.Strategy
{
  /// <summary>
  /// Description of BasicPeerManagementMoustacheStrategy.
  /// </summary>
  public class BasicPeerManagementMoustacheStrategy : MoustacheStrategy
  {
    public BasicPeerManagementMoustacheStrategy()
    {
    }

    #region implemented abstract members of MoustacheStrategy

    public override void Install()
    {
      Network.PeerConnected += NetworkPeerConnected;
      Network.PeerConnectFailed += NetworkPeerConnectFailed;
      Network.NewPeer += NetworkNewPeer;
      Peers.ResetConnectionAttempts();
    }

    public override void Uninstall()
    {
      Network.PeerConnected -= NetworkPeerConnected;
      Network.PeerConnectFailed -= NetworkPeerConnectFailed;
      Network.NewPeer -= NetworkNewPeer;
    }

    public override void Think()
    {
      int connects = Network.ActivePeers.Count;
      // see if we need some new peers
      if ( connects < Settings.MaxActivePeers )
      {
        foreach( var peer in Peers.ToArray() ) // for each peer that we're not connected to
        {
          if ( peer.Enabled && !peer.IsConnected && peer.NextConnectionAttempt < DateTime.Now && connects < Settings.MaxActivePeers )
          {
            connects++;
            Network.Connect(peer);
          }
        }
      }
      
      if ( Peers.Count < Settings.DesiredPeerListSize && Peers.Count < Peers.MaximumPeerListSize )
      {
        int peersToSend = Settings.DesiredPeerListSize - Peers.Count;
        foreach( Peer p in Network.ActivePeers.ToArray())
        {
          Network.RequestPeers(p, peersToSend );
        }
      }
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
      if ( e.Value.Id == MoustacheLayer.Singleton.Me.Id )
        return;
      if ( e.Value.Id == Guid.Empty )
        return;
      
      Peers.RefreshPeer(e.Value);
      Console.WriteLine("NEWPEER: " + e.Value);
    }
    

    #endregion
  }
}
