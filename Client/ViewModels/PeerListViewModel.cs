using FuzzyHipster;
using FuzzyHipster.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    class PeerListViewModel : ObservableObject, IPageViewModel
    {

        public string Name
        {
            get
            {
                return "Peer List";
            }

        }

        public PeerListViewModel()
        {
            RWTorrent.Singleton.Network.PeerConnected += Network_PeerConnected;
            RWTorrent.Singleton.Network.PeerConnectFailed += Network_PeerConnected;
            RWTorrent.Singleton.Network.PeerDisconnected += Network_PeerConnected;
            RWTorrent.Singleton.Network.NewPeer += Network_PeerConnected;
           // RWTorrent.Singleton.Network. += Network_PeerConnected;

        }

        void Network_PeerConnected(object sender, FuzzyHipster.Network.GenericEventArgs<Peer> e)
        {
            throw new NotImplementedException();
        }

        public List<Peer> Peers
        {
            get
            {
                PeerCollection PeerList = FuzzyHipster.RWTorrent.Singleton.Peers;
                return PeerList.Values.ToList();
            }

        }


    }
}
