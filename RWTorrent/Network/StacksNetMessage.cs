/*
 * Created by SharpDevelop.
 * User: Al
 * Date: 10/01/2015
 * Time: 5:11 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using FuzzyHipster.Catalog;
namespace FuzzyHipster.Network
{
	[Serializable()]
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public class ChannelsNetMessage : NetMessage
	{
		public Channel[] Channels {
			get;
			set;
		}

		public ChannelsNetMessage()
		{
			Type = MessageType.Channels;
			Channels = new Channel[0];
		}

		public override string ToString()
		{
			if (Channels.Length == 0)
				return "[ChannelsNetMessage Channels.Count=0]";
			else
				return string.Format("[ChannelsNetMessage Channels={0}]", Channels);
		}
	}
}


