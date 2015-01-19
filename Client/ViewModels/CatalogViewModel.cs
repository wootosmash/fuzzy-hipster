using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FuzzyHipster.Catalog;
using FuzzyHipster;
using System.Windows;
using System.Diagnostics;


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


        public CatalogViewModel()
        {
            FuzzyHipster.MoustacheLayer.Singleton.Network.NewWad += Network_NewWad;
        }

        void Network_NewWad(object sender, FuzzyHipster.Network.GenericEventArgs<FileWad> e)
        {
            OnPropertyChanged("Wads");
        }

        private List<FileWad> _wads = null;
        public List<FileWad> Wads
        {
            get
            {
                ChannelCollection Channels = FuzzyHipster.MoustacheLayer.Singleton.Catalog.Channels;
                _wads = new List<FileWad>();

                foreach (Channel s in Channels)
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