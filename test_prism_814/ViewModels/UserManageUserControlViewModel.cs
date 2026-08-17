using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using test_prism_814.Models;
using test_prism_814.Services;

namespace test_prism_814.ViewModels
{
    public class UserManageUserControlViewModel : BindableBase
    {
        private readonly UserRepository _repository;

        // 用户列表
        private ObservableCollection<User> _users;
        public ObservableCollection<User> Users
        {
            get => _users;
            set => SetProperty(ref _users, value);
        }

        // 当前选中用户
        private User _selectedUser;
        public User SelectedUser
        {
            get => _selectedUser;
            set
            {
                SetProperty(ref _selectedUser, value);
                if (value != null)
                {
                    Account = value.Account;
                    Password = value.Password;
                    Phone = value.Phone;
                    Address = value.Address;
                }
                else
                {
                    ClearForm();
                }
                // 刷新删除按钮状态
                (DeleteCommand as DelegateCommand)?.RaiseCanExecuteChanged();
            }
        }

        // 表单字段
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

        private string _searchKeyword;
        public string SearchKeyword
        {
            get => _searchKeyword;
            set
            {
                SetProperty(ref _searchKeyword, value);
                _ = SearchAsync();
            }
        }

        // 命令
        public ICommand LoadCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand NewCommand { get; }
        public ICommand SearchCommand { get; }

        public UserManageUserControlViewModel(UserRepository repository)
        {
            _repository = repository;
            Users = new ObservableCollection<User>();

            LoadCommand = new DelegateCommand(async () => await LoadAllAsync());
            SaveCommand = new DelegateCommand(async () => await SaveAsync());
            DeleteCommand = new DelegateCommand(async () => await DeleteAsync(), () => SelectedUser != null);
            NewCommand = new DelegateCommand(ClearForm);
            SearchCommand = new DelegateCommand(async () => await SearchAsync());

            _ = LoadAllAsync();
        }

        private async Task LoadAllAsync()
        {
            var list = await _repository.GetAllUsersAsync();
            UpdateList(list);
        }

        private async Task SearchAsync()
        {
            if (string.IsNullOrWhiteSpace(SearchKeyword))
            {
                await LoadAllAsync();
                return;
            }

            // 在内存中搜索（数据量小）
            var all = await _repository.GetAllUsersAsync();
            var filtered = all.Where(x =>
                x.Account.Contains(SearchKeyword, StringComparison.OrdinalIgnoreCase) ||
                x.Phone.Contains(SearchKeyword, StringComparison.OrdinalIgnoreCase) ||
                x.Address.Contains(SearchKeyword, StringComparison.OrdinalIgnoreCase)
            );
            UpdateList(filtered);
        }

        private void UpdateList(System.Collections.Generic.IEnumerable<User> list)
        {
            Users.Clear();
            foreach (var item in list)
            {
                Users.Add(item);
            }
        }

        private async Task SaveAsync()
        {
            if (string.IsNullOrWhiteSpace(Account) || string.IsNullOrWhiteSpace(Password))
            {
                MessageBox.Show("账号和密码不能为空", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (Password.Length < 6)
            {
                MessageBox.Show("密码长度不能少于6位", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (SelectedUser == null)
            {
                // 新增
                if (await _repository.AccountExistsAsync(Account))
                {
                    MessageBox.Show("该账号已存在", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
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
                await _repository.InsertAsync(newUser);
            }
            else
            {
                // 更新
                SelectedUser.Account = Account;
                SelectedUser.Password = Password;
                SelectedUser.Phone = Phone ?? string.Empty;
                SelectedUser.Address = Address ?? string.Empty;
                await _repository.UpdateAsync(SelectedUser);
            }

            await LoadAllAsync();
            ClearForm();
        }

        private async Task DeleteAsync()
        {
            if (SelectedUser == null) return;

            // 不允许删除管理员（安全检查）
            if (SelectedUser.Role == "Admin")
            {
                MessageBox.Show("不能删除管理员账号", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var result = MessageBox.Show($"确定要删除用户「{SelectedUser.Account}」吗？", "确认删除",
                MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                await _repository.DeleteAsync(SelectedUser.Id);
                await LoadAllAsync();
                ClearForm();
            }
        }

        private void ClearForm()
        {
            SelectedUser = null;
            Account = string.Empty;
            Password = string.Empty;
            Phone = string.Empty;
            Address = string.Empty;
        }
    }
}