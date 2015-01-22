

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using FuzzyHipster.Catalog;
using FuzzyHipster.Crypto;
namespace FuzzyHipster.Network
{
	public class NetworkSocket : IDisposable
	{
		public static int NextId {
			get;
			set;
		}

		public int Id {
			get;
			set;
		}

		Socket socket;

		public EndPoint RemoteEndPoint {
			get {
				return socket.RemoteEndPoint;
			}
		}

		public bool Connected {
			get {
				return socket.Connected;
			}
		}

		public NetworkSocket(AddressFamily addressFamily, SocketType socketType, ProtocolType protocol)
		{
			this.Id = NetworkSocket.NextId;
			NextId++;
			socket = new Socket(addressFamily, socketType, protocol);
		}

		public NetworkSocket(Socket socket)
		{
			this.Id = NetworkSocket.NextId;
			NextId++;
			this.socket = socket;
		}

		public int EndReceive(IAsyncResult ar)
		{
			return socket.EndReceive(ar);
		}

		public IAsyncResult BeginReceive(byte[] buffer, int offset, int count, SocketFlags flags, AsyncCallback callback, object state)
		{
			return socket.BeginReceive(buffer, offset, count, flags, callback, state);
		}

		public IAsyncResult BeginConnect(EndPoint remoteEP, AsyncCallback callback, object state)
		{
			return socket.BeginConnect(remoteEP, callback, state);
		}

		public void EndConnect(IAsyncResult asyncResult)
		{
			socket.EndConnect(asyncResult);
		}

		public IAsyncResult BeginSend(byte[] buffer, int offset, int count, SocketFlags flags, AsyncCallback callback, object state)
		{
			return socket.BeginSend(buffer, 0, count, 0, callback, state);
		}

		public int EndSend(IAsyncResult asyncResult)
		{
			return socket.EndSend(asyncResult);
		}

		public void Disconnect()
		{
			if (socket.Connected)
				socket.Disconnect(false);
		}

		#region IDisposable implementation
		public void Dispose()
		{
			socket.Dispose();
		}

		#endregion
		public override string ToString()
		{
			return string.Format("[NetworkSocket Id={1} Socket={0}]", Id, socket);
		}
	}
}


