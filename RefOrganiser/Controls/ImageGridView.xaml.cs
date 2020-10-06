using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.Foundation.Metadata;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using RefOrganiser.Annotations;
using RefOrganiser.Models;
using RefOrganiser.Services;
using RefOrganiser.ViewModels;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace RefOrganiser.Controls
{
    public sealed partial class ImageGridView : UserControl, INotifyPropertyChanged
    {
        public ImageGridView()
        {
            this.InitializeComponent();
            Images = new ObservableCollection<RefImage>();
            SourceManager = InjectorService.Default.GetInstance<ImageSourceManager>();
            AppService = InjectorService.Default.GetInstance<AppService>();
            
        }

        private void OnThumbnailSizeChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            //We need to update thumbnail sizes
            //Can add a method 
        }

        internal ObservableCollection<RefImage> Images { get; set; }
        internal ImageSourceManager SourceManager { get; set; }
        public AppService AppService { get; }
        //public GridView ImagesGrid { get; set; }

        public async void ControlLoaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            SourceManager.SelectedSourcesChanged += SelectedSourcesChanged;
            AppService.ThumbnailSizeChanged += OnThumbnailSizeChanged;
            //TODO: Find out what image sources need to be displayed by default or at startup, Perhaps the previously displayed sources
            await Task.CompletedTask;
        }

        public async void SelectedSourcesChanged(object sender, SelectedSourcesChangedEventArgs e)
        {
            List<RefImage> imagesToAdd = new List<RefImage>();
            for (int i = 0; i < e.AddedSources.Count; i++)
            {
                imagesToAdd.AddRange(await e.AddedSources[i].GetRefImagesAsync());
                //TODO: Fix threading bug since loop never completes i is not incremented so loop completes loading multiple elements
            }

            List<RefImage> removedSource = new List<RefImage>();
            for (int i = 0; i < e.RemovedSources.Count; i++)
            {
                for (var index = 0; index < Images.Count; index++)
                {
                    var refImage = Images[index];
                    if (refImage.SourceToken == e.RemovedSources[i])
                    {
                        Images.Remove(refImage);
                        index--;
                    }
                }
            }

            
            
            foreach (var refImage in imagesToAdd)
            {
                Images.Add(refImage);
            }
            
            OnPropertyChanged("Images");
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
