using Prism.Ioc;
using System;
using System.Windows;
using test_prism_814.Models;
using test_prism_814.ViewModels;

namespace test_prism_814.Views
{
    public partial class LoginWindow : Window
    {
        private bool _isLoggingOut = false; // 标记是否正在登录成功

        public LoginWindow(IContainerProvider containerProvider)
        {
            InitializeComponent();
            this.Closed += LoginWindow_Closed;

            var vm = DataContext as LoginWindowViewModel;
            if (vm != null)
            {
                vm.OnLoginSuccess = (user) => OnLoginSuccess(user, containerProvider);
            }
        }

        private void OnLoginSuccess(User user, IContainerProvider containerProvider)
        {
            _isLoggingOut = true; // ✅ 登录成功，关闭时不退出程序

            var mainWindow = containerProvider.Resolve<MainWindow>(
                new (Type, object)[] { (typeof(User), user) }
            );
            mainWindow.Show();
            this.Close();
        }

        private void LoginWindow_Closed(object sender, System.EventArgs e)
        {
            // ✅ 如果登录窗口被直接关闭（非登录成功），则退出程序
            if (!_isLoggingOut)
            {
                Application.Current.Shutdown();
            }
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as LoginWindowViewModel;
            if (vm != null)
            {
                vm.Password = ((System.Windows.Controls.PasswordBox)sender).Password;
            }
        }

        private void GoToRegister_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var registerWindow = new RegisterWindow();
            registerWindow.ShowDialog();
        }
    }
}