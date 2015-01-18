using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Client
{
    class StartUpViewModel : ObservableObject<StartUpViewModel>, IPageViewModel
    {
        public string Name
        {
            get
            {
                return "Start Up";
            }

        }
   
    }
}
