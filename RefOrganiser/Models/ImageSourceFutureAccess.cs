using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Search;

namespace RefOrganiser.Models
{
    public class ImageSourceFutureAccess : ImageSourceFolder, IFutureAccessImageSource
    {
        public static async Task<ImageSourceFutureAccess> CreateSource(StorageFolder folder, string token, ImageSourceManager sourceManager)
        {
            ImageSourceFutureAccess source = new ImageSourceFutureAccess(folder, token);
            source.FileQuery = folder.CreateFileQuery(CommonFileQuery.DefaultQuery);
            source.SourceManager = sourceManager;
            var fileSearch = await source.FileQuery.GetFilesAsync();//We use GetFiles here to enable the OnContentsChanged callback
            source.ImageCount = (uint)fileSearch.Count;
            source.FileQuery.ContentsChanged += source.ContentsChangedAsync;

            source.HasSubFolders = false;
            var folders = await source.FileQuery.Folder.GetFoldersAsync();
            if (folders.Count > 0)
            {
                //source.Children = new ObservableCollection<IImageSourceFolder>();
                source.HasSubFolders = true;
                //Crashes UWP - runtime broker goes berserk
                //foreach (var storageFolder in folders)
                //{
                //    var newSource = await ImageSourceFolder.CreateSource(storageFolder, sourceManager);
                //    source.ImageCount += newSource.ImageCount;
                //    source.Children.Add(newSource);
                //}
            }
            return source;
        }

        private ImageSourceFutureAccess(StorageFolder folder, string token) : base(folder)
        {
            Token = token;
        }
        public string Token { get; set; }
    }
}
