using Prism.Commands;
using Prism.Regions;
using System.Collections.ObjectModel;
using System.Windows.Input;
using test_prism_814.Models;
using test_prism_814.Services;

namespace test_prism_814.ViewModels
{
    public class MainWindowViewModel
    {
        private readonly IRegionManager _regionManager;
        private readonly User _currentUser;

        public ObservableCollection<MenuItemModel> MenuItems { get; }
        public ICommand NavigateCommand { get; }

        // 显示当前登录用户
        public string CurrentUserDisplay => $"欢迎，{_currentUser?.Account}";

        public MainWindowViewModel(IRegionManager regionManager, User currentUser)
        {
            _regionManager = regionManager;
            _currentUser = currentUser;

            NavigateCommand = new DelegateCommand<string>(Navigate);

            // 根据用户角色动态生成菜单
            MenuItems = new ObservableCollection<MenuItemModel>();

            // 所有用户都能看到便签管理
            MenuItems.Add(new MenuItemModel
            {
                Header = "便签管理",
                Icon = "\uE8A5",
                ViewName = "NoteManageUserControl"
            });

            // 只有管理员能看到用户管理
            if (_currentUser?.Role == "Admin")
            {
                MenuItems.Add(new MenuItemModel
                {
                    Header = "用户管理",
                    Icon = "\uE77B",
                    ViewName = "UserManageUserControl"
                });
            }
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