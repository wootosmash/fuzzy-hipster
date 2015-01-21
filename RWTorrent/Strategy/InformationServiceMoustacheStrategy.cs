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
namespace FuzzyHipster.Strategy
{
  public class InformationServiceMoustacheStrategy : MoustacheStrategy
  {
    public override void Install()
    {
      Network.PeersRequested += NetworkPeersRequested;
      Network.ChannelsRequested += NetworkChannelsRequested;
      Network.WadsRequested += NetworkWadsRequested;
      Network.BlocksAvailableRequested += NetworkBlocksAvailableRequested;
      Network.BlockRequested += NetworkBlockRequested;
    }

    public override void Uninstall()
    {
      Network.PeersRequested -= NetworkPeersRequested;
      Network.ChannelsRequested -= NetworkChannelsRequested;
      Network.WadsRequested -= NetworkWadsRequested;
      Network.BlocksAvailableRequested -= NetworkBlocksAvailableRequested;
      Network.BlockRequested -= NetworkBlockRequested;
    }

    public override void Think()
    {
    }

    void NetworkPeersRequested(object sender, MessageComposite<RequestPeersNetMessage> e)
    {
      var peers = new List<Peer>();
      foreach (var peer in Peers)
        peers.Add(peer);
      Network.SendPeerList(peers.ToArray(), e.Peer);
    }

    void NetworkChannelsRequested(object sender, MessageComposite<RequestChannelsNetMessage> e)
    {
      List<Channel> list = new List<Channel>();
      
      if ( e.Value.Guids != null && e.Value.Guids.Length > 0 )
      {
        for ( int i=0;i<e.Value.Guids.Length;i++)
        {
          var chan = Catalog.Channels.Find(x => x.Id == e.Value.Guids[i]);
          if ( chan != null )
            list.Add(chan);
        }
      }
      else
        for ( int i=0;i<e.Value.Count;i++ )
          list.Add(Catalog.Channels.GetRandom());
      
      Network.SendChannels(list.ToArray(), e.Peer);
    }

    void NetworkWadsRequested(object sender, MessageComposite<RequestWadsNetMessage> e)
    {
      // send a random list
      if (e.Value.ChannelGuid == Guid.Empty )
      {
        var list = new List<FileWad>();
       
        for ( int i=0;i<e.Value.Count;i++)
          list.Add(Catalog.FileWads.GetRandom());
        
        Network.SendWads(list.ToArray(), e.Peer);
      }
      
      // if we dont have the channel request a copy!
      if (Catalog.Channels[e.Value.ChannelGuid] == null)
        Network.RequestChannel( e.Peer, e.Value.ChannelGuid);
      else if (Catalog.Channels[e.Value.ChannelGuid].Wads == null)
        Network.SendWads(null, e.Peer);
      else
        Network.SendWads(Catalog.Channels[e.Value.ChannelGuid].Wads.ToArray(), e.Peer);
    }

    void NetworkBlocksAvailableRequested(object sender, MessageComposite<RequestBlocksAvailableNetMessage> e)
    {
      var wad = Catalog.GetFileWad(e.Value.FileWadId);
      if (wad == null)
        return;
      Network.SendBlocksAvailable(e.Peer, wad);
    }

    void NetworkBlockRequested(object sender, BlockRequestedEventArgs e)
    {
      Network.SendBlock(Catalog.GetFileWad(e.FileWadId), e.Block, e.Peer);
    }
  }
}



