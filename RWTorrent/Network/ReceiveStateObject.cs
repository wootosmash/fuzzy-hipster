

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
	public class ReceiveStateObject
	{
		public const int BufferSize = 65536 * 4;

		public Peer Peer = null;

		public bool WaitingLengthFrame = true;

		public int ExpectedLength = 4;

		public byte[] Buffer = new byte[BufferSize];
	}
}


