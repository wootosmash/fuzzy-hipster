/*
 * Created by SharpDevelop.
 * User: Al
 * Date: 10/01/2015
 * Time: 11:53 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.IO;
using System.Xml.Serialization;
using RWTorrent.Network;

namespace RWTorrent
{
	public class Settings
	{
	  public int ActivePeers { get; set; }
		public int HeartbeatPeerRequestCount {get; set; }
		public int HeartbeatStackRequestCount {get; set; }
		public int HeartbeatInterval {get; set; }
		public int Port { get; set; }

		public Settings()
		{
			ActivePeers = 30;
			HeartbeatPeerRequestCount = 30;
			HeartbeatStackRequestCount = 3;
			HeartbeatInterval = 60000;
			Port = RWNetwork.RWDefaultPort;
		}
		
		public static Settings Load(string path)
		{
		  Settings settings = null;

		  if ( !File.Exists(path))
		    return new Settings();
		  
		  var serialiser = new XmlSerializer(typeof(Settings));
			using (var reader = new StreamReader(path))
				settings = (Settings)serialiser.Deserialize(reader);

			return settings;
		}

		public void Save()
		{
			var serialiser = new XmlSerializer(typeof(Settings));
			using (var writer = new StreamWriter(@"settings.xml"))
				serialiser.Serialize(writer, this);
		}		
	}
}



