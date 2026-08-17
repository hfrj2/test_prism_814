using Prism.DryIoc;
using Prism.Ioc;
using System.Windows;
using test_prism_814.Services;
using test_prism_814.Views;

namespace test_prism_814
{
    public partial class App : PrismApplication
    {
        protected override Window CreateShell()
        {
            // 不再直接启动 MainWindow，而是先启动 LoginWindow
            return null;
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            // 注册导航视图
            containerRegistry.RegisterForNavigation<NoteManageUserControl>("NoteManageUserControl");
            containerRegistry.RegisterForNavigation<UserManageUserControl>("UserManageUserControl");

            // 注册数据库服务（单例）
            containerRegistry.RegisterSingleton<NoteRepository>();
            containerRegistry.RegisterSingleton<UserRepository>();
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            // 启动登录窗口
            var loginWindow = Container.Resolve<LoginWindow>();
            loginWindow.Show();
        }
    }

    // 转换器：字符串非空 -> Visible，空 -> Collapsed
    public class StringToVisibilityConverter : System.Windows.Data.IValueConverter
    {
        public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return string.IsNullOrWhiteSpace(value as string) ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new System.NotImplementedException();
        }
    }
}