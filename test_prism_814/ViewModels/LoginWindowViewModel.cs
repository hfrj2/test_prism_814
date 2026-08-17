using Prism.Commands;
using Prism.Mvvm;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using test_prism_814.Models;
using test_prism_814.Services;

namespace test_prism_814.ViewModels
{
    public class LoginWindowViewModel : BindableBase
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

        private string _errorMessage;
        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public ICommand LoginCommand { get; }
        public ICommand GoToRegisterCommand { get; }

        // 登录成功后的回调（用于关闭登录窗口、打开主窗口）
        public System.Action<User> OnLoginSuccess { get; set; }

        public LoginWindowViewModel(UserRepository userRepository)
        {
            _userRepository = userRepository;
            LoginCommand = new DelegateCommand(async () => await LoginAsync());
            GoToRegisterCommand = new DelegateCommand(GoToRegister);
        }

        private async Task LoginAsync()
        {
            if (string.IsNullOrWhiteSpace(Account) || string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "请输入账号和密码";
                return;
            }

            var user = await _userRepository.GetByAccountAsync(Account);
            if (user == null || user.Password != Password)
            {
                ErrorMessage = "账号或密码错误";
                return;
            }

            ErrorMessage = string.Empty;
            OnLoginSuccess?.Invoke(user);
        }

        private void GoToRegister()
        {
            // 打开注册窗口
            var registerWindow = new Views.RegisterWindow();
            registerWindow.ShowDialog();
        }
    }
}