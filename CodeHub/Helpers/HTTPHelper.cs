using System;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
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
    }
}
