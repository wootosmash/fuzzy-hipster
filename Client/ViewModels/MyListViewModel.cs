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
            RWTorrent.Singleton.Network.NewWad += Network_NewWad;
        }

        void Network_NewWad(object sender, FuzzyHipster.Network.GenericEventArgs<FileWad> e)
        {
            OnPropertyChanged("Stacks");
          
        } 
        
        public string Name
        {
            get
            {
                return "My Channels";
            }
            
        }

        public List<Stack> Stacks
        {
            get
            {
                StackCollection Stacks = FuzzyHipster.RWTorrent.Singleton.Catalog.Stacks;
                return Stacks.ToList<Stack>();
            }

        }

        Stack _selectedStack;
        public Stack SelectedStack
        {
            get { return _selectedStack; }
            set
            {
                if (value != _selectedStack)
                {
                    _selectedStack = value;
                    OnPropertyChanged("SelectedStack");
                    OnPropertyChanged("IsChannelSelected");
                }
            }
        }

        public Boolean IsChannelSelected
        {
            get
            {
                if (SelectedStack == null)
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
                        p => MainWindowModel.ChangeModel(new AddFilesViewModel(SelectedStack)),
                        p => true);
                return _changePageCommand;
            }
        }
        
    }
}
