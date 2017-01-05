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
        #region properties
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

        public ObservableCollection<Issue> _issues;
        public ObservableCollection<Issue> Issues
        {
            get
            {
                return _issues;
            }
            set
            {
                Set(() => Issues, ref _issues, value);
            }
        }

        public ObservableCollection<SearchCode> _searchCodes;
        public ObservableCollection<SearchCode> SearchCodes
        {
            get
            {
                return _searchCodes;
            }
            set
            {
                Set(() => SearchCodes, ref _searchCodes, value);
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

        public bool _isSearchingRepo;
        public bool IsSearchingRepo
        {
            get
            {
                return _isSearchingRepo;
            }
            set
            {
                Set(() => IsSearchingRepo, ref _isSearchingRepo, value);
            }
        }

        public bool _isSearchingCode;
        public bool IsSearchingCode
        {
            get
            {
                return _isSearchingCode;
            }
            set
            {
                Set(() => IsSearchingCode, ref _isSearchingCode, value);
            }
        }

        public bool _isSearchingIssues;
        public bool IsSearchingIssues
        {
            get
            {
                return _isSearchingIssues;
            }
            set
            {
                Set(() => IsSearchingIssues, ref _isSearchingIssues, value);
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

        #endregion

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
                                              IsSearchingRepo = true;

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

        public RelayCommand _searchRepoCommand;
        public RelayCommand SearchRepoCommand
        {
            get
            {
                return _searchRepoCommand
                    ?? (_searchRepoCommand = new RelayCommand(
                                          async () =>
                                          {
                                              IsSearchingRepo = true;
                                              IsSearchingUsers = IsSearchingIssues = IsSearchingCode = false;
                                              if (!string.IsNullOrWhiteSpace(QueryString))
                                              {

                                                  isLoading = true;
                                                  Repositories = await SearchUtility.SearchRepos(QueryString);
                                                  isLoading = false;

                                              }

                                          }));
            }
        }

        public RelayCommand _searchUsersCommand;
        public RelayCommand SearchUsersCommand
        {
            get
            {
                return _searchUsersCommand
                    ?? (_searchUsersCommand = new RelayCommand(
                                          async () =>
                                          {
                                              IsSearchingUsers = true;
                                              IsSearchingRepo = IsSearchingCode = IsSearchingIssues = false;
                                              if (!string.IsNullOrWhiteSpace(QueryString))
                                              {
                                                  isLoading = true;
                                                  Users = await SearchUtility.SearchUsers(QueryString);
                                                  isLoading = false;
                                              }

                                          }));
            }
        }

        public RelayCommand _searchCodeCommand;
        public RelayCommand SearchCodeCommand
        {
            get
            {
                return _searchCodeCommand
                    ?? (_searchCodeCommand = new RelayCommand(
                                          async () =>
                                          {
                                              IsSearchingCode = true;
                                              IsSearchingIssues = IsSearchingRepo = IsSearchingUsers = false;
                                              if (!string.IsNullOrWhiteSpace(QueryString))
                                              {
                                                  isLoading = true;
                                                  SearchCodes = await SearchUtility.SearchCode(QueryString);
                                                  isLoading = false;
                                              }

                                          }));
            }
        }

        public RelayCommand _searchIssuesCommand;
        public RelayCommand SearchIssuesCommand
        {
            get
            {
                return _searchIssuesCommand
                    ?? (_searchIssuesCommand = new RelayCommand(
                                          async () =>
                                          {
                                              IsSearchingIssues = true;
                                              IsSearchingRepo = IsSearchingUsers = IsSearchingCode = false;
                                              if (!string.IsNullOrWhiteSpace(QueryString))
                                              {
                                                  isLoading = true;
                                                  Issues = await SearchUtility.SearchIssues(QueryString);
                                                  isLoading = false;
                                              }

                                          }));
            }
        }

        public void QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            if (IsSearchingRepo)
            {
                SearchRepoCommand.Execute(null);
            }
            else if (IsSearchingUsers)
            {
                SearchUsersCommand.Execute(null);
            }
            else if (IsSearchingIssues)
            {
                SearchIssuesCommand.Execute(null);
            }
            else if (IsSearchingCode)
            {
                SearchCodeCommand.Execute(null);
            }
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
