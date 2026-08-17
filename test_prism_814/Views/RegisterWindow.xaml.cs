using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using test_prism_814.ViewModels;

namespace test_prism_814.Views
{
    public partial class RegisterWindow : Window
    {
        public RegisterWindow()
        {
            InitializeComponent();
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

        private void GoToLogin_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }
    }
}