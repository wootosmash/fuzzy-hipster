﻿using System;
using System.Linq;
using System.Windows.Input;
using FuzzyHipster.Catalog;
using FuzzyHipster;

namespace Client
{
    class NewChannelViewModel : ObservableObject<NewChannelViewModel>, IPageViewModel
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
                return new RelayCommand(p => { MyListViewModel.Instance.Render(); });
            }
        }

      

        public void SaveNewChannel(){


            //TODO: VALIDATION?
          
            Channel newChannel = new Channel()
            {
                Id = Guid.NewGuid(),
                Name = this.ChannelName,
                Description = this.ChannelDescription
            };

            //TODO: new channelsigning.

           // newChannel.Sign(MoustacheLayer.Singleton.Me.AsymmetricKey);
            MoustacheLayer.Singleton.Catalog.AddChannel(newChannel);
            MyListViewModel.Instance.Render();
        }
    }
}
