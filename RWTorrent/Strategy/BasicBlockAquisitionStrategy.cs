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
using System.Diagnostics;
using System.Linq;
using System.Timers;
using FuzzyHipster.Catalog;
using FuzzyHipster.Network;

namespace FuzzyHipster.Strategy
{
  public class BasicBlockAquisitionStrategy : MoustacheStrategy
  {
    public override void Install()
    {
      Network.BlockReceived += NetworkBlockReceived;
      Network.BlockTransferFailed += NetworkBlockTransferFailed;
      Network.BlockTransferStarted += NetworkBlockTransferStarted;
      Catalog.NotifyFileWad += CatalogNotifyFileWad;
    }

    public override void Uninstall()
    {
      Network.BlockReceived -= NetworkBlockReceived;
      Network.BlockTransferFailed -= NetworkBlockTransferFailed;
      Network.BlockTransferStarted -= NetworkBlockTransferStarted;
      Catalog.NotifyFileWad -= CatalogNotifyFileWad;
    }

    public override void Think()
    {
      // get all the subscribed channels and find out all the block availabilities
      if ( Network.InProgressTransfers.Count >= Settings.MaxActiveBlockTransfers )
        return;
      
      foreach (var channel in Catalog.Channels.Find(SearchFilter.PartiallyDownloaded))
      {
        if (!channel.Subscribed)
          continue;
        
        if ( channel.Wads == null )
          continue;
        
        foreach( var wad in channel.Wads )
        {
          if ( wad.IsFullyDownloaded )
            continue;
          
          if ( BlockAvailability.WeDontKnowWhatsAvailable(wad))
          {
            Console.WriteLine("We don't know whats available for {0}", wad);
            foreach (var peer in Network.ActivePeers.ToArray())
              Network.RequestBlocksAvailable(peer, channel.Wads[0]);
          }
          else
          {
            Console.WriteLine("Looking around for blocks");
            BlockVector vector = BlockAvailability.GetBlockPeers(wad, BlockAvailabilityList.SearchStrategy.RandomRareBlock);
            
            if ( vector == null || !vector.IsValid )
            {
              Console.WriteLine("No blocks or no peers");
              continue; // skip this WAD, no block or no peers
            }
            
            Network.RequestBlock(vector.Peers.GetRandom(), wad, vector.Block);
          }
        }
      }
    }
    
    public BasicBlockAquisitionStrategy()
    {
    }
    
    void NetworkBlockReceived(object sender, BlockReceivedEventArgs e)
    {
      Catalog.AddBlock(e.FileWad, e.Block);
    }
    
    void NetworkBlockTransferFailed(object sender, BlockTransferStartedEventArgs e)
    {
      Console.WriteLine("NET: Transfer Fail " + e.FileWad.Id + " " + e.Block);
      // kills the availability matrix for a peer
      BlockAvailability.Invalidate(e.Peer);
    }

    void NetworkBlockTransferStarted(object sender, BlockTransferStartedEventArgs e)
    {
      Catalog.AddBlock(e.FileWad, e.Block);
    }
    
    void CatalogNotifyFileWad(object sender, GenericEventArgs<FileWad> e)
    {
      if ( BlockAvailability.WeDontKnowWhatsAvailable(e.Value))
        foreach (var peer in Network.ActivePeers.ToArray())
          Network.RequestBlocksAvailable(peer, e.Value);
      
    }
  }
}



