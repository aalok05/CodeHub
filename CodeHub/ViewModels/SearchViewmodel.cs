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
        public enum SearchItems
        {
            Repositories,
            Users,
            Issues,
            Code
        }

        /// <summary>
        /// Gets the collection of the available search items
        /// </summary>
        public IEnumerable<SearchItems> AvailableSearchItems { get; } = Enum.GetValues(typeof(SearchItems)).Cast<SearchItems>();

        public int _SelectedSearchItemIndex ;
        public int SelectedSearchItemIndex
        {
            get
            {
                return _SelectedSearchItemIndex;
            }
            set
            {
                Set(() => SelectedSearchItemIndex, ref _SelectedSearchItemIndex, value);
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
                                              SelectedSearchItemIndex = 0;
                                              ChangeVisibilityOfListViews(SelectedSearchItemIndex);

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
                                              if (!string.IsNullOrWhiteSpace(QueryString))
                                              {

                                                  isLoading = true;
                                                  Repositories = await SearchUtility.SearchRepos(QueryString);
                                                  if (Repositories != null)
                                                  {
                                                      ZeroResultCount = Repositories.Count == 0 ? true : false;
                                                  }
                                                  else
                                                  {
                                                      ZeroResultCount = true;
                                                  }
                                                  
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
                                              if (!string.IsNullOrWhiteSpace(QueryString))
                                              {
                                                  isLoading = true;
                                                  Users = await SearchUtility.SearchUsers(QueryString);
                                                  if (Users != null)
                                                  {
                                                       ZeroResultCount = Users.Count == 0 ? true : false;
                                                  }
                                                  else
                                                  {
                                                      ZeroResultCount = true;
                                                  }
                                                 
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
                                              if (!string.IsNullOrWhiteSpace(QueryString))
                                              {
                                                  isLoading = true;
                                                  SearchCodes = await SearchUtility.SearchCode(QueryString);
                                                  if (SearchCodes != null)
                                                  {
                                                      ZeroResultCount = SearchCodes.Count == 0 ? true : false;
                                                  }
                                                  else
                                                  {
                                                      ZeroResultCount = true;
                                                  }
                                                  
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
                                              if (!string.IsNullOrWhiteSpace(QueryString))
                                              {
                                                  isLoading = true;
                                                  Issues = await SearchUtility.SearchIssues(QueryString);
                                                  if(Issues!=null)
                                                  {
                                                      ZeroResultCount = Issues.Count == 0 ? true : false;
                                                  }
                                                  else
                                                  {
                                                      ZeroResultCount = true;
                                                  }
                                                  
                                                  isLoading = false;
                                              }

                                          }));
            }
        }

        public void SearchItem_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(e.AddedItems.Count != 0)
            {
                ChangeVisibilityOfListViews((int)e.AddedItems.First());
                switch ((int)e.AddedItems.First())
                {
                    case 0:
                        if(Repositories == null)
                            SearchRepoCommand.Execute(null);
                        break;
                    case 1:
                        if (Users == null)
                            SearchUsersCommand.Execute(null);
                        break;
                    case 2:
                        if (Issues == null)
                            SearchIssuesCommand.Execute(null);
                        break;
                    case 3:
                        if (SearchCodes == null)
                            SearchCodeCommand.Execute(null);
                        break;
                }
            }
        }
        public void QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            ChangeVisibilityOfListViews(SelectedSearchItemIndex);
            switch (SelectedSearchItemIndex)
            {
                case 0:
                    SearchRepoCommand.Execute(null);
                    break;
                case 1:
                    SearchUsersCommand.Execute(null);
                    break;
                case 2:
                    SearchIssuesCommand.Execute(null);
                    break;
                case 3:
                    SearchCodeCommand.Execute(null);
                    break;
            }
        }
        public void RepoDetailNavigateCommand(object sender, ItemClickEventArgs e)
        {
            SimpleIoc.Default.GetInstance<Services.IAsyncNavigationService>().NavigateAsync(typeof(RepoDetailView),"Repository", e.ClickedItem as Repository);
        }
        public void UserDetailNavigateCommand(object sender, ItemClickEventArgs e)
        {
            SimpleIoc.Default.GetInstance<Services.IAsyncNavigationService>().NavigateAsync(typeof(DeveloperProfileView), "Profile", (e.ClickedItem as User).Login);
        }
        public void CodeNavigate(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as SearchCode;
            if (item != null)
            {
                SimpleIoc.Default.GetInstance<IAsyncNavigationService>().NavigateAsync(typeof(FileContentView),item.Repository.FullName, new Tuple<Repository, string, string>(item.Repository, item.Path, item.Repository.DefaultBranch));
            }
        }
        public void IssueNavigate(object sender, ItemClickEventArgs e)
        {
            var issue = e.ClickedItem as Issue;

            /* The 'Repository' field of the Issue is null (Octokit API returns null), 
             * so we have to extract Owner Login and Repository name from the Html Url
             */
            string owner = (issue.HtmlUrl.Segments[1]).Replace("/", "");
            string repo = issue.HtmlUrl.Segments[2].Replace("/", "");

            SimpleIoc.Default.GetInstance<IAsyncNavigationService>().NavigateAsync(typeof(IssueDetailView),"Issues", new Tuple<string, string, Issue>(owner, repo, e.ClickedItem as Issue));
        }

        private void ChangeVisibilityOfListViews(int selectedSearchItemIndex)
        {
            IsSearchingRepo = IsSearchingUsers = IsSearchingIssues = IsSearchingCode = false;
            switch (selectedSearchItemIndex)
            {
                case 0:
                    IsSearchingRepo = true;
                    break;
                case 1:
                    IsSearchingUsers = true;
                    break;
                case 2:
                    IsSearchingIssues = true;
                    break;
                case 3:
                    IsSearchingCode = true;
                    break;
            }
        }

    }
}
