using FuzzyHipster;
using FuzzyHipster.Catalog;
using FuzzyHipster.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Client
{
    class AddPeersViewModel : ObservableObject<AddFilesViewModel>, IPageViewModel
    {
        //


        public string Name
        {
            get
            {
                return "Add Peers";
            }
        }


        public string PeerIPAddress
        {
            get;
            set;
        }
        private int _port = RWNetwork.RWDefaultPort;
        public int Port
        {
            get{ return _port; }
            set{ _port = value; }
        }

        public string PeerName
        {
            get;
            set;
        }

        public string DefaultPort
        {
            get { return RWNetwork.RWDefaultPort.ToString(); }
        }

        public ICommand AddPeer
        {
            get
            {
                return new RelayCommand(p => AddPeerAndConnect());
            }
        }

        private void AddPeerAndConnect()
        {
            Peer p = new Peer()
            {
                HostAddress = PeerIPAddress,
                Port = Port,
                Name = PeerName
            };
            MoustacheLayer.Singleton.Peers.Add(p);
            MoustacheLayer.Singleton.Network.Connect(p);
            MoustacheLayer.Singleton.Peers.Save();
            PeerListViewModel.Instance.Render();

        }



        public ICommand Cancel
        {
            get
            {
                return new RelayCommand(p => { PeerListViewModel.Instance.Render(); });
            }
        }



    }
}
