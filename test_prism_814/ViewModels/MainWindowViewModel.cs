using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System.Collections.ObjectModel;
using System.Windows.Input;
using test_prism_814.Models;

namespace test_prism_814.ViewModels
{


        public class MainWindowViewModel
        {
        private readonly IRegionManager _regionManager;

        public ObservableCollection<MenuItemModel> MenuItems { get; }

        public ICommand NavigateCommand { get; }

        public MainWindowViewModel(IRegionManager regionManager)
        {
            _regionManager = regionManager;

            NavigateCommand = new DelegateCommand<string>(Navigate); ;

            MenuItems = new ObservableCollection<MenuItemModel>
            {
                new MenuItemModel
                {
                    Header = "便签管理",
                    Icon = "\uE8A5",      // Segoe MDL2 字体中的文档图标
                    ViewName = "NoteManageUserControl"

                },
                 new MenuItemModel
                 {
                    Header = "用户管理",
                    Icon = "\uE77B",      // Segoe MDL2 字体中的人物图标
                    ViewName = "UserManageUserControl"
                }
            };



        }
        private void Navigate(string viewName)
        {
            if (!string.IsNullOrWhiteSpace(viewName))
            {
                _regionManager.RequestNavigate("ContentRegion", viewName);
            }
        }
    }
    
}
