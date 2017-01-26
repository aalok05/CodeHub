using System;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using JetBrains.Annotations;
using Lumia.Imaging;
using Lumia.Imaging.Adjustments;

namespace CodeHub.Helpers
{
    /// <summary>
    /// A static class with some methods to manipulate images
    /// </summary>
    public static class ImageHelper
    {
        /// <summary>
        /// Loads an image and returns it and a blurred copy
        /// </summary>
        /// <param name="buffer">The pixel data of the image to load</param>
        /// <param name="blur">The amount of blur to apply</param>
        [ItemCanBeNull]
        public static async Task<Tuple<ImageSource, ImageSource>> GetImageAndBlurredCopyFromPixelDataAsync([NotNull] IBuffer buffer, int blur)
        {
            // Check if the input is valid
            if (buffer.Length == 0) return null;

            // Apply the blur effect on a copy of the original image
            using (Stream imageStream = buffer.AsStream())
            using (IRandomAccessStream randomImageStream = imageStream.AsRandomAccessStream())
            {
                // Load the default image
                BitmapImage original = new BitmapImage();
                await original.SetSourceAsync(randomImageStream);

                // Blur the copy of the image
                randomImageStream.Seek(0);
                using (RandomAccessStreamImageSource imageProvider = new RandomAccessStreamImageSource(randomImageStream))
                using (BlurEffect blurEffect = new BlurEffect(imageProvider) { KernelSize = blur })
                {
                    // Process the blurred image
                    WriteableBitmap blurred = new WriteableBitmap((int)original.PixelWidth, (int)original.PixelHeight);
                    await blurEffect.GetBitmapAsync(blurred, OutputOption.Stretch);

                    // Return the two images
                    return Tuple.Create<ImageSource, ImageSource>(original, blurred);
                }
            }
        }

        /// <summary>
        /// Blurs a single image from a data stream
        /// </summary>
        /// <param name="buffer">The buffer that contains the data of the image to blur</param>
        /// <param name="blur">The amount of blur to apply</param>
        [ItemCanBeNull]
        public static async Task<ImageSource> BlurImageAsync([NotNull] IBuffer buffer, int blur)
        {
            using (Stream imageStream = buffer.AsStream())
            using (IRandomAccessStream randomImageStream = imageStream.AsRandomAccessStream())
            {
                BitmapDecoder decoder;
                try
                {
                    decoder = await BitmapDecoder.CreateAsync(randomImageStream);
                }
                catch
                {
                    // Invalid image data
                    return null;
                }
                randomImageStream.Seek(0);
                using (RandomAccessStreamImageSource imageProvider = new RandomAccessStreamImageSource(randomImageStream))
                using (BlurEffect blurEffect = new BlurEffect(imageProvider) { KernelSize = blur })
                {
                    WriteableBitmap blurred = new WriteableBitmap((int)decoder.PixelWidth, (int)decoder.PixelHeight);
                    return await blurEffect.GetBitmapAsync(blurred, OutputOption.Stretch);
                }
            }
        }
    }
}
