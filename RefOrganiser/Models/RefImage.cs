using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using GalaSoft.MvvmLight;
using RefOrganiser.ViewModels;

namespace RefOrganiser.Models
{
    public class RefImage
    {
        /// <summary>
        /// This constructor is used when no cached thumbnail could be found
        /// </summary>
        /// <param name="sourceFile"></param>
        /// <param name="sourceToken"></param>
        public RefImage(StorageFile sourceFile, IImageSource sourceToken)
        {
            ImageFile = sourceFile;
            ImagePath = sourceFile.DisplayName;
            SourceToken = sourceToken;
        }
        //TODO: This needs to be changed to represent the actual source the image is contained in
        public IImageSource SourceToken { get; set; }
        public async Task<BitmapImage> LoadThumbnailAsync()
        {

            BitmapImage bitmapImage = new BitmapImage();
            var thumbnail = await ImageFile.GetThumbnailAsync(ThumbnailMode.PicturesView, 128);

            await bitmapImage.SetSourceAsync(thumbnail);
            //TODO: Check if the image was loaded and if it failed load a different thumbnail
            //TODO: Use placeholders
            return bitmapImage;
        }

        //public IImageSource ImageSource { get; set; }

        //Path to image
        private StorageFile ImageFile { get; set; }
        public string ImagePath { get; set; }

        private async Task GenerateScaledImageAsync(StorageFile imageFile, uint maxDimension, BitmapImage scaledImage)
        {

            using (var imageStream = await ImageFile.OpenReadAsync())
            {
                BitmapDecoder decoder = await BitmapDecoder.CreateAsync(imageStream);
                // create a new stream and encoder for the new image
                InMemoryRandomAccessStream scaledStream = new InMemoryRandomAccessStream();
                //WriteableBitmap writeableBitmap = new WriteableBitmap();

                BitmapEncoder enc = await BitmapEncoder.CreateForTranscodingAsync(scaledStream, decoder);
                // The scale will vary based on settings in the UI
                // Also I need to keep the aspect ratio by reducing the longer dimension to 128 and scaling the other proportionally
                if (decoder.PixelWidth > decoder.PixelHeight)
                {
                    enc.BitmapTransform.ScaledWidth = maxDimension;
                    float ratio = (float)decoder.PixelHeight / decoder.PixelWidth;
                    enc.BitmapTransform.ScaledHeight = (uint)(maxDimension * ratio);
                }
                else
                {
                    enc.BitmapTransform.ScaledHeight = maxDimension;
                    float ratio = (float)decoder.PixelWidth / decoder.PixelHeight;
                    enc.BitmapTransform.ScaledWidth = (uint)(maxDimension * ratio);
                }


                try
                {
                    await enc.FlushAsync();
                }
                catch (Exception ex)
                {
                    string s = ex.ToString();
                }

                // render the stream to a bitmap

                await scaledImage.SetSourceAsync(scaledStream);

            }
        }


        /// <summary>
        /// This method is called when a preview of the image is requested
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public async void LoadPreviewAsync(object sender, RoutedEventArgs e)
        {

            //TODO: Create the preview image and then store result
            Image img = sender as Image;
            Debug.Assert(img != null, "LoadImage event was trigged from a control that was not an image");
            if (img.Source == null)
            {
                BitmapImage scaledImage = new BitmapImage();
                img.Source = scaledImage;
                await GenerateScaledImageAsync(ImageFile, 512, scaledImage);
            }
        }

        public async Task<BitmapImage> LoadCompleteImageAsync()
        {
            using (var imageStream = await ImageFile.OpenReadAsync())
            {
                BitmapImage image = new BitmapImage();
                await image.SetSourceAsync(imageStream);
                return image;
            }
        }

        public async void ToggleImageColorAsync(object sender, TappedRoutedEventArgs e)
        {
            //TODO: Toggle how the image to rendered
            e.Handled = true;
            await Task.CompletedTask;
        }
    }
}
