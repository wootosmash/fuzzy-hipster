using FuzzyHipster;
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
            MoustacheLayer.Singleton.Catalog.NotifyChannel += Catalog_NotifyChannel;
            MoustacheLayer.Singleton.Catalog.NotifyFileWad += Catalog_NotifyFileWad;
        }

        void Catalog_NotifyFileWad(object sender, GenericEventArgs<FileWad> e)
        {
             OnPropertyChanged("Wads");  
        }
        
        void Catalog_NotifyChannel(object sender, FuzzyHipster.GenericEventArgs<Channel> e)
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
                    OnPropertyChanged("IsChannelSelectedCollapsed");
                    OnPropertyChanged("WadVisibility");
                    OnPropertyChanged("Wads");

                    WadsColumn = "300";
                    OnPropertyChanged("WadsColumn");

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

        public Visibility IsChannelSelectedCollapsed
        {
            get
            {
                if (SelectedChannel == null)
                {
                    return Visibility.Collapsed;
                }

                return Visibility.Visible;
            }
        }


        private string _wadsColumn = "0";
        public string WadsColumn
        {
            get
            {
                return _wadsColumn;
            }
            set
            {
                _wadsColumn = value;
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
            get { return _description; }
            set
            {
                if (value != _description)
                {
                    _description = value;
                    OnPropertyChanged("Description");
                }
            }
        }
        



        public List<FileWad> Wads
        {
            get
            {
                return _selectedChannel.Wads.ToList<FileWad>();
            }

        }

        public ICommand AddFiles
        {
            get
            {

                return new RelayCommand( p => MoustacheClientModel.ChangeModel(new AddFilesViewModel()) );
            }
        }

        public ICommand NewChannel
        {
            get
            {

               return  new RelayCommand( p => MoustacheClientModel.ChangeModel(new NewChannelViewModel()));
            }
        }
        
    }
}
