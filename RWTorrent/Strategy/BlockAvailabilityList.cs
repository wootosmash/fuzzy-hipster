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
  public class BlockVector
  {
    public int Block;
    public PeerCollection Peers;
    
    public bool IsValid {
      get {
        return Block >= 0 && Peers != null && Peers.Count > 0;
      }
    }
    
    public BlockVector( int block, PeerCollection peers )
    {
      Block = block;
      Peers = peers;
    }
  }
  
  public class BlockAvailabilityList : SortedList<Guid, List<BlockAvailabilityMatrix>>
  {
    public const int BlockAvailabilityTimeout = 60 * 60; // seconds
    
    public enum SearchStrategy
    {
      RandomRareBlock,
      RandomBlock,
      RandomCommonBlock,
      FirstBlock,
      LastBlock,
      NextBlock
    }
    
    /// <summary>
    /// Gets a list of peers with the block
    /// </summary>
    /// <param name="wad"></param>
    /// <param name="block"></param>
    /// <returns></returns>
    public BlockVector GetBlockPeers( FileWad wad, int block )
    {
      Console.WriteLine("GetBlockPeers(" + block + ")");
      var peers = new PeerCollection();
      
      if ( block >= 0 )
        if ( ContainsKey(wad.Id))
          foreach( var matrix in this[wad.Id] )
            if ( matrix.BlockAvailability.Length > block )
              if ( matrix.BlockAvailability[block] )
                peers.Add(matrix.Peer);
      
      return new BlockVector(block, peers);
    }
    
    public BlockVector GetBlockPeers( FileWad wad, SearchStrategy strategy )
    {
      return GetBlockPeers( wad, strategy, 0, wad.TotalBlocks-1 );
    }

    /// <summary>
    /// Gets a random block and list of peers with the block based on the list of available blocks and the desired strategy
    /// </summary>
    /// <param name="wad"></param>
    /// <param name="strategy"></param>
    /// <param name = "minBlock"></param>
    /// <param name = "maxBlock"></param>
    /// <returns></returns>
    public BlockVector GetBlockPeers(FileWad wad, SearchStrategy strategy, int minBlock, int maxBlock)
    {
      const int CommonBlockThreshold = 80;
      const int RareBlockThreshold = 20;
      const int MinPercentile = 0;
      const int MaxPercentile = 100;
      BlockVector vector;
      
      bool[] avail = wad.GetBlockAvailability();
      
      if ( !ContainsKey(wad.Id))
        return new BlockVector(-1,new PeerCollection());
      
      if ( minBlock >= 0 )
        for ( int i=0;i<minBlock;i++)
          avail[i] = true;
      
      if ( maxBlock >= 0 )
        for ( int i=maxBlock+1;i<avail.Length;i++)
          avail[i] = true;
      
      bool[] downloading = wad.GetBlocksDownloading();
      for( int i=0;i<downloading.Length;i++)
        if ( downloading[i] )
          avail[i] = true; // treat it as available if we're downloading it
      
      if ( strategy == SearchStrategy.FirstBlock )
        vector = GetBlockPeers(wad, minBlock);
      else if ( strategy == SearchStrategy.LastBlock )
        vector = GetBlockPeers(wad, maxBlock);
      else if ( strategy == SearchStrategy.NextBlock )
        vector = GetBlockPeers(wad, getNextUnavailableBlock( avail ));
      else // some random one using a histogram
      {
        var histogram = GetHistogram(wad);
        if (strategy == SearchStrategy.RandomRareBlock)
          vector = histogram.GetRandom(avail, MinPercentile, RareBlockThreshold);
        else if (strategy == SearchStrategy.RandomCommonBlock)
          vector = histogram.GetRandom(avail, CommonBlockThreshold, MaxPercentile);
        else
          vector = histogram.GetRandom(avail, MinPercentile, MaxPercentile);
      }
      return vector;
    }
    
    public BlockAvailabilityHistogram GetHistogram( FileWad wad )
    {
      return new BlockAvailabilityHistogram(this[wad.Id]);
    }
    
    /// <summary>
    /// Invalidates the availability for a peer
    /// </summary>
    /// <param name="peer"></param>
    public void Invalidate( Peer peer )
    {
      foreach( var wad in Values )
        foreach( var matrix in wad )
          if ( matrix.Peer == peer )
            matrix.LastUpdated = DateTime.MinValue;
    }
    
    public bool WeDontKnowWhatsAvailable(FileWad wad)
    {
      DateTime timeout = DateTime.Now.AddSeconds(-BlockAvailabilityTimeout);
      
      if ( !ContainsKey(wad.Id))
        return true;
      
      foreach( var matrix in this[wad.Id] )
        if ( matrix.LastUpdated < timeout )
          return true;
      
      return false;
    }
    
    public void Update( Peer peer, FileWad wad, bool[] availability )
    {
      List<BlockAvailabilityMatrix> availabilityList = null;
      
      if ( !ContainsKey(wad.Id))
      {
        availabilityList = new List<BlockAvailabilityMatrix>();
        Add(wad.Id, availabilityList);
      }
      else
        availabilityList = this[wad.Id];
      
      var matrix = availabilityList.Find(x => x.Peer == peer );
      
      if ( matrix != null )
      {
        matrix.BlockAvailability = availability;
        matrix.LastUpdated = DateTime.Now;
      }
      else
      {
        matrix = new BlockAvailabilityMatrix()
        {
          BlockAvailability = availability,
          Peer = peer,
          LastUpdated = DateTime.Now
        };
        availabilityList.Add(matrix);
      }
      
    }
    
    int getNextUnavailableBlock( bool [] availability )
    {
      for ( int i=0;i<availability.Length;i++)
        if ( !availability[i] )
          return i;
      return -1;
    }
    
  }
}





