using FuzzyHipster;
using FuzzyHipster.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Client
{
    class MyListViewModel : ObservableObject, IPageViewModel
    {

        public MyListViewModel()
        {
            MoustacheLayer.Singleton.Network.NewChannel += Network_NewChannel;
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

        public ICommand AddFiles
        {
            get
            {

                ICommand _changePageCommand = new RelayCommand(
                        p => MainWindowModel.ChangeModel(typeof(AddFilesViewModel)),
                        p => true);
                return _changePageCommand;
            }
        }
        
    }
}
