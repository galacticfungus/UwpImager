using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using GalaSoft.MvvmLight;
using RefOrganiser.Models;
using RefOrganiser.Services;
using RefOrganiser.Views;

namespace RefOrganiser.ViewModels
{
    public class ReferenceViewModel : ViewModelBase, INavigable
    {
        private BitmapImage _image;
        private AppService AppService { get; set; }
        public ReferenceViewModel(AppService appService)
        {
            AppService = appService;
        }

        public async void PageLoading(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {

            await Task.CompletedTask;
        }

        public async void PageLoaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            await Task.CompletedTask;
        }

        public async void OnNavigatedTo(Frame sender, NavigationEventArgs e)
        {
            if (e.NavigationMode != NavigationMode.New)
            {
                throw new InvalidOperationException("");
            }
            ConnectedAnimation imageAnimation =
                ConnectedAnimationService.GetForCurrentView().GetAnimation("imageTransition");
            var page = e.Content as ReferencePage;
            var target = page.FindName("ReferenceImage") as Image;
            imageAnimation?.TryStart(target);
            if (e.Parameter is RefImage refImage)//TODO: if it isn't I should show an error
            {
                //Forced to load the image here since this is where we have first access to the parameter
                await refImage.LoadCompleteImageAsync();
                //TODO: Consider creating a RefThumbnail class that can create a RefImage class
                Image = await refImage.LoadCompleteImageAsync();
            }
        }

        public BitmapImage Image
        {
            get
            {
                return _image;
            }

            set
            {
                _image = value;
                RaisePropertyChanged();
            }
        }

        public async void OnNavigatedFrom(Frame sender, NavigationEventArgs e)
        {
            
        }
    }
}
