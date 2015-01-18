using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    class KeysViewModel : ObservableObject<KeysViewModel>, IPageViewModel
    {

        public KeysViewModel()
        {
           // FuzzyHipster.Crypto.Key
            //MoustacheLayer.Singleton.Network.NewChannel += Network_NewChannel;
        }



        public string Name
        {
            get
            {
                return "My Channels";
            }
        }
            
    }
}
