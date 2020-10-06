using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.ApplicationModel.Core;
using Windows.Storage;
using Windows.UI.Xaml;
using GalaSoft.MvvmLight;
using RefOrganiser.Models;
using RefOrganiser.Services;
using RefOrganiser.Views;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Navigation;

namespace RefOrganiser.ViewModels
{
    public class MainViewModel : ViewModelBase, INavigable
    {
        public MainViewModel(ImageSourceManager sourceManager, AppService appService, INavigationService navigationService)
        {
            SourceManager = sourceManager;
            AppService = appService;
            NavigationService = navigationService;
        }

        public async void PageLoaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            await Task.CompletedTask;
        }

        /// <summary>
        /// Responds to a UI trigger to add a Folder as a source of images
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public async void AddFolder(object sender, RoutedEventArgs e)
        {

            var folderPicker = new FolderPicker
            {
                CommitButtonText = "Add Folder",
                SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary
            };
            folderPicker.FileTypeFilter.Add("*");
            var pickedFolder = await folderPicker.PickSingleFolderAsync();
            if (pickedFolder == null)//Operation was cancelled
            {
                return;
            }
            //add picked folder to images list
            //TODO: Move this FutureAccessList inside the SourceManager ie if IImageSourceFutureAccess then
            var token = StorageApplicationPermissions.FutureAccessList.Add(pickedFolder);
            var folderSource = await ImageSourceFutureAccess.CreateSource(pickedFolder, token, SourceManager);
            SourceManager.AddSource(folderSource);
            //The Source manager will inform anyone who cares that a new image source was just added
        }

        public async void ThumbnailSizeChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            AppService.ChangeThumbnailSize(this, e);
            await Task.CompletedTask;
        }

        private void MainPageActivated(object sender, WindowActivatedEventArgs e)
        {
            //TODO: I dont need this function so remove it
            Debug.Assert(Window.Current.Content is Frame);
            var mainPage = ((Frame)Window.Current.Content).Content as MainPage;
            if (e.WindowActivationState != CoreWindowActivationState.Deactivated)
            {
                //mainPage.BackButtonGrid.Visibility = Visibility.Visible;
                //MainTitleBar.Opacity = 1;
                //SearchBox.Opacity = 1;
            }
            else
            {
                //BackButtonGrid.Visibility = Visibility.Collapsed;
                //MainTitleBar.Opacity = 0.5;
                //SearchBox.Opacity = 0.5;
            }
        }

        private ImageSourceManager SourceManager { get; set; }
        private AppService AppService { get; }
        private INavigationService NavigationService { get; }


        public void OnNavigatedTo(Frame sender, NavigationEventArgs e)
        {
            //Restore selected sources
        }

        public void OnNavigatedFrom(Frame sender, NavigationEventArgs e)
        {
            //Save selected sources

        }
    }
}
