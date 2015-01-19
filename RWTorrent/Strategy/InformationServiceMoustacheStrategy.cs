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
using System.Timers;
using FuzzyHipster.Catalog;
using FuzzyHipster.Network;
namespace FuzzyHipster
{
	public class InformationServiceMoustacheStrategy : MoustacheStrategy
	{
		public override void Install()
		{
			Network.PeersRequested += NetworkPeersRequested;
			Network.ChannelsRequested += NetworkChannelsRequested;
			Network.WadsRequested += NetworkWadsRequested;
			Network.BlocksAvailableRequested += NetworkBlocksAvailableRequested;
			Network.BlockRequested += NetworkBlockRequested;
		}

		public override void Uninstall()
		{
			Network.PeersRequested -= NetworkPeersRequested;
			Network.ChannelsRequested -= NetworkChannelsRequested;
			Network.WadsRequested -= NetworkWadsRequested;
			Network.BlocksAvailableRequested -= NetworkBlocksAvailableRequested;
			Network.BlockRequested -= NetworkBlockRequested;
		}

		public override void Think()
		{
		}

		void NetworkPeersRequested(object sender, MessageComposite<RequestPeersNetMessage> e)
		{
			var peers = new List<Peer>();
			foreach (var peer in Peers)
				peers.Add(peer);
			Network.SendPeerList(peers.ToArray(), e.Peer);
		}

		void NetworkChannelsRequested(object sender, MessageComposite<RequestChannelsNetMessage> e)
		{
			Network.SendChannels(Catalog.Channels.ToArray(), e.Peer);
		}

		void NetworkWadsRequested(object sender, MessageComposite<RequestWadsNetMessage> e)
		{
			if (Catalog.Channels[e.Value.ChannelGuid] == null)
				return;
			if (Catalog.Channels[e.Value.ChannelGuid].Wads == null)
				Network.SendWads(null, e.Peer);
			else
				Network.SendWads(Catalog.Channels[e.Value.ChannelGuid].Wads.ToArray(), e.Peer);
		}

		void NetworkBlocksAvailableRequested(object sender, MessageComposite<RequestBlocksAvailableNetMessage> e)
		{
			var wad = Catalog.GetFileWad(e.Value.FileWadId);
			if (wad == null)
				return;
			Network.SendBlocksAvailable(e.Peer, wad);
		}

		void NetworkBlockRequested(object sender, BlockRequestedEventArgs e)
		{
			Network.SendBlock(Catalog.GetFileWad(e.FileWadId), e.Block, e.Peer);
		}
	}
}



