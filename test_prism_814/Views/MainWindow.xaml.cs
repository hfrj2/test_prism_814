using Prism.Ioc;
using Prism.Regions;
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
            var region = Prism.Ioc.ContainerLocator.Current.Resolve<Prism.Regions.IRegionManager>();
            // 传入当前用户信息给 ViewModel
            this.DataContext = new MainWindowViewModel(
               region,
               currentUser
            );
            RegionManager.SetRegionManager(A, region);
        }
    }
}