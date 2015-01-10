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
using RWTorrent.Network;
using RWTorrent.Catalog;

namespace RWTorrent
{
	public class RWTorrent
	{
	  public static RWTorrent Singleton { get; protected set; }
	  
		public RWServer Server {
			get;
			set;
		}

		public List<RWClient> Clients {
			get;
			set;
		}
	  
	  public Catalog.Catalog Catalog { get; set; }

		public RWTorrent( Catalog.Catalog catalog )
		{
			Server = new RWServer();
			Singleton = this;
			Catalog = catalog;
		}
		
		public void Start()
		{
		  Server.StartListening();
		}
	}

}

