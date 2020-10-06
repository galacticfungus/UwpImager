using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Search;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Imaging;
using RefOrganiser.Annotations;

namespace RefOrganiser.Models
{
    public class ImageSourceFolder : IImageSourceFolder
    {
        private bool _hasSubFolders;

        public static async Task<IImageSourceFolder> CreateSource(StorageFolder folder, ImageSourceManager sourceManager)
        {
            ImageSourceFolder source = new ImageSourceFolder(folder);
            source.FileQuery = folder.CreateFileQuery(CommonFileQuery.DefaultQuery);
            source.SourceManager = sourceManager;
            //TODO: Change this to getitemsasync 
            var fileSearch = await source.FileQuery.GetFilesAsync();//We use GetFiles here to enable the OnContentsChanged callback

            source.ImageCount = (uint)fileSearch.Count;
            //TODO: Change this to be recursive but it means traversing the folder structure so should we just store it from the start
            //Maybe make each recursive call a seperate task that adds its total to the running total
            source.FileQuery.ContentsChanged += source.ContentsChangedAsync;
            source.HasSubFolders = false;
            var folders = await source.FileQuery.Folder.GetFoldersAsync();
            if (folders.Count> 0)
            {
                //source.Children = new ObservableCollection<IImageSourceFolder>();
                source.HasSubFolders = true;
                //TODO: Scan them for item counts
                //Recursively scanning crashes UWP runtimebroker.exe
                //foreach (var storageFolder in folders)
                //{
                //    var newSource = await CreateSource(storageFolder, sourceManager);
                //    source.ImageCount += newSource.ImageCount;
                //    source.Children.Add(newSource);
                //}
            }
            return source;
        }

        public bool HasSubFolders
        {
            get => _hasSubFolders;
            set
            {
                _hasSubFolders = value;
                RaisePropertyChanged();
            }
        }

        private StorageFolder Folder { get; set; }
        public string DisplayName { get; set; }
        public ImageSourceType SourceType => ImageSourceType.Folder;
        protected ImageSourceManager SourceManager { get; set; }

        public uint ImageCount { get; set; }

        protected StorageFileQueryResult FileQuery { get; set; }

        protected ImageSourceFolder(StorageFolder folder)
        {
            Folder = folder;
            DisplayName = folder.DisplayName;
        }

        /// <summary>
        /// This callback is called when the contents of the folder that this image source represents changes in someway
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected async void ContentsChangedAsync(IStorageQueryResultBase sender, object args)
        {
            await UpdateCountAsync();
            //TODO: Need to inform the source manager that this source has changed
            //SourceManager.SourceChanged(this, sender);
            Debug.WriteLine("Source directory or file changed");
            //This refers to the ImageSource and sender refers to the actual StorageFolder that was changed
            //TODO: See what structure changes to files takes
            //TODO: Report to the tree control if needed
        }

        private async Task UpdateCountAsync()
        {
            //uint imageCount = await FileQuery.GetItemCountAsync();
            //if (Children!=null)
            {
                //foreach (var child in Children)
                {
                 //   imageCount += child.ImageCount;
                }
            }
            //ImageCount = imageCount;
        }

        /// <summary>
        /// Returns a list of tasks that will retrieve the thumbnails from this source when awaited
        /// </summary>
        /// <returns></returns>
        public async Task<List<RefImage>> GetRefImagesAsync()
        {
            //TODO: Add some sort of caching to the results of the refimages call

            //TODO: Add subfolders
            //TODO: Add cache
            //TODO: Add Source monitoring, ie watch for changes
            var filesInFolder = await FileQuery.GetFilesAsync();
            var imageTasks = new List<RefImage>(filesInFolder.Count);
            foreach (var file in filesInFolder)
            {
                //TODO: Files that fail to load get a bad thumbnail
                //TODO: Add a cache that can be loaded
                var refImage = new RefImage(file, this);
                imageTasks.Add(refImage);
            }
            return imageTasks;
        }

        public IImageSourceFolder Parent { get; set; }
        public ObservableCollection<IImageSourceFolder> Children { get; set; }

        public async Task GetSubFoldersAsync()
        {
            if (Children != null)//If we have already loaded the children just return
            {
                return;
            }
            Children = new ObservableCollection<IImageSourceFolder>();
            var foldersInFolder = await FileQuery.Folder.GetFoldersAsync();
            foreach (var storageFolder in foldersInFolder)
            {
                var imageSourceFolder = await CreateSource(storageFolder, SourceManager);
                Children.Add(imageSourceFolder as IImageSourceFolder);
            }
            RaisePropertyChanged(nameof(Children));

            //for updates to the tree structure we must rely on the Source updated callback
        }

        /// <summary>
        /// Returns a list of tasks that will retrieve the thumbnails from this source when awaited
        /// </summary>
        /// <returns></returns>
        public async Task<List<RefImage>> GetRefImagesAsync(bool includeSubFolders)
        {
            if (!includeSubFolders)
            {
                return await GetRefImagesAsync();
            }
            throw new NotImplementedException("Not done");
            //TODO: Add some sort of caching to the results of the refimages call

            //TODO: Add subfolders
            //TODO: Add cache
            //TODO: Add Source monitoring, ie watch for changes
            var filesInFolder = await FileQuery.GetFilesAsync();
            var imageTasks = new List<RefImage>(filesInFolder.Count);
            foreach (var file in filesInFolder)
            {
                //TODO: Files that fail to load get a bad thumbnail
                //TODO: Add a cache that can be loaded
                var refImage = new RefImage(file, this);
                imageTasks.Add(refImage);
            }
            return imageTasks;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }


}
