using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Client
{
    public class SettingsWindowModel: ObservableSettingsObject<SettingsWindowModel>
    {
        #region Fields

        private ICommand _changePageCommand;
        
        private IPageViewModel _currentPageViewModel;
        private List<IPageViewModel> _pageViewModels;
        private Dictionary<string, IPageViewModel> _pageViewModelMap;
        

        #endregion

        public SettingsWindowModel()
        {

        }

        public void Setup()
        {
            //Map views against declared type

            IPageViewModel ConnectionPage = AddPageInstance(new ConnectionViewModel(), true);
            IPageViewModel CorePage = AddPageInstance(new CoreViewModel(), true);
            CurrentPageViewModel = CorePage;
        }


        public IPageViewModel AddPageInstance(IPageViewModel page, bool AddToMenu)
        {
            PageViewModelMap.Add(page.GetType().Name, page);
            if (AddToMenu)
            {
                XamlPageMenuDef.Add(page);
            }
            return page;
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
        //    MoustacheClientModel mw = MoustacheClient.Instance.Model;
        //    mw.CurrentPageViewModel = model;
        //}



        public static void ChangeModelas(Type classType)
        {

            string item = classType.Name;
            MoustacheClientModel mw = (MoustacheClientModel)Application.Current.MainWindow.DataContext;



            if (!mw.PageViewModelMap.Keys.Contains(item))
            {
                return;
            }

            mw.CurrentPageViewModel = mw.PageViewModelMap[item];

        }


        public static void ChangeModel(IPageViewModel model)
        {

            MoustacheClientModel mw = (MoustacheClientModel)Application.Current.MainWindow.DataContext;


            mw.CurrentPageViewModel = model;

        }

        public static void RememberState(string key)
        {
            MoustacheClientModel mw = (MoustacheClientModel)Application.Current.MainWindow.DataContext;
            mw.PageViewModelMap.Add(key, mw.CurrentPageViewModel);
        }


        public static IPageViewModel CurrentView()
        {
            MoustacheClientModel mw = (MoustacheClientModel)Application.Current.MainWindow.DataContext;
            return mw.CurrentPageViewModel;
        }



        public static IPageViewModel GetModel(Type classType)
        {
            MoustacheClientModel mw = (MoustacheClientModel)Application.Current.MainWindow.DataContext;
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