using RWTorrent.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    class MyListViewModel : ObservableObject, IPageViewModel
    {

        
        public string Name
        {
            get
            {
                return "My Channels";
            }
            
        }

        public List<Stack> Stacks
        {
            get
            {
                StackCollection Stacks = RWTorrent.RWTorrent.Singleton.Catalog.Stacks;
                return Stacks.ToList<Stack>();
            }

        }

        string _description;
        public string Description
        {
            get{return _description;}
            set
            {
                if (value != _description)
                {
                    _description = value;
                    OnPropertyChanged("Description");
                }
            }
        }
        
    }
}
