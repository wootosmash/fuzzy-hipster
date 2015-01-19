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
    class CatalogViewModel : ObservableObject<CatalogViewModel>, IPageViewModel
    {
        //
        public string Name
        {
            get
            {
                return "Catalog";
            }

        }

       

        void Catalog_NotifyWad(object sender, FuzzyHipster.GenericEventArgs<FileWad> e)
        {
            OnPropertyChanged("Channels");

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
                ChannelCollection Channels = FuzzyHipster.MoustacheLayer.Singleton.Catalog.Channels;
                _wads = new List<FileWad>();

                foreach (Channel s in Channels)
                {
                    //if (s.Wads == null)
                    //{
                    // MessageBox.Show("Asdasdasd");
                    //}
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