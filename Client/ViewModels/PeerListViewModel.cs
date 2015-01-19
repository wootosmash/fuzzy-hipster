using FuzzyHipster;
using FuzzyHipster.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    class PeerListViewModel : ObservableObject<PeerListViewModel>, IPageViewModel
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
            
           MoustacheLayer.Singleton.Network.PeerConnected += Network_PeerConnected;

           MoustacheLayer.Singleton.Network.PeerConnectFailed += Network_PeerConnected;

           MoustacheLayer.Singleton.Network.PeerDisconnected += Network_PeerConnected;

           MoustacheLayer.Singleton.Network.NewPeer += Network_PeerConnected;

           //// MoustacheLayer.Singleton.Network. += Network_PeerConnected;


            FuzzyHipster.MoustacheLayer.Singleton.HeartBeat.Elapsed += HeartBeat_Elapsed;

        }

        void HeartBeat_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            OnPropertyChanged("Peers");
        }

        void Network_PeerConnected(object sender, FuzzyHipster.GenericEventArgs<Peer> e)
        {
            OnPropertyChanged("Peers");
        }

        public List<Peer> Peers
        {
            get
            {
                PeerCollection PeerList = FuzzyHipster.MoustacheLayer.Singleton.Peers;
                return PeerList.ToList <Peer>();
            }

        }


    }
}
