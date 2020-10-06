using RefOrganiser.Services;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using RefOrganiser.Models;
using RefOrganiser.ViewModels;
using SimpleInjector;
using RefOrganiser.Controls;

namespace RefOrganiser
{
    public sealed partial class App : Application
    {
        public App()
        {
            RegisterDependencies();
            InitializeComponent();
            var appService = InjectorService.Default.GetInstance<AppService>();
            this.LeavingBackground += appService.HandleLeaveBackground;
            this.EnteredBackground += appService.HandleEnteredBackground;
            NavigationService.RegisterPages();
        }

        private void RegisterDependencies()
        {
            InjectorService.Default.RegisterSingleton<INavigationService> (() => new NavigationService(Pages.MainPage));
            InjectorService.Default.RegisterSingleton<AppService>();
            InjectorService.Default.RegisterSingleton<ImageSourceManager>();
            InjectorService.Default.RegisterSingleton<MainViewModel>();
            InjectorService.Default.Register<ReferenceViewModel>(Lifestyle.Transient);//We should recreate directx resources each time to avoid using GPU unnecessarily
            InjectorService.Default.RegisterSingleton<ViewModelLocator>();
            InjectorService.Default.Verify();
        }

        protected override async void OnLaunched(LaunchActivatedEventArgs args)
        {
            var appService = InjectorService.Default.GetInstance<AppService>();
            await appService.HandleLaunch(args);
        }
    }
}
