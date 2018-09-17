using CodeHub.Models;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.Web.Http;

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
		public static async Task<IBuffer> DownloadDataAsync([NotNull] string url, CancellationToken token)
		{
			try
			{
				using (HttpClient client = new HttpClient())
				{
					var asyncOperation = client.GetBufferAsync(new Uri(url, UriKind.Absolute));
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
		private const string CacheExtension = ".cache";

		// The hash algorithm to use to manage cached files
		private static readonly HashAlgorithmProvider HashProvider = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Sha256);

		/// <summary>
		/// Returns a buffer from the remote URL, loading a cached version if possible
		/// </summary>
		/// <param name="url">The link to the image to retrieve</param>
		/// <param name="token">The cancellation token for the operation</param>
		[ItemCanBeNull]
		public static async Task<IBuffer> GetBufferFromUrlAsync([NotNull] string url, CancellationToken token)
		{
			// URL check
			if (string.IsNullOrEmpty(url))
			{
				return null;
			}

			// Loop to make sure to retry once if the existing cached file is invalid
			while (true)
			{
				// Input check
				if (token.IsCancellationRequested)
				{
					return null;
				}

				// Get the filename for the cache storage
				var bytes = Encoding.Unicode.GetBytes(url);
				var hash = HashProvider.HashData(bytes.AsBuffer());
				string hex = CryptographicBuffer.EncodeToHexString(hash), cacheFilename = $"{hex}{CacheExtension}";

				// Check the cache result
				if (!(await ApplicationData.Current.LocalCacheFolder.TryGetItemAsync(cacheFilename) is StorageFile file))
				{
					// Try to get the remote buffer
					var buffer = await DownloadDataAsync(url, token);
					if (buffer == null)
					{
						return null;
					}

					// Save the buffer if possible
					var cacheFile = await ApplicationData.Current.LocalCacheFolder.CreateFileAsync(cacheFilename, CreationCollisionOption.OpenIfExists);
					using (var outputStream = await cacheFile.OpenAsync(FileAccessMode.ReadWrite))
					{
						await outputStream.WriteAsync(buffer);
						return buffer;
					}
				}

				// Load the buffer from the cached file
				if (token.IsCancellationRequested)
				{
					return null;
				}

				using (var stream = await file.OpenAsync(FileAccessMode.Read))
				{
					try
					{
						var data = new byte[stream.Size];
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

		/// <summary>
		/// Returns the serialized result to a POST call that uses a local cache
		/// </summary>
		/// <param name="url">The POST URL</param>
		/// <param name="parameters">The POST parameters</param>
		/// <param name="token">The cancellation token for the operation</param>
		public static async Task<WrappedHTTPWebResult<string>> POSTWithCacheSupportAsync(
		    [NotNull] string url, [NotNull] IEnumerable<KeyValuePair<string, string>> parameters, CancellationToken token)
		{
			// URL check
			if (string.IsNullOrEmpty(url))
			{
				return new ArgumentException("The URL is invalid");
			}

			// Loop to make sure to retry once if the existing cached file is invalid
			while (true)
			{
				// Input check
				if (token.IsCancellationRequested)
				{
					return new OperationCanceledException("The operation was canceled");
				}

				// Get the filename for the cache storage
				var serialized = parameters.Aggregate(new StringBuilder(url), (b, s) =>
				 {
					 b.Append(s);
					 return b;
				 }).ToString();
				var request = Encoding.Unicode.GetBytes(serialized);
				var hash = HashProvider.HashData(request.AsBuffer());
				string hex = CryptographicBuffer.EncodeToHexString(hash), cacheFilename = $"{hex}{CacheExtension}";

				// Check the cache result
				if (!(await ApplicationData.Current.LocalCacheFolder.TryGetItemAsync(cacheFilename) is StorageFile file))
				{
					// Try to get the remote buffer
					HttpResponseMessage response;
					using (var client = new HttpClient())
					{
						try
						{
							using (IHttpContent content = new HttpFormUrlEncodedContent(parameters))
							{
								// Make the POST call
								response = await client.PostAsync(new Uri(url), content).AsTask(token).ContinueWith(t => t.GetAwaiter().GetResult());
							}
						}
						catch (OperationCanceledException e)
						{
							// Token expired
							return e;
						}
						catch (ArgumentException e)
						{
							// Invalid POST content
							return e;
						}
					}
					if (response == null)
					{
						return new InvalidOperationException("The result content is null");
					}

					// Save the buffer if possible
					try
					{
#if DEBUG
						System.Diagnostics.Debug.WriteLine("[POST] HTTP call made, response cached");
#endif
						if (!response.IsSuccessStatusCode)
						{
							return response.StatusCode;
						}

						string html = await response.Content.ReadAsStringAsync();
						var cacheFile = await ApplicationData.Current.LocalCacheFolder.CreateFileAsync(cacheFilename, CreationCollisionOption.OpenIfExists);
						await FileIO.WriteTextAsync(cacheFile, html);
						return html;
					}
					catch (Exception e)
					{
						return e;
					}
					finally
					{
						// Manually dispose since the response was assigned in a try/catch block
						response.Dispose();
					}
				}

				// Load the buffer from the cached file
				if (token.IsCancellationRequested)
				{
					return new OperationCanceledException("The operation was canceled");
				}

				try
				{
#if DEBUG
					System.Diagnostics.Debug.WriteLine("[POST] Content retrieved from local cache");
#endif
					return await FileIO.ReadTextAsync(file);
				}
				catch
				{
					// Delete the cached file
					await file.DeleteAsync(StorageDeleteOption.PermanentDelete);
				}
			}
		}
	}
}
