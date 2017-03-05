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
using Windows.UI.Xaml.Input;
using CodeHub.Models;

namespace CodeHub.ViewModels
{
    public class SourceCodeViewmodel : AppViewmodel
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

        public ObservableCollection<string> _branches;
        public ObservableCollection<string> Branches
        {
            get
            {
                return _branches;
            }
            set
            {
                Set(() => Branches, ref _branches, value);

            }
        }
        public async Task Load(Repository repo)
        {

            if (!GlobalHelper.IsInternet())
            {
                //Sending NoInternet message to all viewModels
                Messenger.Default.Send(new GlobalHelper.LocalNotificationMessageType { Message = "No Internet", Glyph = "\uE704" });
            }
            else
            {
                //Sending Internet available message to all viewModels
                Messenger.Default.Send(new GlobalHelper.HasInternetMessageType());

                isLoading = true;
                if (repo != Repository)
                {
                    Repository = repo;

                    if (string.IsNullOrWhiteSpace(Repository.DefaultBranch))
                    {
                        SelectedBranch = await RepositoryUtility.GetDefaultBranch(Repository.Id);
                    }
                    else
                    {
                        SelectedBranch = Repository.DefaultBranch;
                    }

                    Branches = await RepositoryUtility.GetAllBranches(Repository);
                    Content = await RepositoryUtility.GetRepositoryContent(Repository, SelectedBranch);
                }
                isLoading = false;
            }

        }

        public void RepoContentDrillNavigate(object sender, ItemClickEventArgs e)
        {
            RepositoryContent item = e.ClickedItem as RepositoryContentWithCommitInfo;
            if (item.Type == Octokit.ContentType.File)
            {
                SimpleIoc.Default.GetInstance<IAsyncNavigationService>().NavigateWithoutAnimations(typeof(FileContentView),Repository.FullName, new Tuple<Repository, string, string>(Repository, item.Path, SelectedBranch));
            }
            else if (item.Type == Octokit.ContentType.Dir)
            {
                SimpleIoc.Default.GetInstance<IAsyncNavigationService>().NavigateWithoutAnimations(typeof(ContentView), Repository.FullName, new Tuple<Repository, string, string>(Repository, item.Path, SelectedBranch));
            }
        }
        public async void BranchChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 0)
            {
                if (!GlobalHelper.IsInternet())
                {
                    //Sending NoInternet message to all viewModels
                    Messenger.Default.Send(new GlobalHelper.LocalNotificationMessageType { Message = "No Internet", Glyph = "\uE704" });
                }
                else
                {
                    //Sending Internet available message to all viewModels
                    Messenger.Default.Send(new GlobalHelper.HasInternetMessageType()); 
                    isLoading = true;
                    SelectedBranch = e.AddedItems.First().ToString();
                    Content = await RepositoryUtility.GetRepositoryContent(Repository, SelectedBranch);
                    isLoading = false;
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
                                              SimpleIoc.Default.GetInstance<Services.IAsyncNavigationService>().NavigateAsync(typeof(RepoDetailView), "Repository", Repository);
                                          }));
            }
        }
    }
}


