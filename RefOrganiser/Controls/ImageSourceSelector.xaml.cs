using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using RefOrganiser.Annotations;
using RefOrganiser.Models;
using RefOrganiser.Services;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace RefOrganiser.Controls
{
    public sealed partial class ImageSourceSelector : UserControl, INotifyPropertyChanged
    {
        private ObservableCollection<IImageSource> _imageSources;

        public ImageSourceSelector()
        {
            this.InitializeComponent();
            SourceManager = InjectorService.Default.GetInstance<ImageSourceManager>();
            
        }

        public ObservableCollection<IImageSource> ImageSources
        {
            get => _imageSources;
            private set
            {
                _imageSources = value;
                RaisePropertyChanged();
            }
        }
        private ImageSourceManager SourceManager { get; set; }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            var sourcesList = new ObservableCollection<IImageSource>();
            var imageSources = await SourceManager.GetSources();
            foreach (var source in imageSources)
            {
                sourcesList.Add(source);
            }
            ImageSources = sourcesList;
        }

        public async void SelectedImageSourceChanged(object sender, SelectionChangedEventArgs e)
        {
            Debug.WriteLine("Menu changing selection");
            var addedItems = new List<IImageSource>(e.AddedItems.Count);
            var removedItems = new List<IImageSource>(e.RemovedItems.Count);
            for (int i = 0; i < e.AddedItems.Count; i++)
            {
                IImageSource imageSource = e.AddedItems[i] as IImageSource;
                addedItems.Add(imageSource);
            }
            for (int i = 0; i < e.RemovedItems.Count; i++)
            {
                IImageSource imageSource = e.RemovedItems[i] as IImageSource;
                removedItems.Add(imageSource);
            }
            await SourceManager.ChangeSelectedSources(addedItems, removedItems);
            Debug.WriteLine("Menu changing done");
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private async void ExpandButtonTapped(object sender, TappedRoutedEventArgs e)
        {
            var buttonElement = (ExpandButton) sender;
            if (buttonElement.State == ExpandButton.ExpandState.Expanded)
            {
                if (buttonElement.DataContext is IImageSourceFolder imageSourceFolder)
                {
                    await imageSourceFolder.GetSubFoldersAsync();
                    //RaisePropertyChanged(nameof(ImageSources));
                    //Show the list of sub elements
                    //if (((Grid)buttonElement.Parent).Children[4] is ListView childrenList)
                    //{
                    //    childrenList.Visibility = Visibility.Visible;
                    //}
                }
            }
            else
            {
                //Hide the list of sub elements
                //if (((Grid)buttonElement.Parent).Children[4] is ListView childrenList)
                //{
                //    childrenList.Visibility = Visibility.Collapsed;
                //}
            }
            e.Handled = true;
        }
    }

    //public class ImageSourceTemplateSelector : DataTemplateSelector
    //{
    //    public DataTemplate ImageSource { get; set; }
    //    public DataTemplate ImageSource2 { get; set; }

    //    protected override DataTemplate SelectTemplateCore(object item)
    //    {
    //        if (item is IImageSourceFolder imageFolderSource)
    //        {
    //            return ImageSource2;
    //        }
    //        throw new NotImplementedException("Only one template type is expected");
    //    }

    //    protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
    //    {
    //        if (item is IImageSourceFolder imageFolderSource)
    //        {
    //            return ImageSource2;
    //        }
    //        throw new NotImplementedException("Only one template type is expected");
    //    }
    //}
}
