using System;
using Windows.UI.Xaml;

namespace RefOrganiser.Models
{

    public class SourcesChangedEventArgs : RoutedEventArgs
    {
        public SourcesChangedEventArgs(IImageSource imageSource)
        {
            ImageSource = imageSource;
        }

        public IImageSource ImageSource { get; set; }
    }
}
