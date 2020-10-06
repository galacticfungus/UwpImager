using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Networking.Connectivity;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using RefOrganiser.Services;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace RefOrganiser.Controls
{
    public sealed partial class TitleBar : UserControl
    {
        public TitleBar()
        {
            this.InitializeComponent();
            NavigationService = InjectorService.Default.GetInstance<INavigationService>();
        }

        private void TitleBarLoaded(object sender, RoutedEventArgs e)
        {
            var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            var titleBar = ApplicationView.GetForCurrentView().TitleBar;
            titleBar.ButtonBackgroundColor = Colors.Transparent;
            titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
            PositionTitleBar(coreTitleBar);
            coreTitleBar.LayoutMetricsChanged += TitleBarMetricsChanged;
            Window.Current.SetTitleBar(AppTitleBar);
            //AppTitle.Text = TitleText;
            AppTitle.Text = "Reference Thingy";
            Window.Current.Activated += PageActivated;
            BackButton.IsEnabled = NavigationService.CanGoBack;
        }

        private void PositionTitleBar(CoreApplicationViewTitleBar titleBar)
        {
            LeftPaddingColumn.Width = new GridLength(titleBar.SystemOverlayLeftInset);
            RightPaddingColumn.Width = new GridLength(titleBar.SystemOverlayRightInset);
            //mainPage.TitleBarButton.Margin = new Thickness(0, 0, coreTitleBar.SystemOverlayRightInset, 0);
            AppTitleBar.Height = titleBar.Height;
        }

        private void PageActivated(object sender, WindowActivatedEventArgs e)
        {
            //throw new NotImplementedException();
            //Need to color the title bar to show it is active etc
        }

        private void TitleBarMetricsChanged(CoreApplicationViewTitleBar sender, object args)
        {
            // Get the size of the caption controls area and back button 
            // (returned in logical pixels), and move your content around as necessary.
            PositionTitleBar(sender);
        }

        public static readonly DependencyProperty TitleTextProperty = DependencyProperty.Register(
            "TitleText", typeof(string), typeof(TitleBar), new PropertyMetadata(default(string)));

        public string TitleText
        {
            get { return (string) GetValue(TitleTextProperty); }
            set { SetValue(TitleTextProperty, value); }
        }

        private INavigationService NavigationService { get; }

        private void BackButtonTapped(object sender, TappedRoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}
