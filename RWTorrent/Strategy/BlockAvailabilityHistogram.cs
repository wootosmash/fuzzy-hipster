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
namespace FuzzyHipster
{
  /// <summary>
  /// Builds list of blocks that are available and determines how available they are
  /// </summary>
  public class BlockAvailabilityHistogram
  {
    Dictionary<int, int> histogram = new Dictionary<int, int>();

    IList<BlockAvailabilityMatrix> list = new List<BlockAvailabilityMatrix>();

    public BlockAvailabilityHistogram(IList<BlockAvailabilityMatrix> list)
    {
      this.list = list;
      while (histogram.Count < list.Count)
        histogram.Add(histogram.Count, 0);
      foreach (var matrix in list)
        Add(matrix);
    }

    public void Add(BlockAvailabilityMatrix matrix)
    {
      for (int i = 0; i < matrix.BlockAvailability.Length; i++)
      {
        if (matrix.BlockAvailability[i])
        {
          if ( histogram.ContainsKey(i))
            histogram[i]++;
          else
            histogram.Add(i,1);
        }
      }
    }

    /// <summary>
    /// Gets a random block based on the percentiles passed and the list of blocks to ignore
    /// </summary>
    /// <param name="blocksToIgnore"></param>
    /// <param name="minPercentile"></param>
    /// <param name="maxPercentile"></param>
    /// <returns></returns>
    public KeyValuePair<int, Peer[]> GetRandom(bool[] blocksToIgnore, int minPercentile, int maxPercentile)
    {
      var dic = new Dictionary<int, int>(histogram);
      for (int i = 0; i < blocksToIgnore.Length; i++)
        dic.Remove(i);
      
      if (dic.Count == 0)
        return new KeyValuePair<int, Peer[]>(-1, null);
      
      List<KeyValuePair<int, int>> myList = dic.ToList();
      myList.Sort((firstPair, nextPair) => firstPair.Value.CompareTo(nextPair.Value));
      
      int min = (minPercentile * myList.Count) / 100;
      int max = (maxPercentile * myList.Count) / 100;
      int block = myList[MoustacheLayer.Singleton.Random.Next(min, max + 1)].Key;
      
      // build a list of peers that have the block
      var peers = new List<Peer>();
      foreach (var matrix in list) {
        if (matrix.BlockAvailability[block])
          peers.Add(matrix.Peer);
      }
      
      return new KeyValuePair<int, Peer[]>(block, peers.ToArray());
    }
  }
}





