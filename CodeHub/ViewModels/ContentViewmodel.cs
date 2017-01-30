using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using CodeHub.Helpers;
using CodeHub.Services;
using CodeHub.Views;
using Octokit;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using CodeHub.Models;

namespace CodeHub.ViewModels
{
    public class ContentViewmodel : AppViewmodel
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

        public ObservableCollection<RepositoryContentWithCommitInfo> _content;
        public ObservableCollection<RepositoryContentWithCommitInfo> Content
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

        public async Task Load(Tuple<Repository, string, string> repoPath)  //This page recieves Repository and Path
        {
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

            if (!GlobalHelper.IsInternet())
            {
                Messenger.Default.Send(new GlobalHelper.NoInternetMessageType()); //Sending NoInternet message to all viewModels
            }
            else
            {
                Messenger.Default.Send(new GlobalHelper.HasInternetMessageType()); //Sending Internet available message to all viewModels
                isLoading = true;
                Content = await RepositoryUtility.GetRepositoryContentByPath(Repository.Id, Path, SelectedBranch);

                isLoading = false;

            }
        }
        public void RepoContentDrillNavigate(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as RepositoryContent;
            if (item != null)
            {
                if (item.Type == ContentType.File)
                {
                    SimpleIoc.Default.GetInstance<INavigationService>().Navigate(typeof(FileContentView), new Tuple<Repository, string, string>(Repository, item.Path, SelectedBranch), Repository.FullName);
                }
                else if (item.Type == ContentType.Dir)
                {
                    SimpleIoc.Default.GetInstance<INavigationService>().Navigate(typeof(ContentView), new Tuple<Repository, string, string>(Repository, item.Path, SelectedBranch), Repository.FullName);
                }
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
