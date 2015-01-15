

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
	public class GenericEventArgs<T> : EventArgs
	{
		public T Value {
			get;
			set;
		}

		public GenericEventArgs(T value)
		{
			Value = value;
		}
	}
}


