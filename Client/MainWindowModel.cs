﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using FuzzyHipster.Network;

namespace Client
{
    class MainWindowModel : ObservableObject<MainWindowModel>
    {
        #region Fields

        private ICommand _changePageCommand;
        
        private IPageViewModel _currentPageViewModel;
        private List<IPageViewModel> _pageViewModels;
        private Dictionary<string, IPageViewModel> _pageViewModelMap;
        

        #endregion

        public MainWindowModel()
        {
            CurrentPageViewModel = new SplashViewModel();
        }

        public void Setup()
        {
            //Map views against declared type
            PageViewModelMap.Add(typeof(StartUpViewModel).Name, new StartUpViewModel());
            PageViewModelMap.Add(typeof(MyListViewModel).Name, new MyListViewModel());
            PageViewModelMap.Add(typeof(CatalogViewModel).Name, new CatalogViewModel());
            PageViewModelMap.Add(typeof(PeerListViewModel).Name, new PeerListViewModel());

            PageViewModelMap.Add(typeof(AddFilesViewModel).Name, new AddFilesViewModel());
            PageViewModelMap.Add(typeof(NewChannelViewModel).Name, new NewChannelViewModel());
            
            //
            XamlPageMenuDef.Add(PageViewModelMap[typeof(StartUpViewModel).Name]);
            XamlPageMenuDef.Add(PageViewModelMap[typeof(MyListViewModel).Name]);
            XamlPageMenuDef.Add(PageViewModelMap[typeof(CatalogViewModel).Name]);
            XamlPageMenuDef.Add(PageViewModelMap[typeof(PeerListViewModel).Name]);
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

        public List<IPageViewModel> XamlPageMenuDef
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

        //public static void ChangeModel(IPageViewModel model){
        //    MainWindowModel mw = (MainWindowModel)Application.Current.MainWindow.DataContext;
        //    mw.CurrentPageViewModel = model;
        //}



        public static void ChangeModelas(Type classType)
        {

            string item = classType.Name;
            MainWindowModel mw = (MainWindowModel)Application.Current.MainWindow.DataContext;



            if (!mw.PageViewModelMap.Keys.Contains(item))
            {
                return;
            }

            mw.CurrentPageViewModel = mw.PageViewModelMap[item];

        }


        public static void ChangeModel(IPageViewModel model)
        {
           
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



        public static IPageViewModel GetModel(Type classType)
        {
            MainWindowModel mw = (MainWindowModel)Application.Current.MainWindow.DataContext;
            return mw.PageViewModelMap[classType.Name];
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
           // if (!XamlPageMenuDef.Contains(viewModel))
             //   XamlPageMenuDef.Add(viewModel);

            CurrentPageViewModel = XamlPageMenuDef
                .FirstOrDefault(vm => vm == viewModel);



        }




        #endregion
    }
}
