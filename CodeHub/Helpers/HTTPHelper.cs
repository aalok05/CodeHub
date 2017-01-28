using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.Web.Http;
using JetBrains.Annotations;

namespace CodeHub.Helpers
{
    /// <summary>
    /// A simple class with some web helper methods
    /// </summary>
    public static class HTTPHelper
    {
        /// <summary>
        /// Downloads a data buffer from the input URL
        /// </summary>
        /// <param name="url">The location of the data to retrieve</param>
        /// <param name="token">The cancellation token for the operation</param>
        [ItemCanBeNull]
        public static async Task<IBuffer> DownloadDataAsync([NotNull] String url, CancellationToken token)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    IAsyncOperationWithProgress<IBuffer, HttpProgress> asyncOperation = client.GetBufferAsync(new Uri(url, UriKind.Absolute));
                    return await asyncOperation.AsTask(token).ContinueWith(t => t.GetAwaiter().GetResult(), token);
                }
            }
            catch
            {
                // Either operation canceled or network error
                return null;
            }
        }

        /// <summary>
        /// Gets the extension for the local cached files
        /// </summary>
        private const String CacheExtension = ".cache";

        private static readonly HashAlgorithmProvider HashProvider = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Sha256);

        /// <summary>
        /// Returns a buffer from the remote URL, loading a cached version if possible
        /// </summary>
        /// <param name="url">The link to the image to retrieve</param>
        /// <param name="token">The cancellation token for the operation</param>
        [ItemCanBeNull]
        public static async Task<IBuffer> GetBufferFromUrlAsync([NotNull] String url, CancellationToken token)
        {
            // Loop to make sure to retry once if the existing cached file is invalid
            while (true)
            {
                // Input check
                if (String.IsNullOrEmpty(url)) return null;
                if (token.IsCancellationRequested) return null;

                // Get the filename for the cache storage
                byte[] bytes = Encoding.Unicode.GetBytes(url);
                IBuffer hash = HashProvider.HashData(bytes.AsBuffer());
                String hex = CryptographicBuffer.EncodeToHexString(hash), cacheFilename = $"{hex}{CacheExtension}";
                StorageFile file = (await ApplicationData.Current.LocalCacheFolder.TryGetItemAsync(cacheFilename)) as StorageFile;

                // Check the cache result
                if (file == null)
                {
                    // Try to get the remote buffer
                    IBuffer buffer = await DownloadDataAsync(url, token);
                    if (buffer == null) return null;

                    // Save the buffer if possible
                    StorageFile cacheFile = await ApplicationData.Current.LocalCacheFolder.CreateFileAsync(cacheFilename, CreationCollisionOption.OpenIfExists);
                    using (IRandomAccessStream outputStream = await cacheFile.OpenAsync(FileAccessMode.ReadWrite))
                    {
                        await outputStream.WriteAsync(buffer);
                        return buffer;
                    }
                }

                // Load the buffer from the cached file
                if (token.IsCancellationRequested) return null;
                using (IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.Read))
                {
                    try
                    {
                        byte[] data = new byte[stream.Size];
                        return await stream.ReadAsync(data.AsBuffer(), (uint)data.Length, InputStreamOptions.None);
                    }
                    catch
                    {
                        // Invalid file
                    }
                }

                // Delete the cached file
                try
                {
                    await file.DeleteAsync(StorageDeleteOption.PermanentDelete);
                }
                catch
                {
                    return null;
                }
            }
        }
    }
}
