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
using System.Linq;
using System.Timers;
using FuzzyHipster.Catalog;
using FuzzyHipster.Network;
namespace FuzzyHipster
{
  public class BasicBlockAquisitionStrategy : MoustacheStrategy
  {
    public const int BlockAvailabilityTimeout = 60 * 60; // seconds
    
    BlockAvailabilityList BlockAvailability { get; set; }
    
    
    public override void Install()
    {
      Network.BlockReceived += NetworkBlockReceived;
      Network.BlocksAvailableReceived += NetworkBlocksAvailableReceived;
      Catalog.NotifyFileWad += CatalogNotifyFileWad;
    }

    public override void Uninstall()
    {
      Network.BlockReceived -= NetworkBlockReceived;
      Network.BlocksAvailableReceived -= NetworkBlocksAvailableReceived;
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
        
        foreach( var wad in channel.Wads )
        {
          if ( wad.IsFullyDownloaded )
            continue;
          
          if ( WeDontKnowWhatsAvailable(wad))
          {
            foreach (var peer in Peers.ToArray())
              Network.RequestBlocksAvailable(peer, channel.Wads[0]);
          }
          else
          {
            KeyValuePair<int, Peer[]> blockPeer = BlockAvailability.GetRandomBlockPeer(wad, BlockAvailability, BlockAvailabilityList.SearchStrategy.RareBlocks);
            Network.RequestBlock(blockPeer.Value[MoustacheLayer.Singleton.Random.Next(0,blockPeer.Value.Length)], wad, blockPeer.Key);
          }
        }
      }
    }
    
    public BasicBlockAquisitionStrategy()
    {
      BlockAvailability = new BlockAvailabilityList();
    }
    
    public bool WeDontKnowWhatsAvailable(FileWad wad)
    {
      DateTime timeout = DateTime.Now.AddSeconds(-BlockAvailabilityTimeout);
      
      if ( !BlockAvailability.ContainsKey(wad.Id))
        return false;
      
      foreach( var matrix in BlockAvailability[wad.Id] )
        if ( matrix.LastUpdated < timeout )
          return false;
      
      return true;
    }

    void NetworkBlocksAvailableReceived(object sender, MessageComposite<BlocksAvailableNetMessage> e)
    {
      var wad = MoustacheLayer.Singleton.Catalog.GetFileWad(e.Value.FileWadId);
      
      List<BlockAvailabilityMatrix> availabilityList = null;
      
      if ( !BlockAvailability.ContainsKey(wad.Id))
      {
        availabilityList = new List<BlockAvailabilityMatrix>();
        BlockAvailability.Add(wad.Id, availabilityList);
      }
      else
        availabilityList = BlockAvailability[wad.Id];
      
      var matrix = availabilityList.Find(x => x.Peer == e.Peer );
      
      if ( matrix != null )
      {
        matrix.BlockAvailability = e.Value.BlocksAvailable;
        matrix.LastUpdated = DateTime.Now;
      }
      else
      {
        matrix = new BlockAvailabilityMatrix()
        {
          BlockAvailability = e.Value.BlocksAvailable,
          Peer = e.Peer,
          LastUpdated = DateTime.Now
        };
        availabilityList.Add(matrix);
      }
    }

    void NetworkBlockReceived(object sender, BlockReceivedEventArgs e)
    {
      FileWad wad = Catalog.GetFileWad(e.FileWadId);
      if (wad.IsFullyDownloaded)
        wad.SaveFromBlocks(Catalog.BasePath + @"\Files\");
    }
    
    void CatalogNotifyFileWad(object sender, GenericEventArgs<FileWad> e)
    {
      if ( WeDontKnowWhatsAvailable(e.Value))
        foreach (var peer in Network.ActivePeers)
          Network.RequestBlocksAvailable(peer, e.Value);
    }
  }
}



