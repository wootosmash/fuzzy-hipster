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
namespace RWTorrent
{
	public class BlockIndexItem
	{
		public string Hash {
			get;
			set;
		}

		public long Length {
			get;
			set;
		}

		public bool Downloaded {
			get;
			set;
		}
	}
}



