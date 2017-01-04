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

namespace CodeHub.ViewModels
{
    public class SearchViewmodel : AppViewmodel
    {
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

        public ObservableCollection<User> _users;
        public ObservableCollection<User> Users
        {
            get
            {
                return _users;
            }
            set
            {
                Set(() => Users, ref _users, value);
            }
        }

        /// <summary>
        /// 'No Results' TextBlock will be displayed if this property is true
        /// </summary>
        public bool _zeroResultCount;
        public bool ZeroResultCount
        {
            get
            {
                return _zeroResultCount;
            }
            set
            {
                Set(() => ZeroResultCount, ref _zeroResultCount, value);
            }
        }

        public bool _isSearchingUsers;
        public bool IsSearchingUsers
        {
            get
            {
                return _isSearchingUsers;
            }
            set
            {
                Set(() => IsSearchingUsers, ref _isSearchingUsers, value);
            }
        }

        public string queryString;
        public string QueryString
        {
            get
            {
                return queryString;
            }
            set
            {
                Set(() => QueryString, ref queryString, value);
            }
        }

        public int _resultCount;
        public int ResultCount
        {
            get
            {
                return _resultCount;
            }
            set
            {
                Set(() => ResultCount, ref _resultCount, value);
            }
        }

        public RelayCommand _loadCommand;
        public RelayCommand LoadCommand
        {
            get
            {
                return _loadCommand
                    ?? (_loadCommand = new RelayCommand(
                                          () =>
                                          {
                                              ZeroResultCount = true;

                                              if (!GlobalHelper.IsInternet())
                                              {
                                                  //Sending NoInternet message to all viewModels
                                                  Messenger.Default.Send(new GlobalHelper.NoInternetMessageType()); 
                                              }
                                              else
                                              {
                                                  //Sending Internet available message to all viewModels
                                                  Messenger.Default.Send(new GlobalHelper.HasInternetMessageType()); 
                                              }

                                          }));
            }
        }

        public RelayCommand _searchCommand;
        public RelayCommand SearchCommand
        {
            get
            {
                return _searchCommand
                    ?? (_searchCommand = new RelayCommand(
                                          async () =>
                                          {
                                              if (!string.IsNullOrWhiteSpace(QueryString))
                                              {
                                                  if (IsSearchingUsers)
                                                  {
                                                      isLoading = true;
                                                      Users = await SearchUtility.SearchUsers(QueryString);
                                                      isLoading = false;

                                                      if (Users.Count == 0)
                                                      {
                                                          ZeroResultCount = true;
                                                      }
                                                      else
                                                      {
                                                          ZeroResultCount = false;
                                                      }
                                                  }
                                                  else
                                                  {
                                                      isLoading = true;
                                                      Repositories = await SearchUtility.SearchRepos(QueryString);
                                                      isLoading = false;

                                                      if (Repositories.Count == 0)
                                                      {
                                                          ZeroResultCount = true;
                                                      }
                                                      else
                                                      {
                                                          ZeroResultCount = false;
                                                      }
                                                          
                                                  }
                                              }

                                          }));
            }
        }
        public async void RefreshReposCommand(object sender, EventArgs e)
        {
            if (!GlobalHelper.IsInternet())
            {
                Messenger.Default.Send(new GlobalHelper.NoInternetMessageType()); //Sending NoInternet message to all viewModels
            }
            else
            {
                Messenger.Default.Send(new GlobalHelper.HasInternetMessageType()); //Sending Internet available message to all viewModels
                isLoading = true;
                Repositories = await SearchUtility.SearchRepos(QueryString);
                isLoading = false;
            }


        }
        public async void RefreshUsersCommand(object sender, EventArgs e)
        {
            if (!GlobalHelper.IsInternet())
            {
                Messenger.Default.Send(new GlobalHelper.NoInternetMessageType()); //Sending NoInternet message to all viewModels
            }
            else
            {
                Messenger.Default.Send(new GlobalHelper.HasInternetMessageType()); //Sending Internet available message to all viewModels
                isLoading = true;
                Users = await SearchUtility.SearchUsers(QueryString);
                isLoading = false;
            }


        }
        public void QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
             SearchCommand.Execute(null);
        }
        public void RepoDetailNavigateCommand(object sender, ItemClickEventArgs e)
        {
            SimpleIoc.Default.GetInstance<Services.INavigationService>().Navigate(typeof(RepoDetailView), e.ClickedItem as Repository);
        }
        public void UserDetailNavigateCommand(object sender, ItemClickEventArgs e)
        {
            SimpleIoc.Default.GetInstance<Services.INavigationService>().Navigate(typeof(DeveloperProfileView), (e.ClickedItem as User).Login);
        }

    }
}
