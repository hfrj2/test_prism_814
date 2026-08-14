using Prism.Ioc;
using System.Windows;
using test_prism_814.Services;
using test_prism_814.Views;

namespace test_prism_814
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {

            

            containerRegistry.RegisterForNavigation<NoteManageUserControl>("NoteManageUserControl");
            containerRegistry.RegisterForNavigation<UserManageUserControl>("UserManageUserControl");

            containerRegistry.RegisterSingleton<NoteRepository>();
        }
    }
}
