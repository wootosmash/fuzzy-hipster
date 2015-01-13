using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RWTorrent.Catalog;


namespace Client
{
    class CatalogViewModel : ObservableObject, IPageViewModel
    {
        public string Name
        {
            get
            {
                return "Catalog";
            }

        }

        private List<FileWad> _wads = null;
        public List<FileWad> Wads
        {
            get 
            {   
                if(_wads != null){
                    return _wads;
                }
                StackCollection Stacks = RWTorrent.RWTorrent.Singleton.Catalog.Stacks;
                _wads = new List<FileWad>();

                foreach (Stack s in Stacks)
                {
                    _wads.AddRange(s.Wads);
                }

                return _wads; 
            }
                         
        }

        public int WadCount
        {
            get
            {
                
                return Wads.Count;

            }
        }


        


    }
}