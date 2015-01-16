using System;
using System.Linq;
using System.Windows.Input;
using FuzzyHipster.Catalog;

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

        public ICommand Cancel
        {
            get
            {
                return new RelayCommand( p => { MainWindowModel.ChangeModel(new MyListViewModel()); });
            }
        }

      

        public void SaveNewChannel(){


            //TODO: VALIDATION?

            Catalog catalog = FuzzyHipster.MoustacheLayer.Singleton.Catalog;
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
