using FuzzyHipster;
using FuzzyHipster.Catalog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Client
{
    class AddFilesViewModel : ObservableObject, IPageViewModel
    {
        //

        private Channel _channel = null;

        public string Name
        {
            get
            {
                return "Add Files";
            }

        }

        public AddFilesViewModel(Channel channel)
        {
            _channel = channel;
        }
        public AddFilesViewModel()
        {
            _channel = null;
        }


        public string Channel
        {
            get { return _channel.Name; }
        }
        
        private string _wadPath;
        public string WadPath
        {
            get { return this._wadPath; }
            set
            {
                // Implement with property changed handling for INotifyPropertyChanged
                if (!string.Equals(this._wadPath, value))
                {
                    this._wadPath = value;
                    this.RaisePropertyChanged("WadPath"); // Method to raise the PropertyChanged event in your BaseViewModel class...
                }
            }
        }

        

        private string _wadName;
        public string WadName
        {
            get { return this._wadName; }
            set
            {
                // Implement with property changed handling for INotifyPropertyChanged
                if (!string.Equals(this._wadName, value))
                {
                    this._wadName = value;
                    this.RaisePropertyChanged("WadName"); // Method to raise the PropertyChanged event in your BaseViewModel class...
                }
            }
        }

        private string _wadDescription;
        public string WadDescription
        {
            get { return this._wadDescription; }
            set
            {
                // Implement with property changed handling for INotifyPropertyChanged
                if (!string.Equals(this._wadDescription, value))
                {
                    this._wadDescription = value;
                    this.RaisePropertyChanged("WadDescription"); // Method to raise the PropertyChanged event in your BaseViewModel class...
                }
            }
        }


        public ICommand SelectFile
        {
            get
            {
                return new RelayCommand(p => getFile());
            }
        }

        public ICommand SelectFolder
        {
            get
            {
                return new RelayCommand(p => getFolder());
            }
        }


        private void getFolder()
        {
            WadPath = Utils.GetFolderPath();
            OnPropertyChanged("WadPath");
        }

        private void getFile()
        {
            WadPath = Utils.getFilePath();
            OnPropertyChanged("WadPath");
        }



        public ICommand CreateFolder
        {
            get
            {
                return new RelayCommand(p => AddWad());
            }
        }

        public ICommand Cancel
        {
            get
            {
                return new RelayCommand(p => { MainWindowModel.ChangeModel(typeof (MyListViewModel)); });
            }
        }


        public void AddWad()
        {

            long BlockSize = 1024;
            try
            {
                 BlockSize = FileWad.CalculatePathSize(WadPath);
            }
            catch (Exception e)
            {
                Debug.Print(e.ToString());
            }
          
            

            FileWad myprog = new FileWad() { ChannelId = _channel.Id, BlockSize = BlockSize, Name = this._wadName, Description = _wadDescription };
            _channel.Wads.Add(myprog);
            myprog.Save();

            MainWindowModel.ChangeModel(typeof(MyListViewModel));
        }

    }
}
