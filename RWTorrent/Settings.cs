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
using FuzzyHipster.Network;

namespace FuzzyHipster
{
	public class Settings
	{
	  public int MaxActivePeers { get; set; }
	  
	  /// <summary>
	  /// Amount of seconds to wait between connect attempts to the same peer. Will be used in exponential backoff.
	  /// </summary>
	  public int ConnectAttemptWaitTime { get; set; }
		public int HeartbeatPeerRequestCount {get; set; }
		public int HeartbeatStackRequestCount {get; set; }
		public int HeartbeatInterval {get; set; }
		public int Port { get; set; }		
		public int MaxActiveBlockTransfers { get; set; }
		public int DefaultMaxBlockPacketSize { get; set; }
		public int ThinkTimeGraceMilliseconds { get; set; }

		public Settings()
		{
			MaxActivePeers = 1;
			HeartbeatPeerRequestCount = 30;
			HeartbeatStackRequestCount = 3;
			HeartbeatInterval = 10000;
			Port = RWNetwork.RWDefaultPort;
			ConnectAttemptWaitTime = 60;
			MaxActiveBlockTransfers = 10;
			DefaultMaxBlockPacketSize = 40000;
			ThinkTimeGraceMilliseconds = 3000;
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



