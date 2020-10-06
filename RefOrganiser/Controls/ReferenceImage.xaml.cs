using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.Foundation.Metadata;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;
using RefOrganiser.Annotations;
using RefOrganiser.Models;
using RefOrganiser.Services;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace RefOrganiser.Controls
{
    public sealed partial class ReferenceImage : UserControl, INotifyPropertyChanged
    {
        public ReferenceImage()
        {
            this.InitializeComponent();
            AppService = InjectorService.Default.GetInstance<AppService>();
        }

        public AppService AppService { get; set; }

        public string ImagePath
        {
            get => _imagePath;

            set
            {
                _imagePath = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Executes when the user control has completed loading
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public async void ReferenceLoaded(object sender, RoutedEventArgs e)
        {
            //ThumbnailImage.Source = await Reference.LoadThumbnailAsync();
            ThumbnailImage.Source = await Reference.LoadThumbnailAsync();
        }

        public static readonly DependencyProperty ReferenceProperty = DependencyProperty.Register(
            "Reference", typeof(RefImage), typeof(ReferenceImage), new PropertyMetadata(default(RefImage), PropertyChangedCallback));

        private static void PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ReferenceImage refImage)
            {
                refImage.OnPropertyChanged(e.Property.ToString());
                refImage.ReferenceLoaded(null, new RoutedEventArgs());
                refImage.ImagePath = refImage.Reference.ImagePath;
            }
        }

        private string _imagePath;

        public RefImage Reference
        {
            get => (RefImage)GetValue(ReferenceProperty);
            set
            {
                SetValue(ReferenceProperty, value);
                OnPropertyChanged();
                OnPropertyChanged(nameof(ImagePath));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected override void OnPointerEntered(PointerRoutedEventArgs e)
        {
            base.OnPointerEntered(e);
            // Only show hover buttons when the user is using mouse or pen.
            if (e.Pointer.PointerDeviceType == Windows.Devices.Input.PointerDeviceType.Mouse || e.Pointer.PointerDeviceType == Windows.Devices.Input.PointerDeviceType.Pen)
            {
                VisualStateManager.GoToState(this, "HoverButtonsShown", true);
            }
            
        }

        protected override void OnPointerExited(PointerRoutedEventArgs e)
        {
            base.OnPointerExited(e);
            VisualStateManager.GoToState(this, "HoverButtonsHidden", true);
        }

        /// <summary>
        /// This event occurs when an image is tapped, so not one when of the overlayed buttons are pressed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public async void ImageTapped(object sender, TappedRoutedEventArgs e)
        {
            //TODO: This can be disabled if using the overlay thing and maybe full screen as well
            //Prepare content transition
            ConnectedAnimationService.GetForCurrentView().PrepareToAnimate("imageTransition", ThumbnailImage);

            if (e.OriginalSource is FrameworkElement itemClicked)
            {
                var refImage = itemClicked.DataContext as RefImage;
                await refImage.LoadCompleteImageAsync();//TODO: Need to change this, the new page can be responsible for loading the image
                AppService.NavigationService.NavigateTo(Pages.ShowImagePage, refImage);
            }
            e.Handled = true;
        }

        public async void DisplayAsPiP(object sender, TappedRoutedEventArgs e)
        {
            //Here we display the image in either PiP or Full screen
            // ReSharper disable once UseNameofExpression - If we could use nameof we wouldn't need to check for the API
            var compactViewAvailable = ApiInformation.IsEnumNamedValuePresent("Windows.UI.ViewManagement.ApplicationViewMode",
                "CompactOverlay");
            //TODO: Move this to the app service so it only needs to be checked once and used through the lifetime of the app
            if (compactViewAvailable)
            {
                if (ApplicationView.GetForCurrentView().IsViewModeSupported(ApplicationViewMode.CompactOverlay))
                {
                    //Navigate to display image page
                    if (e.OriginalSource is FrameworkElement itemClicked)
                    {
                        var refImage = itemClicked.DataContext as RefImage;
                        await refImage.LoadCompleteImageAsync();
                        AppService.NavigationService.NavigateTo(Pages.ShowImagePage, refImage);
                        //we can use overlay mode
                        ViewModePreferences compactOptions = ViewModePreferences.CreateDefault(ApplicationViewMode.CompactOverlay);

                        compactOptions.CustomSize = new Windows.Foundation.Size(1000, 1000);
                        bool modeSwitched = await ApplicationView.GetForCurrentView().TryEnterViewModeAsync(ApplicationViewMode.CompactOverlay, compactOptions);
                    }
                }
            }
            e.Handled = true;
        }
    }
}
