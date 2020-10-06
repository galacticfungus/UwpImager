using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.UI.Xaml.Data;
using RefOrganiser.ViewModels;

namespace RefOrganiser.Models
{
    /// <summary>
    /// Keeps track of all sources of images, signals events when changes occur to the list of folders or when the contents of a folder changes 
    /// </summary>
    public partial class ImageSourceManager
    {
        public ImageSourceManager()
        {
            SelectedImageSources = new List<IImageSource>();

        }

        public delegate void SourceChangedEventHandler(object sender, SourcesChangedEventArgs e);
        public delegate void SelectedSourcesChangedEventHandler(object sender, SelectedSourcesChangedEventArgs e);

        public event SourceChangedEventHandler ImageSourceAdded;
        public event SourceChangedEventHandler ImageSourceRemoved;

        public event SelectedSourcesChangedEventHandler SelectedSourcesChanged;

        public async Task<List<IImageSource>> GetSelectedSources()
        {
            await Task.CompletedTask;
            return SelectedImageSources;
        }

        /// <summary>
        /// Used to change the currently in use image sources
        /// </summary>
        /// <param name="addedSources">The sources that were added to the selection</param>
        /// <param name="removedSources">The sources that were removed from the selection</param>
        /// <returns></returns>
        public async Task ChangeSelectedSources(List<IImageSource> addedSources, List<IImageSource> removedSources)
        {
            Debug.WriteLine("Request to change selected image sources made");
            for (int i = 0; i < addedSources.Count; i++)
            {
                SelectedImageSources.Add(addedSources[i]);
            }

            for (int i = 0; i < removedSources.Count; i++)
            {
                SelectedImageSources.Remove(removedSources[i]);
            }
            //TODO: Convert SelectedImageSources to dictionary?
            OnSelectedSourcesChanged(new SelectedSourcesChangedEventArgs(addedSources, removedSources));
            Debug.WriteLine("Request to change selected image sources finished");
            await Task.CompletedTask;
        }

        /// <summary>
        /// This function is used to report that the currently in use sources are changing
        /// </summary>
        /// <param name="e"></param>
        public void OnSelectedSourcesChanged(SelectedSourcesChangedEventArgs e)
        {
            Debug.WriteLine("Selected sources changing");
            //This for some reason calls twice
            SelectedSourcesChanged?.Invoke(this, e);
            Debug.WriteLine("Selected sources changed");
        }

        /// <summary>
        /// This function is used to broadcast an SelectedSources was added event
        /// </summary>
        /// <param name="e">Event data including details about the SelectedSources</param>
        public void OnImageSourceAdded(SourcesChangedEventArgs e)
        {
            ImageSourceAdded?.Invoke(this, e);
        }
        /// <summary>
        /// Reports that an image source was removed, the image source in question may or may not have been in use
        /// </summary>
        /// <param name="e"></param>
        public void OnImageSourceRemoved(SourcesChangedEventArgs e)
        {
            ImageSourceRemoved?.Invoke(this, e);
        }

        //TODO: Free sources when suspending

        //TODO: Build sources when resuming without refreshing UI?

        public void AddSource(IImageSource source)
        {
            //TODO: Deal with ImageSources being null - ie we are adding a source before retrieving saved sources
            //TODO: Move adding the source to the FutureList to this method - add a string token parameter
            ImageSources.Add(source);
            OnImageSourceAdded(new SourcesChangedEventArgs(source));
        }

        public void RemoveSource(IImageSource source)
        {
            //TODO: Deal with ImageSources being null - ie we are adding a source before retrieving saved sources
            //TODO: Deal with the case that the image source was currently in use and should be removed from the selected sources
            if (source is IFutureAccessImageSource futureSource)
            {
                StorageApplicationPermissions.FutureAccessList.Remove(futureSource.Token);
            }
            ImageSources.Remove(source);
            OnImageSourceRemoved(new SourcesChangedEventArgs(source));
        }

        //TODO: Add support for ISupportIncrementalLoading INotifyCollectionChanged IList to IImageSource
        private ObservableCollection<IImageSource> ImageSources { get; set; }
        private List<IImageSource> SelectedImageSources { get; set; }

        public async Task<ObservableCollection<IImageSource>> GetSources()
        {
            if (ImageSources == null)
            {
                ImageSources = new ObservableCollection<IImageSource>();
                foreach (var token in StorageApplicationPermissions.FutureAccessList.Entries)
                {
                    //Read folder into list
                    //If we add additional types of ImageSources then we need to filter how we add them here
                    var folder = await StorageApplicationPermissions.FutureAccessList.GetFolderAsync(token.Token);
                    //TODO: Static method to create an ImageSourceFolder from a token
                    var imageFolder = await ImageSourceFutureAccess.CreateSource(folder, token.Token, this);
                    //TODO: Refresh the content of the image folder or validate it
                    ImageSources.Add(imageFolder);
                    //TODO: We can only monitor a folder for changes if the get files function has been called on the folder
                }
            }

            return ImageSources;
        }

        public async Task<IImageSource> GetFutureAccessSource(string token)
        {
            var folder = await StorageApplicationPermissions.FutureAccessList.GetFolderAsync(token);
            //TODO: Static method to create an ImageSourceFolder from a token
            var imageFolder = await ImageSourceFutureAccess.CreateSource(folder, token, this);
            //TODO: Refresh the content of the image folder or validate it
            ImageSources.Add(imageFolder);
            return imageFolder;
        }
    }

    
}
