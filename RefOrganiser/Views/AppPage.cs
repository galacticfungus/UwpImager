using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace RefOrganiser.Views
{
    public class AppPage : Page
    {
        protected AppPage() : base()
        {
            
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (this.DataContext is INavigable navService)
            {
                navService.OnNavigatedTo(this.Frame, e);
            }
            
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            if (this.DataContext is INavigable navService)
            {
                navService.OnNavigatedFrom(this.Frame, e);
            }
        }

    }

    internal interface INavigable
    {
        void OnNavigatedTo(Frame sender, NavigationEventArgs e);
        void OnNavigatedFrom(Frame sender, NavigationEventArgs e);
    }
}
