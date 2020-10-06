using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.UI.Xaml;

namespace RefOrganiser.Models
{
    public class SelectedSourcesChangedEventArgs : RoutedEventArgs
    {
        public SelectedSourcesChangedEventArgs(List<IImageSource> addedSources, List<IImageSource> removedSources)
        {
            AddedSources = addedSources;
            RemovedSources = removedSources;
        }
        public List<IImageSource> AddedSources { get; set; }
        public List<IImageSource> RemovedSources { get; set; }
    }
}
