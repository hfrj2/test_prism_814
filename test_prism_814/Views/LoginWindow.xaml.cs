using Prism.Ioc;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using test_prism_814.Models;
using test_prism_814.ViewModels;

namespace test_prism_814.Views
{
    public partial class LoginWindow : Window
    {
        private IContainerProvider _containerProvider;
        public LoginWindow(IContainerProvider containerProvider)
        {
            _containerProvider = containerProvider;
            InitializeComponent();
            // 订阅登录成功事件
            var vm = DataContext as LoginWindowViewModel;
            if (vm != null)
            {
                vm.OnLoginSuccess = OnLoginSuccess;
            }
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as LoginWindowViewModel;
            if (vm != null)
            {
                vm.Password = PasswordBox.Password;
            }
        }

        private void OnLoginSuccess(Models.User user)
        {
            // 登录成功，打开主窗口
            var mainWindow = _containerProvider.Resolve<MainWindow>(new (Type Type, object Instance)[] { (typeof(User), user) });
            mainWindow.Show();
            this.Close();
        }

        private void GoToRegister_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var registerWindow = new RegisterWindow();
            registerWindow.ShowDialog();
        }
    }
}