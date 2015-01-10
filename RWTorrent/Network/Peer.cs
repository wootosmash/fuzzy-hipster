/*
 * Created by SharpDevelop.
 * User: Al
 * Date: 10/01/2015
 * Time: 5:11 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Runtime.InteropServices;
namespace RWTorrent.Network
{
  [Serializable()]
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public class Peer
	{
		public Guid Guid {
			get;
			set;
		}

		public string Name {
			get;
			set;
		}

		public int PeerCount {
			get;
			set;
		}

		public int CatalogRecency {
			get;
			set;
		}

		public long Uptime {
			get;
			set;
		}

	  public Peer()
	  {
	    Guid = Guid.NewGuid();
	  }
	  
		public override string ToString()
		{
			return string.Format("[Peer Guid={0}, Name={1}, PeerCount={2}, CatalogRecency={3}, Uptime={4}]", Guid, Name, PeerCount, CatalogRecency, Uptime);
		}
	}
}


