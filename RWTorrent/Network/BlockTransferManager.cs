

using System;
using FuzzyHipster.Catalog;

namespace FuzzyHipster.Network
{
	public class BlockTransferManager : TransferManager
	{
		public override void Execute()
		{
			var fileWad = MoustacheLayer.Singleton.Catalog.GetFileWad(FileWadId);
			if (fileWad == null)
				throw new Exception("Can't find file wad to save block to " + FileWadId);
			fileWad.VerifyBlock(Block, TempFile);
			fileWad.BlockIndex[Block].Downloaded = true;
			fileWad.CatalogBlock(Block, TempFile);
		}
	}
}


