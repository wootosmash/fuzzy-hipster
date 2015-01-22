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
        
    }
}
