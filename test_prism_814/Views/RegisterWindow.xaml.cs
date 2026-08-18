using System.Windows;
using System.Windows.Controls;
using test_prism_814.ViewModels;

namespace test_prism_814.Views
{
    public partial class RegisterWindow : Window
    {
        public RegisterWindow()
        {
            InitializeComponent();

            // ✅ 订阅 ViewModel 的清空事件
            var vm = DataContext as RegisterWindowViewModel;
            if (vm != null)
            {
                vm.ClearPasswordBoxes += OnClearPasswordBoxes;
            }
        }

        private void OnClearPasswordBoxes()
        {
            // ✅ 清空 PasswordBox 的密码
            PasswordBox.Password = string.Empty;
            ConfirmPasswordBox.Password = string.Empty;

            // ✅ 同时清空 ViewModel 中的密码属性
            var vm = DataContext as RegisterWindowViewModel;
            if (vm != null)
            {
                vm.Password = string.Empty;
                vm.ConfirmPassword = string.Empty;
            }
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as RegisterWindowViewModel;
            if (vm != null)
            {
                vm.Password = PasswordBox.Password;
            }
        }

        private void ConfirmPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            var vm = DataContext as RegisterWindowViewModel;
            if (vm != null)
            {
                vm.ConfirmPassword = ConfirmPasswordBox.Password;
            }
        }

        private void GoToLogin_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.Close();
        }
    }
}