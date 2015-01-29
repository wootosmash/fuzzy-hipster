

using System;
using FuzzyHipster.Catalog;

namespace FuzzyHipster.Network
{
	public class BlockTransferManager : TransferManager
	{
		public override void Execute()
		{
			FileWad.VerifyBlock(Block, TempFile);
			FileWad.BlockIndex[Block].Downloading = false;
			FileWad.BlockIndex[Block].Downloaded = true;
			FileWad.CatalogBlock(Block, TempFile);
			MoustacheLayer.Singleton.Catalog.AddBlock(FileWad, Block);
		}
	}
}


