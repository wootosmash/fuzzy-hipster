using FuzzyHipster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    class CoreViewModel : ObservableSettingsObject<CoreViewModel>, IPageViewModel
    {
        public string Name
        {
            get
            {
                return "Core Settings";
            }

        }

        public int HeartBeat
        {
            get
            {
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
