

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using FuzzyHipster.Catalog;
namespace FuzzyHipster.Network
{
  /// <summary>
  /// EventArgs sent when a block transfer request arrives
  /// </summary>
	public class BlockTransferStartedEventArgs : EventArgs
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
	  
	  /// <summary>
	  /// Set to false if you don't want to accept
	  /// </summary>
	  public bool Accept {
	    get;
	    set; 
	  }

		public BlockTransferStartedEventArgs(Peer peer, FileWad fileWad, int block)
		{
			this.Peer = peer;
			this.Block = block;
			this.FileWad = fileWad;
			this.Accept = true;
		}
	}
}


