using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using Windows.Foundation.Metadata;
using Windows.Storage;

namespace RefOrganiser.Models
{
    /// <summary>
    /// Represents an object that can retrieve a list of images from somewhere, it is a flat list with no organisation
    /// </summary>
    public interface IImageSource : INotifyPropertyChanged
    {
        Task<List<RefImage>> GetRefImagesAsync();
        string DisplayName { get; set; }
        uint ImageCount { get; set; }
    }

    ///  <inheritdoc cref="IImageSource"/>
    ///  <summary>
    /// Represents an object that can retrieve a list of folders that contain images, it can also retrieve all the images contained in all of its sub folders
    ///  </summary>
    public interface IImageSourceFolder : IImageSource
    {
        IImageSourceFolder Parent { get; set; }//TODO: Establish this is needed - cyclic references are bad
        ObservableCollection<IImageSourceFolder> Children { get; set; }//We need to know when new items are added to the tree control
        Task GetSubFoldersAsync();
        Task<List<RefImage>> GetRefImagesAsync(bool includeSubFolders);
    }

    /// <inheritdoc cref="IImageSourceFolder"/>
    /// <summary>
    /// Represents a folder that was loaded from the FutureAccessList and stores a token identifying it
    /// </summary>
    public interface IFutureAccessImageSource : IImageSourceFolder
    {
        string Token { get; set; }
    }

    public enum ImageSourceType
    {
        Folder,
    }
}
