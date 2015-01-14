using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    class SplashViewModel : ObservableObject, IPageViewModel
    {

        public string Name
        {
            get
            {
                return "Splash";
            }

        }

        

    }
}
