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

        public ObservableCollection<RepositoryContent> _content;
        public ObservableCollection<RepositoryContent> Content
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
                Messenger.Default.Send(new GlobalHelper.NoInternetMessageType()); 
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
            var item = e.ClickedItem as RepositoryContent;
            if (item.Type == Octokit.ContentType.File)
            {
                SimpleIoc.Default.GetInstance<INavigationService>().Navigate(typeof(FileContentView), new Tuple<Repository, string, string>(Repository, item.Path, SelectedBranch),Repository.FullName);
            }
            else if (item.Type == Octokit.ContentType.Dir)
            {
                SimpleIoc.Default.GetInstance<INavigationService>().Navigate(typeof(ContentView), new Tuple<Repository, string, string>(Repository, item.Path, SelectedBranch), Repository.FullName);
            }
        }
        public async void BranchChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 0)
            {
                if (!GlobalHelper.IsInternet())
                {
                    //Sending NoInternet message to all viewModels
                    Messenger.Default.Send(new GlobalHelper.NoInternetMessageType());
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
                                              SimpleIoc.Default.GetInstance<Services.INavigationService>().Navigate(typeof(RepoDetailView), Repository, "Repository");
                                          }));
            }
        }
    }
}


