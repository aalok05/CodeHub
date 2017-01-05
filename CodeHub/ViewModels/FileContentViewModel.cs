using GalaSoft.MvvmLight.Messaging;
using CodeHub.Helpers;
using CodeHub.Services;
using Octokit;
using System;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using MarkdownSharp;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using CodeHub.Views;

namespace CodeHub.ViewModels
{
    public class FileContentViewModel : AppViewmodel
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

        public bool _isReadme;
        public bool IsReadme
        {
            get
            {
                return _isReadme;
            }
            set
            {
                Set(() => IsReadme, ref _isReadme, value);

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

        public string _content;
        public string Content
        {
            get
            {
                return _content;
            }
            set
            {
                Set(() => Content, ref _content, value);

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

        public async Task Load(Tuple<Repository, string, string> repoPath)  //This page recieves RepositoryId and name of the file
        {
            Content = "";
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

                IsReadme = false;
                IsImage = false;

                if ((Path.ToLower().EndsWith(".exe")) ||
                   (Path.ToLower().EndsWith(".pdf")) ||
                   (Path.ToLower().EndsWith(".ttf")))
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
                    IsReadme = true;

                    var str = (await RepositoryUtility.GetRepositoryContentByPath(Repository.Id, Path, SelectedBranch))[0].Content;
                    Content = "<html><head><meta charset = \"utf-8\" /></head><body style=\"font-family: sans-serif\">" + markDown.Transform(str) + "</body></html>";
                    isLoading = false;
                    return;
                }

                Content = (await RepositoryUtility.GetRepositoryContentByPath(Repository.Id, Path, SelectedBranch))[0].Content;
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
                                              SimpleIoc.Default.GetInstance<Services.INavigationService>().Navigate(typeof(RepoDetailView), Repository);
                                          }));
            }
        }
    }
}