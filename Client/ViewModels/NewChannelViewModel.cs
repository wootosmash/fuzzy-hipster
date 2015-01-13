using RWTorrent.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Client
{
    class NewChannelViewModel : ObservableObject, IPageViewModel
    {


        public string Name
        {
            get
            {
                return "New Channel";
            }

        }

        private string _channelName;
        public string ChannelName
        {
            get { return this._channelName; }
            set
            {
                // Implement with property changed handling for INotifyPropertyChanged
                if (!string.Equals(this._channelName, value))
                {
                    this._channelName = value;
                    this.RaisePropertyChanged("ChannelName"); // Method to raise the PropertyChanged event in your BaseViewModel class...
                }
            }
        }

        private string _channelDescription;
        public string ChannelDescription
        {
            get { return this._channelDescription; }
            set
            {
                // Implement with property changed handling for INotifyPropertyChanged
                if (!string.Equals(this._channelDescription, value))
                {
                    this._channelDescription = value;
                    this.RaisePropertyChanged("ChannelDescription"); // Method to raise the PropertyChanged event in your BaseViewModel class...
                }
            }
        }



        public ICommand SaveChannel
        {
            get
            {

                ICommand _changePageCommand = new RelayCommand(
                        p => SaveNewChannel(),
                        p => true);

            

                return _changePageCommand;
            }
        }

      

        public void SaveNewChannel(){


            //TODO: VALIDATION?

            Catalog catalog = RWTorrent.RWTorrent.Singleton.Catalog;
            Stack newStack = new Stack()
            {
                Id = Guid.NewGuid(),
                Name = this.ChannelName,
                Description = this.ChannelDescription,
                PublicKey = "..."
            };
            catalog.Stacks.Add(newStack);
            catalog.Save();

            MainWindowModel.ChangeModel(new MyListViewModel());
        }
    }
}
