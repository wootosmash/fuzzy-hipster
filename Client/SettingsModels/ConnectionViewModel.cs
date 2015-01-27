using FuzzyHipster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    class ConnectionViewModel : ObservableSettingsObject<ConnectionViewModel>, IPageViewModel
    {
        public string Name
        {
            get
            {
                return "Connection";
            }

        }

        public int HeartBeat
        {
            get {
                return MoustacheLayer.Singleton.Settings.HeartbeatInterval;
            }
            set
            {
                MoustacheLayer.Singleton.Settings.HeartbeatInterval = value;
                MoustacheLayer.Singleton.Settings.Save();
            }
        }

        public int CatalogThinkInterval
        {
            get
            {
                return MoustacheLayer.Singleton.Settings.CatalogThinkInterval;
            }
            set
            {
                MoustacheLayer.Singleton.Settings.CatalogThinkInterval = value;
            }
        }

        public int ConnectAttemptWaitTime
        {
            get
            {
                return MoustacheLayer.Singleton.Settings.ConnectAttemptWaitTime;
            }
            set
            {
                MoustacheLayer.Singleton.Settings.ConnectAttemptWaitTime = value;
            }
        }

        public int DefaultAdvertisementMoratorium
        {
            get
            {
                return MoustacheLayer.Singleton.Settings.DefaultAdvertisementMoratorium;
            }
            set
            {
                MoustacheLayer.Singleton.Settings.DefaultAdvertisementMoratorium = value;
            }
        }

        public int DefaultBlockQuantity
        {
            get
            {
                return MoustacheLayer.Singleton.Settings.DefaultBlockQuantity;
            }
            set
            {
                MoustacheLayer.Singleton.Settings.DefaultBlockQuantity = value;
            }
        }

        public int DefaultMaxBlockPacketSize
        {
            get
            {
                return MoustacheLayer.Singleton.Settings.DefaultMaxBlockPacketSize;
            }
            set
            {
                MoustacheLayer.Singleton.Settings.DefaultMaxBlockPacketSize = value;
            }
        }


        public int DesiredPeerListSize
        {
            get
            {
                return MoustacheLayer.Singleton.Settings.DesiredPeerListSize;
            }
            set
            {
                MoustacheLayer.Singleton.Settings.DesiredPeerListSize = value;
            }
        }

        public int KeepAliveInterval
        {
            get
            {
                return MoustacheLayer.Singleton.Settings.KeepAliveInterval;
            }
            set
            {
                MoustacheLayer.Singleton.Settings.KeepAliveInterval = value;
            }
        }

        public int MaxActiveBlockTransfers
        {
            get
            {
                return MoustacheLayer.Singleton.Settings.MaxActiveBlockTransfers;
            }
            set
            {
                MoustacheLayer.Singleton.Settings.MaxActiveBlockTransfers = value;
            }
        }



        
    }
}
