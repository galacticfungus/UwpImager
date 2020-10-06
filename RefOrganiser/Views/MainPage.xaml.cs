using System;

using RefOrganiser.ViewModels;

using Windows.UI.Xaml.Controls;

namespace RefOrganiser.Views
{
    public sealed partial class MainPage : AppPage
    {
        private MainViewModel ViewModel => DataContext as MainViewModel;

        public MainPage()
        {
            InitializeComponent();
        }

        
    }
}
