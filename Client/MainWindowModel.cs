using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using RWTorrent.Network;

namespace Client
{
    class MainWindowModel : ObservableObject
    {
        #region Fields

        private ICommand _changePageCommand;
        
        private IPageViewModel _currentPageViewModel;
        private List<IPageViewModel> _pageViewModels;
        private Dictionary<string, IPageViewModel> _pageViewModelMap;
        

        #endregion

        public MainWindowModel()
        {
            // Add available pages
            PageViewModels.Add(new StartUpViewModel());
            PageViewModels.Add(new MyListViewModel());
            PageViewModels.Add(new CatalogViewModel());

           

            // Set starting page
            CurrentPageViewModel = new SplashViewModel();

        }

        #region Properties / Commands

        public ICommand ChangePageCommand
        {
            get
            {
                if (_changePageCommand == null)
                {
                    _changePageCommand = new RelayCommand(
                        p => ChangeViewModel((IPageViewModel)p),
                        p => p is IPageViewModel);
                }

                return _changePageCommand;
            }
        }

        public List<IPageViewModel> PageViewModels
        {
            get
            {
                if (_pageViewModels == null)
                    _pageViewModels = new List<IPageViewModel>();

                return _pageViewModels;
            }
        }

        public Dictionary<string,IPageViewModel> PageViewModelMap
        {
            get
            {
                if (_pageViewModelMap == null)
                    _pageViewModelMap = new Dictionary<string,IPageViewModel>();

                return _pageViewModelMap;
            }
        }

        public static void ChangeModel(IPageViewModel model){
            MainWindowModel mw = (MainWindowModel)Application.Current.MainWindow.DataContext;
            mw.CurrentPageViewModel = model;
        }

        public static void RememberState(string key)
        {
            MainWindowModel mw = (MainWindowModel)Application.Current.MainWindow.DataContext;
            mw.PageViewModelMap.Add(key, mw.CurrentPageViewModel);
        }


        public static IPageViewModel CurrentView()
        {
            MainWindowModel mw = (MainWindowModel)Application.Current.MainWindow.DataContext;
            return mw.CurrentPageViewModel;
        }



         public static void LoadState(string key)
        {
            MainWindowModel mw = (MainWindowModel)Application.Current.MainWindow.DataContext;
            mw.CurrentPageViewModel = mw.PageViewModelMap[key];
        }


        public IPageViewModel CurrentPageViewModel
        {
            get
            {
                return _currentPageViewModel;
            }
            set
            {
                if (_currentPageViewModel != value)
                {
                    _currentPageViewModel = value;
                    OnPropertyChanged("CurrentPageViewModel");
                }
            }
        }

        #endregion

        #region Methods

        public void ChangeViewModel(IPageViewModel viewModel)
        {
            if (!PageViewModels.Contains(viewModel))
                PageViewModels.Add(viewModel);

            CurrentPageViewModel = PageViewModels
                .FirstOrDefault(vm => vm == viewModel);
        }

        #endregion
    }
}
