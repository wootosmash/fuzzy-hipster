

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
	public class MessageComposite<T> : EventArgs
	{
		public Peer Peer {
			get;
			set;
		}

		public T Value {
			get;
			set;
		}

		public MessageComposite(Peer peer, T value)
		{
			Peer = peer;
			Value = value;
		}
	}
}


