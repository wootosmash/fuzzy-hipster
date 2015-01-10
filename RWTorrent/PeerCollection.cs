

using System;
using System.Collections.Generic;

namespace RWTorrent
{
	public class PeerCollection : SortedList<Guid, Peer>
	{
		public void RefreshPeer(Peer peer)
		{
			if (ContainsKey(peer.Guid))
				this[peer.Guid] = peer;
			this.Add(peer.Guid, peer);
		}

		public void Add(Peer peer)
		{
			Add(peer.Guid, peer);
		}
	}
}


