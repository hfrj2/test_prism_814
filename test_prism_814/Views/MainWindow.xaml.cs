using Prism.Ioc;
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
            // 传入当前用户信息给 ViewModel
            this.DataContext = new MainWindowViewModel(
                Prism.Ioc.ContainerLocator.Current.Resolve<Prism.Regions.IRegionManager>(),
                currentUser
            );
        }
    }
}