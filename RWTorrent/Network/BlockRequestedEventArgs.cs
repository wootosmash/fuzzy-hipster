

using System;
using FuzzyHipster.Catalog;
namespace FuzzyHipster.Network
{
	public class BlockRequestedEventArgs : EventArgs
	{
		public int Block {
			get;
			set;
		}

		public FileWad FileWad {
			get;
			set;
		}

		public Peer Peer {
			get;
			set;
		}

		public BlockRequestedEventArgs(Peer peer, FileWad fileWad, int block)
		{
			this.Peer = peer;
			this.Block = block;
			this.FileWad = fileWad;
		}
	}
}


