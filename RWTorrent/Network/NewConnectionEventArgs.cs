

using System;
using System.Net.Sockets;
namespace FuzzyHipster.Network
{
	public class NewConnectionEventArgs : EventArgs
	{
		public Socket Socket = null;

		public bool Accept = true;

		public NewConnectionEventArgs(Socket socket)
		{
			Socket = socket;
		}
	}
}


