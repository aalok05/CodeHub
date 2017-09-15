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
using Windows.UI.Xaml;

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

        /// <summary>
        /// All Languages in GitHub
        /// </summary>
        public IEnumerable<Language> AvailableLanguages { get; } = Enum.GetValues(typeof(Language)).Cast<Language>();
        
        public int _SelectedLanguageIndex;
        public int SelectedLanguageIndex
        {
            get
            {
                return _SelectedLanguageIndex;
            }
            set
            {
                Set(() => SelectedLanguageIndex, ref _SelectedLanguageIndex, value);
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
                                              SelectedLanguageIndex = -1;
                                              ChangeVisibilityOfListViews(SelectedSearchItemIndex);

                                              if (!GlobalHelper.IsInternet())
                                              {
                                                  //Sending NoInternet message to all viewModels
                                                  Messenger.Default.Send(new GlobalHelper.LocalNotificationMessageType { Message="No Internet", Glyph= "\uE704" });
                                              }

                                          }));
            }
        }

        public async Task SearchRepos()
        {
            if (!string.IsNullOrWhiteSpace(QueryString))
            {

                isLoading = true;

                if (SelectedLanguageIndex != -1)
                    Repositories = await SearchUtility.SearchRepos(QueryString, (Language)SelectedLanguageIndex);
                else Repositories = await SearchUtility.SearchRepos(QueryString);

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
        }
        public async Task SearchUsers()
        {
            if (!string.IsNullOrWhiteSpace(QueryString))
            {
                isLoading = true;

                if (SelectedLanguageIndex != -1)
                    Users = await SearchUtility.SearchUsers(QueryString, (Language)SelectedLanguageIndex);
                else Users = await SearchUtility.SearchUsers(QueryString);

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
        }
        public async Task SearchCode()
        {
            if (!string.IsNullOrWhiteSpace(QueryString))
            {
                isLoading = true;

                if (SelectedLanguageIndex != -1)
                    SearchCodes = await SearchUtility.SearchCode(QueryString, (Language)SelectedLanguageIndex);
                else SearchCodes = await SearchUtility.SearchCode(QueryString);

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
        }
        public async Task SearchIssues()
        {
            if (!string.IsNullOrWhiteSpace(QueryString))
            {
                isLoading = true;

                if (SelectedLanguageIndex != -1)
                    Issues = await SearchUtility.SearchIssues(QueryString, (Language)SelectedLanguageIndex);
                else Issues = await SearchUtility.SearchIssues(QueryString);

                if (Issues != null)
                {
                    ZeroResultCount = Issues.Count == 0 ? true : false;
                }
                else
                {
                    ZeroResultCount = true;
                }

                isLoading = false;
            }
        }

        public async void SearchItem_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(e.AddedItems.Count != 0)
            {
                ChangeVisibilityOfListViews((int)e.AddedItems.First());
                switch ((int)e.AddedItems.First())
                {
                    case 0: await SearchRepos();
                    break;
                    case 1: await SearchUsers();
                    break;
                    case 2: await SearchIssues();
                    break;
                    case 3: await SearchCode();
                    break;
                }
            }
        }
        public async void Language_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            await SearchResultsReload();
        }
        public async void QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            ChangeVisibilityOfListViews(SelectedSearchItemIndex);
            await SearchResultsReload();
        }
        public void RepoDetailNavigateCommand(object sender, ItemClickEventArgs e)
        {
            SimpleIoc.Default.GetInstance<IAsyncNavigationService>().NavigateAsync(typeof(RepoDetailView), e.ClickedItem as Repository);
        }
        public void UserDetailNavigateCommand(object sender, ItemClickEventArgs e)
        {
            SimpleIoc.Default.GetInstance<IAsyncNavigationService>().NavigateAsync(typeof(DeveloperProfileView), e.ClickedItem as User);
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
            Issue issue = e.ClickedItem as Issue;
            
            /* The 'Repository' field of the Issue is null (Octokit API returns null), 
             * so we have to extract Owner Login and Repository name from the Html Url
             */
            string[] array = issue.HtmlUrl.Split('/');
            string owner = array[3];
            string repo = array[4];

            SimpleIoc.Default.GetInstance<IAsyncNavigationService>().NavigateAsync(typeof(IssueDetailView), new Tuple<string, string, Issue>(owner, repo, e.ClickedItem as Issue));
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

        public async void ResetFilters(object sender, RoutedEventArgs e)
        {
            SelectedLanguageIndex = -1;
            await SearchResultsReload();
        }

        private async Task SearchResultsReload()
        {
            switch (SelectedSearchItemIndex)
            {
                case 0:
                    await SearchRepos();
                    break;
                case 1:
                    await SearchUsers();
                    break;
                case 2:
                    await SearchIssues();
                    break;
                case 3:
                    await SearchCode();
                    break;
            }
        }
    }
}
