using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using test_prism_814.Models;
using test_prism_814.Services;

namespace test_prism_814.ViewModels
{
    public class RegisterWindowViewModel : BindableBase
    {
        private readonly UserRepository _userRepository;

        private string _account;
        public string Account
        {
            get => _account;
            set => SetProperty(ref _account, value);
        }

        private string _password;
        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        private string _confirmPassword;
        public string ConfirmPassword
        {
            get => _confirmPassword;
            set => SetProperty(ref _confirmPassword, value);
        }

        private string _phone;
        public string Phone
        {
            get => _phone;
            set => SetProperty(ref _phone, value);
        }

        private string _address;
        public string Address
        {
            get => _address;
            set => SetProperty(ref _address, value);
        }

        private string _errorMessage;
        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public ICommand RegisterCommand { get; }
        public ICommand GoToLoginCommand { get; }

        public RegisterWindowViewModel(UserRepository userRepository)
        {
            _userRepository = userRepository;
            RegisterCommand = new DelegateCommand(async () => await RegisterAsync());
            GoToLoginCommand = new DelegateCommand(GoToLogin);
        }

        private async Task RegisterAsync()
        {
            // 验证
            if (string.IsNullOrWhiteSpace(Account) || string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "账号和密码不能为空";
                return;
            }

            if (Password != ConfirmPassword)
            {
                ErrorMessage = "两次输入的密码不一致";
                return;
            }

            if (Password.Length < 6)
            {
                ErrorMessage = "密码长度不能少于6位";
                return;
            }

            // 检查账号是否已存在
            if (await _userRepository.AccountExistsAsync(Account))
            {
                ErrorMessage = "该账号已被注册";
                return;
            }

            var newUser = new User
            {
                Account = Account,
                Password = Password,
                Phone = Phone ?? string.Empty,
                Address = Address ?? string.Empty,
                Role = "User",
                CreatedAt = DateTime.Now
            };

            await _userRepository.InsertAsync(newUser);

            MessageBox.Show("注册成功！请返回登录。", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
            ErrorMessage = string.Empty;

            // 清空表单
            Account = string.Empty;
            Password = string.Empty;
            ConfirmPassword = string.Empty;
            Phone = string.Empty;
            Address = string.Empty;
        }

        private void GoToLogin()
        {
            // 关闭注册窗口
            Application.Current.Windows.OfType<Views.RegisterWindow>().FirstOrDefault()?.Close();
        }
    }
}