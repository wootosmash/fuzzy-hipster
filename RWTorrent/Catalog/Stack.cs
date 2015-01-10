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
		
		/// <summary>
		/// Gets the wads after a certain recency number oldest to newest
		/// </summary>
		/// <param name="recency"></param>
		/// <param name="count"></param>
		/// <returns></returns>
		public FileWad[] GetWadsByRecency( long recency, int count )
		{
		  int i = 0;
		  var wads = new List<FileWad>();
		  
		  while( i < count && i < Wads.Count )
		  {
		    if ( wads[i].LastUpdate > recency )
		      wads.Add( Wads[i] );
		    i++;
		  }
		  
		  return wads.ToArray();
		}
	}
}



