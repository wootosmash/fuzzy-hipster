﻿using FuzzyHipster;
using FuzzyHipster.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Client
{
    class MyListViewModel : ObservableObject<MyListViewModel>, IPageViewModel
    {

        public MyListViewModel()
        {
            MoustacheLayer.Singleton.Network.NewChannel += Network_NewChannel;
            MoustacheLayer.Singleton.Network.NewWad += Network_NewWad;
        }

        void Network_NewWad(object sender, FuzzyHipster.Network.GenericEventArgs<FileWad> e)
        {
            OnPropertyChanged("CatalogWads");
        }

        void Network_NewChannel(object sender, FuzzyHipster.Network.GenericEventArgs<Channel> e)
        {
            OnPropertyChanged("Channels");
        }

        
        public string Name
        {
            get
            {
                return "My Channels";
            }
            
        }

        public List<Channel> Channels
        {
            get
            {
                ChannelCollection Channels = FuzzyHipster.MoustacheLayer.Singleton.Catalog.Channels;
                return Channels.ToList<Channel>();
            }

        }

        Channel _selectedChannel;
        public Channel SelectedChannel
        {
            get { return _selectedChannel; }
            set
            {
                if (value != _selectedChannel)
                {
                    _selectedChannel = value;
                    OnPropertyChanged("SelectedChannel");
                    OnPropertyChanged("IsChannelSelected");
                    OnPropertyChanged("WadVisibility");
                    OnPropertyChanged("CatalogWads");
                }
            }
        }

        public Boolean IsChannelSelected
        {
            get
            {
                if (SelectedChannel == null)
                {
                    return false;
                }

                return true;
            }
        }

        public Visibility WadVisibility
        {
            get
            {
                return SelectedChannel != null ? Visibility.Visible : Visibility.Hidden;
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


        private List<FileWad> _catalogWads = null;
        public List<FileWad> CatalogWads
        {
            get
            {
                return _selectedChannel.Wads;
            }

        }

        public ICommand AddFiles
        {
            get
            {

                return new RelayCommand( p => MainWindowModel.ChangeModel(new AddFilesViewModel()) );
            }
        }

        public ICommand NewChannel
        {
            get
            {

               return  new RelayCommand( p => MainWindowModel.ChangeModel(new NewChannelViewModel()));
            }
        }
        
    }
}
