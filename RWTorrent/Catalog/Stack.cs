/*
 * Created by SharpDevelop.
 * User: Al
 * Date: 10/01/2015
 * Time: 11:53 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Xml.Serialization;
namespace RWTorrent.Catalog
{
	public class Stack
	{
		public Guid Id {
			get;
			set;
		}

		public string Name {
			get;
			set;
		}

		public string Description {
			get;
			set;
		}

		public string PublicKey {
			get;
			set;
		}

		public List<FileWad> Wads {
			get;
			set;
		}

		public Stack()
		{
			Wads = new List<FileWad>();
		}
	}
}



