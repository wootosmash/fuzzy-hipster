/*
 * Created by SharpDevelop.
 * User: Al
 * Date: 23/01/2015
 * Time: 6:00 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using FuzzyHipster.Catalog;
using FuzzyHipster.Network;

namespace FuzzyHipster.Strategy
{
  public class StreamingBlockAquisitionStrategy : MoustacheStrategy
  {
    public FileWad FileWad { get; set; }
    public FileDescriptor File { get; set; }
    public bool TransferStarted { get; set; }
    
    
    public StreamingBlockAquisitionStrategy()
    {
      TransferStarted = false;
    }
    
    public override void Install()
    {
      Catalog.NotifyBlockIndexItem += CatalogNotifyBlockIndexItem;;
    }
    
    public override void Uninstall()
    {
      Catalog.NotifyBlockIndexItem -= CatalogNotifyBlockIndexItem;
    }
    
    public override void Think()
    {
      if ( BlockAvailability.Count > 0 && !TransferStarted )
      {
        Console.WriteLine("STRATEGY: StreamingBlockStrategy.Think()");

        var vector = BlockAvailability.GetBlockPeers(FileWad, BlockAvailabilityList.SearchStrategy.FirstBlock, File.StartBlock, File.EndBlock);
        if ( vector == null ) return;
        Network.RequestBlock(vector.Peers.GetRandom(), FileWad, vector.Block);

        vector = BlockAvailability.GetBlockPeers(FileWad, BlockAvailabilityList.SearchStrategy.LastBlock, File.StartBlock, File.EndBlock);
        if ( vector == null ) return;
        Network.RequestBlock(vector.Peers.GetRandom(), FileWad, vector.Block);

        TransferStarted = true;
      }
      
    }
    
    public void Setup( FileWad wad, FileDescriptor file )
    {
      FileWad = wad;
      File = file;

      Console.WriteLine(string.Format("STRATEGY: Streaming Mode: {0}, File {1}", wad, file));
      
      file.AllocateFile();
      
      if ( BlockAvailability.Count == 0 )
        foreach (var peer in Network.ActivePeers.ToArray())
          Network.RequestBlocksAvailable(peer, FileWad);
    }

    void CatalogNotifyBlockIndexItem(object sender, NotifyBlockIndexItemEventArgs e)
    {
      if ( FileWad == null ) return;
      if ( File == null ) return;
      
      File.WriteBlock(FileWad.GetBlockPath(e.Block), e.Block);
      
      var vector = BlockAvailability.GetBlockPeers(FileWad, BlockAvailabilityList.SearchStrategy.NextBlock, File.StartBlock, File.EndBlock);
      
      if ( !vector.IsValid )
        TransferStarted = false;
      else
        Network.RequestBlock(vector.Peers.GetRandom(), FileWad, vector.Block);
    }
    
  }
}
