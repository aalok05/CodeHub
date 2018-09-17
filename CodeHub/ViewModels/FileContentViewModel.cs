using CodeHub.Helpers;
using CodeHub.Services;
using CodeHub.Services.Hilite_me;
using CodeHub.Views;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using Octokit;
using System;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace CodeHub.ViewModels
{
	public class FileContentViewmodel : AppViewmodel
	{
		public Repository _repository;
		public Repository Repository
		{
			get => _repository;
			set => Set(() => Repository, ref _repository, value);
		}

		public bool _isSupportedFile;
		public bool IsSupportedFile
		{
			get => _isSupportedFile;
			set => Set(() => IsSupportedFile, ref _isSupportedFile, value);
		}

		public bool _isImage;
		public bool IsImage
		{
			get => _isImage;
			set => Set(() => IsImage, ref _isImage, value);
		}

		public ImageSource _imageFile;
		public ImageSource ImageFile
		{
			get => _imageFile;
			set => Set(() => ImageFile, ref _imageFile, value);
		}

		public string _path;
		public string Path
		{
			get => _path;
			set => Set(() => Path, ref _path, value);
		}

		// HTMLContent
		public string _HTMLContent;
		public string HTMLContent
		{
			get => _HTMLContent;
			set => Set(() => HTMLContent, ref _HTMLContent, value);
		}

		// Text Content
		public string _TextContent;
		public string TextContent
		{
			get => _TextContent;
			set => Set(() => TextContent, ref _TextContent, value);
		}

		public string _selectedBranch;
		public string SelectedBranch
		{
			get => _selectedBranch;
			set => Set(() => SelectedBranch, ref _selectedBranch, value);
		}

		public Color _HTMLBackgroundColor = SettingsService.Get<bool>(SettingsKeys.AppLightThemeEnabled) ? Colors.White : Colors.Black;

		/// <summary>
		/// Gets the base background color for the HTML content
		/// </summary>
		public Color HTMLBackgroundColor
		{
			get => _HTMLBackgroundColor;
			private set => Set(() => HTMLBackgroundColor, ref _HTMLBackgroundColor, value);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="repoPath">Tuple with Repository, path and branch</param>
		/// <returns></returns>
		public async Task Load(Tuple<Repository, string, string> repoPath)
		{
			IsSupportedFile = true;
			Repository = repoPath.Item1;
			Path = repoPath.Item2;

			if (GlobalHelper.IsInternet())
			{
				IsLoading = true;

				if (string.IsNullOrWhiteSpace(repoPath.Item3))
				{
					SelectedBranch = await RepositoryUtility.GetDefaultBranch(Repository.Id);
				}
				else
				{
					SelectedBranch = repoPath.Item3;
				}

				IsImage = false;

				if (Path.ToLower().EndsWith(".exe") ||
				    Path.ToLower().EndsWith(".pdf") ||
				    Path.ToLower().EndsWith(".ttf") ||
				    Path.ToLower().EndsWith(".suo") ||
				    Path.ToLower().EndsWith(".mp3") ||
				    Path.ToLower().EndsWith(".mp4") ||
				    Path.ToLower().EndsWith(".avi"))
				{
					/*
					 * Unsupported file types
					 */
					IsSupportedFile = false;
					IsLoading = false;
					return;
				}
				if (Path.ToLower().EndsWith(".png") ||
				    Path.ToLower().EndsWith(".jpg") ||
				    Path.ToLower().EndsWith(".jpeg") ||
				    Path.ToLower().EndsWith(".gif"))
				{
					/*
					* Image file types
					*/

					IsImage = true;
					string uri = (await RepositoryUtility.GetRepositoryContentByPath(Repository, Path, SelectedBranch))?[0].Content.DownloadUrl;
					if (!string.IsNullOrWhiteSpace(uri))
					{
						ImageFile = new BitmapImage(new Uri(uri));
					}

					IsLoading = false;
					return;
				}
				if (Path.ToLower().EndsWith(".md"))
				{
					/*
					 *  Files with .md extension
					 */
					TextContent = (await RepositoryUtility.GetRepositoryContentByPath(Repository, Path, SelectedBranch))?[0].Content.Content;
					IsLoading = false;
					return;
				}

				/*
                    *  Code files
                    */

				var content = (await RepositoryUtility.GetRepositoryContentByPath(Repository, Path, SelectedBranch))?[0].Content.Content;
				if (content == null)
				{
					IsSupportedFile = false;
					IsLoading = false;
					return;
				}
				var style = (SyntaxHighlightStyleEnum)SettingsService.Get<int>(SettingsKeys.HighlightStyleIndex);
				var lineNumbers = SettingsService.Get<bool>(SettingsKeys.ShowLineNumbers);
				HTMLContent = await HiliteAPI.TryGetHighlightedCodeAsync(content, Path, style, lineNumbers, CancellationToken.None);

				if (HTMLContent == null)
				{
					/*
					*  Plain text files (Getting HTML for syntax highlighting failed)
					*/

					RepositoryContent result = await RepositoryUtility.GetRepositoryContentTextByPath(Repository, Path, SelectedBranch);
					if (result != null)
					{
						TextContent = result.Content;
					}
				}

				if (HTMLContent == null && TextContent == null)
				{
					IsSupportedFile = false;
				}

				IsLoading = false;

			}
		}

		private RelayCommand _repoDetailNavigateCommand;
		public RelayCommand RepoDetailNavigateCommand
			=> _repoDetailNavigateCommand
			?? (_repoDetailNavigateCommand = new RelayCommand(() =>
														 {
															 SimpleIoc
															 .Default
															 .GetInstance<IAsyncNavigationService>()
															 .NavigateAsync(typeof(RepoDetailView), Repository);
														 }));
	}
}