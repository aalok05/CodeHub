using GalaSoft.MvvmLight.Messaging;
using CodeHub.Helpers;
using CodeHub.Services;
using Octokit;
using System;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using CodeHub.Services.Hilite_me;
using MarkdownSharp;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using CodeHub.Views;

namespace CodeHub.ViewModels
{
    public class FileContentViewmodel : AppViewmodel
    {
        public Repository _repository;
        public Repository Repository
        {
            get
            {
                return _repository;
            }
            set
            {
                Set(() => Repository, ref _repository, value);

            }
        }

        public bool _isSupportedFile;
        public bool IsSupportedFile
        {
            get
            {
                return _isSupportedFile;
            }
            set
            {
                Set(() => IsSupportedFile, ref _isSupportedFile, value);

            }
        }

        public bool _isImage;
        public bool IsImage
        {
            get
            {
                return _isImage;
            }
            set
            {
                Set(() => IsImage, ref _isImage, value);

            }
        }

        public ImageSource _imageFile;
        public ImageSource ImageFile
        {
            get
            {
                return _imageFile;
            }
            set
            {
                Set(() => ImageFile, ref _imageFile, value);

            }
        }

        public string _path;
        public string Path
        {
            get
            {
                return _path;
            }
            set
            {
                Set(() => Path, ref _path, value);

            }
        }

        //TextContent
        public string _Textcontent;
        public string TextContent
        {
            get
            {
                return _Textcontent;
            }
            set
            {
                Set(() => TextContent, ref _Textcontent, value);

            }
        }

        // HTMLContent

        public string _HTMLContent;
        public string HTMLContent
        {
            get
            {
                return _HTMLContent;
            }
            set
            {
                Set(() => HTMLContent, ref _HTMLContent, value);

            }
        }


        public string _selectedBranch;
        public string SelectedBranch
        {
            get
            {
                return _selectedBranch;
            }
            set
            {
                Set(() => SelectedBranch, ref _selectedBranch, value);

            }
        }

        public Color _HTMLBackgroundColor = SettingsService.Get<bool>(SettingsKeys.AppLightThemeEnabled) ? Colors.White : Colors.Black;

        /// <summary>
        /// Gets the base background color for the HTML content
        /// </summary>
        public Color HTMLBackgroundColor
        {
            get { return _HTMLBackgroundColor; }
            private set
            { 
                Set(() => HTMLBackgroundColor, ref _HTMLBackgroundColor, value);
            }
        }

        public async Task Load(Tuple<Repository, string, string> repoPath)  //This page recieves RepositoryId and name of the file
        {
            IsSupportedFile = true;
            Repository = repoPath.Item1;
            Path = repoPath.Item2;

            if (string.IsNullOrWhiteSpace(repoPath.Item3))
            {
                SelectedBranch = await RepositoryUtility.GetDefaultBranch(Repository.Id);
            }
            else
            {
                SelectedBranch = repoPath.Item3;
            }


            MarkdownOptions options = new MarkdownOptions
            {
                AsteriskIntraWordEmphasis = true,
                AutoNewlines = true,
                StrictBoldItalic = true,
                AutoHyperlink = false,
                LinkEmails = true
            };
            Markdown markDown = new Markdown(options);

            if (!GlobalHelper.IsInternet())
            {
                Messenger.Default.Send(new GlobalHelper.NoInternetMessageType()); //Sending NoInternet message to all viewModels
            }
            else
            {
                Messenger.Default.Send(new GlobalHelper.HasInternetMessageType()); //Sending Internet available message to all viewModels
                isLoading = true;
                IsImage = false;

                if ((Path.ToLower().EndsWith(".exe")) ||
                   (Path.ToLower().EndsWith(".pdf")) ||
                   (Path.ToLower().EndsWith(".ttf")) ||
                   (Path.ToLower().EndsWith(".suo")) ||
                   (Path.ToLower().EndsWith(".mp3")) ||
                   (Path.ToLower().EndsWith(".mp4")) ||
                   (Path.ToLower().EndsWith(".avi")))
                {
                    /*
                     * Unsupported file types
                     */
                    IsSupportedFile = false;
                    isLoading = false;
                    return;
                }
                if ((Path.ToLower()).EndsWith(".png") ||
                    (Path.ToLower()).EndsWith(".jpg") ||
                    (Path.ToLower()).EndsWith(".jpeg") ||
                    (Path.ToLower().EndsWith(".gif")))
                {
                    /*
                    * Image file types
                    */

                    IsImage = true;
                    var uri = (await RepositoryUtility.GetRepositoryContentByPath(Repository.Id, Path, SelectedBranch))[0].DownloadUrl;
                    ImageFile = new BitmapImage(uri);
                    isLoading = false;
                    return;
                }
                if ((Path.ToLower()).EndsWith(".md"))
                {
                    /*
                     *  Files with .md extension will be shown with full markdown
                     */
                    HTMLBackgroundColor = Colors.White;
                    var str = (await RepositoryUtility.GetRepositoryContentByPath(Repository.Id, Path, SelectedBranch))[0].Content;
                    HTMLContent = "<html><head><meta charset = \"utf-8\" /></head><body style=\"font-family: sans-serif\">" + markDown.Transform(str) + "</body></html>";
                    isLoading = false;
                    return;
                }


                String content = (await RepositoryUtility.GetRepositoryContentByPath(Repository.Id, Path, SelectedBranch))?[0].Content;
                if (content == null)
                {
                    IsSupportedFile = false;
                    isLoading = false;
                    return;
                }
                SyntaxHighlightStyle style = (SyntaxHighlightStyle)SettingsService.Get<int>(SettingsKeys.HighlightStyleIndex);
                bool lineNumbers = SettingsService.Get<bool>(SettingsKeys.ShowLineNumbers);
                HTMLContent = await HiliteAPI.TryGetHighlightedCodeAsync(content, Path, style, lineNumbers, CancellationToken.None);
                IsSupportedFile = HTMLContent != null;
                isLoading = false;

            }
        }

        private RelayCommand _repoDetailNavigateCommand;
        public RelayCommand RepoDetailNavigateCommand
        {
            get
            {
                return _repoDetailNavigateCommand
                    ?? (_repoDetailNavigateCommand = new RelayCommand(
                                          () =>
                                          {
                                              SimpleIoc.Default.GetInstance<Services.INavigationService>().Navigate(typeof(RepoDetailView), Repository, "Repository");
                                          }));
            }
        }
    }
}