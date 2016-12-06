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
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace CodeHub.ViewModels
{
    public class MyReposViewmodel : AppViewmodel
    {
        public bool _zeroRepoCount;
        public bool ZeroRepoCount
        {
            get
            {
                return _zeroRepoCount;
            }
            set
            {
                Set(() => ZeroRepoCount, ref _zeroRepoCount, value);
            }
        }

        public bool _zeroStarRepoCount;
        public bool ZeroStarRepoCount
        {
            get
            {
                return _zeroStarRepoCount;
            }
            set
            {
                Set(() => ZeroStarRepoCount, ref _zeroStarRepoCount, value);
            }
        }

        public ObservableCollection<Repository> _repositories;
        public ObservableCollection<Repository> Repositories
        {
            get
            {
                return _repositories;
            }
            set
            {
                Set(() => Repositories, ref _repositories, value);
            }

        }

        public ObservableCollection<Repository> _starredRepositories;
        public ObservableCollection<Repository> StarredRepositories
        {
            get
            {
                return _starredRepositories;
            }
            set
            {
                Set(() => StarredRepositories, ref _starredRepositories, value);
            }
        }
        public async Task Load()
        {
          
            if (!GlobalHelper.IsInternet())
            {
                Messenger.Default.Send(new GlobalHelper.NoInternetMessageType()); //Sending NoInternet message to all viewModels           
            }
            else
            {
                Messenger.Default.Send(new GlobalHelper.HasInternetMessageType()); //Sending Internet available message to all viewModels

                if (User != null)
                {
                    isLoggedin = true;
                    isLoading = true;
                    if (Repositories == null || StarredRepositories == null)
                    {
                        Repositories = new ObservableCollection<Repository>();
                        StarredRepositories = new ObservableCollection<Repository>();

                        await LoadRepos();
                        await LoadStarRepos();
                        GlobalHelper.NewStarActivity = false;
                    }
                    
                    if (GlobalHelper.NewStarActivity)
                    {
                        await LoadStarRepos();
                        GlobalHelper.NewStarActivity = false;
                    }
                    isLoading = false;
                }
                else
                {
                    isLoggedin = false;
                }
            }

        }
        public async void RefreshCommand(object sender, EventArgs e)
        {
            if (!GlobalHelper.IsInternet())
            {
                Messenger.Default.Send(new GlobalHelper.NoInternetMessageType()); //Sending NoInternet message to all viewModels
            }
            else
            {
                Messenger.Default.Send(new GlobalHelper.HasInternetMessageType()); //Sending Internet available message to all viewModels
                isLoading = true;
                if (User != null)
                {
                    await LoadRepos();
                }
            }
            isLoading = false;
        }
        public async void RefreshStarredCommand(object sender, EventArgs e)
        {
            if (!GlobalHelper.IsInternet())
            {
                Messenger.Default.Send(new GlobalHelper.NoInternetMessageType()); //Sending NoInternet message to all viewModels
            }
            else
            {
                Messenger.Default.Send(new GlobalHelper.HasInternetMessageType()); //Sending Internet available message to all viewModels
                isLoading = true;
                if (User != null)
                {
                    await LoadStarRepos();
                    GlobalHelper.NewStarActivity = false;
                }
            }
            isLoading = false;
        }
        public void RepoDetailNavigateCommand(object sender, ItemClickEventArgs e)
        {
            SimpleIoc.Default.GetInstance<Services.INavigationService>().Navigate(typeof(RepoDetailView), e.ClickedItem as Repository);
        }
       
        public void RecieveSignOutMessage(GlobalHelper.SignOutMessageType empty)
        {
            isLoggedin = false;
            User = null;
            Repositories = null;
            StarredRepositories = null;
        }
        public async void RecieveSignInMessage(User user)
        {
            isLoading = true;
            if (user != null)
            {
                isLoggedin = true;
                User = user;
                await LoadRepos();
                await LoadStarRepos();
                GlobalHelper.NewStarActivity = false;

            }
            isLoading = false;

        }
        private async Task LoadRepos()
        {
            var repos = await UserDataService.getUserRepositories();
            if (repos.Count == 0 || repos == null)
            {
                ZeroRepoCount = true;
                if(Repositories!=null)
                {
                    Repositories.Clear();
                }
            }
            else
            {
                ZeroRepoCount = false;
                Repositories = repos;
            }
        }
        private async Task LoadStarRepos()
        {
            var starred = await UserDataService.getStarredRepositories();
            if (starred.Count == 0 || starred == null)
            {
                ZeroStarRepoCount = true;
                if(StarredRepositories!=null)
                {
                    StarredRepositories.Clear();
                }
            }
            else
            {
                ZeroStarRepoCount = false;
                StarredRepositories = starred;
            }
        }
    }
}
