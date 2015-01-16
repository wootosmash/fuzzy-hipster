using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FuzzyHipster.Catalog;
using FuzzyHipster;
using System.Windows;


namespace Client
{
    class CatalogViewModel : ObservableObject, IPageViewModel
    {
        //
        public string Name
        {
            get
            {
                return "Catalog";
            }

        }

       

        void Network_NewWad(object sender, FuzzyHipster.Network.GenericEventArgs<FileWad> e)
        {
            OnPropertyChanged("Stacks");

        } 

        private List<FileWad> _wads = null;
        public List<FileWad> Wads
        {
            get
            {
               // if (_wads != null)
                //{
                //    return _wads;
              //  }
                StackCollection Stacks = FuzzyHipster.MoustacheLayer.Singleton.Catalog.Stacks;
                _wads = new List<FileWad>();

                foreach (Stack s in Stacks)
                {
                    if (s.Wads == null)
                    {
                        MessageBox.Show("Asdasdasd");
                    }
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