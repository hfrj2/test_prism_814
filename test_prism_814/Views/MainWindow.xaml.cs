using Prism.Ioc;
using Prism.Regions;
using System.Linq;
using System.Windows;
using test_prism_814.Models;
using test_prism_814.ViewModels;

namespace test_prism_814.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow(User currentUser)
        {
            InitializeComponent();
            var region = ContainerLocator.Current.Resolve<IRegionManager>();
            this.DataContext = new MainWindowViewModel(region, currentUser);
            RegionManager.SetRegionManager(A, region);
        }

        private void BackToLogin_Click(object sender, RoutedEventArgs e)
        {
            // 清理全局区域
            var globalRegionManager = ContainerLocator.Current.Resolve<IRegionManager>();
            var regionsToRemove = globalRegionManager.Regions.ToList();
            foreach (var region in regionsToRemove)
            {
                globalRegionManager.Regions.Remove(region.Name);
            }

            // 打开登录窗口（不绑定 Closed 事件）
            var loginWindow = ContainerLocator.Current.Resolve<LoginWindow>();
            Application.Current.MainWindow = loginWindow;
            loginWindow.Show();
            loginWindow.Activate();

            this.Close();
        }

        protected override void OnClosed(System.EventArgs e)
        {
            base.OnClosed(e);
            // 如果通过 X 关闭，且程序未退出，则回到登录
            if (Application.Current.MainWindow == null || Application.Current.MainWindow == this)
            {
                var globalRegionManager = ContainerLocator.Current.Resolve<IRegionManager>();
                var regionsToRemove = globalRegionManager.Regions.ToList();
                foreach (var region in regionsToRemove)
                {
                    globalRegionManager.Regions.Remove(region.Name);
                }

                var loginWindow = ContainerLocator.Current.Resolve<LoginWindow>();
                Application.Current.MainWindow = loginWindow;
                loginWindow.Show();
                loginWindow.Activate();
            }
        }
    }
}