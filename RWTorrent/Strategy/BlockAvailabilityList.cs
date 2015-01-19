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
	public class BlockAvailabilityList : SortedList<Guid, List<BlockAvailabilityMatrix>>
	{
		public enum SearchStrategy
		{
			RareBlocks,
			Any,
			CommonBlocks
		}

		/// <summary>
		/// Gets a random block and list of peers with the block based on the list of available blocks and the desired strategy
		/// </summary>
		/// <param name="wad"></param>
		/// <param name="list"></param>
		/// <param name="strategy"></param>
		/// <returns></returns>
		public KeyValuePair<int, Peer[]> GetRandomBlockPeer(FileWad wad, BlockAvailabilityList list, SearchStrategy strategy)
		{
			const int CommonBlockThreshold = 80;
			const int RareBlockThreshold = 20;
			const int MinPercentile = 0;
			const int MaxPercentile = 100;
			KeyValuePair<int, Peer[]> blockPeers;
			
			var histogram = new BlockAvailabilityHistogram(list[wad.Id]);
			if (strategy == SearchStrategy.RareBlocks)
				blockPeers = histogram.GetRandom(wad.GetBlockAvailability(), MinPercentile, RareBlockThreshold);
			// get blocks that haven't been peer'd lots
			else
				if (strategy == SearchStrategy.CommonBlocks)
					blockPeers = histogram.GetRandom(wad.GetBlockAvailability(), CommonBlockThreshold, MaxPercentile);
				else
					blockPeers = histogram.GetRandom(wad.GetBlockAvailability(), MinPercentile, MaxPercentile);
			return blockPeers;
		}
	}
}





