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


        public List<FileWad> Wads
        {
            get 
            {
                StackCollection Stacks = RWTorrent.RWTorrent.Singleton.Catalog.Stacks;
                List<FileWad> Wads = new List<FileWad>();

                foreach (Stack s in Stacks)
                {
                    Wads.AddRange(s.Wads);
                }

                return Wads; 
            }
                         
        }


        


    }
}