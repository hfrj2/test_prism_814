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

            // ✅ 直接在构造函数中导航到便签管理
            region.RequestNavigate("ContentRegion", "NoteManageUserControl");
        }

        // ✅ 窗口加载完成后自动导航到便签管理
      

        // ✅ 点击按钮回到登录
        private void BackToLogin_Click(object sender, RoutedEventArgs e)
        {
            // 清理全局区域
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

            this.Close();
        }

        protected override void OnClosed(System.EventArgs e)
        {
            base.OnClosed(e);
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