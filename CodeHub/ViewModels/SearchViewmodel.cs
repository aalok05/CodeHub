using CodeHub.Services;
using CodeHub.Views;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using Octokit;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace CodeHub.ViewModels
{
	public class SearchViewmodel : AppViewmodel
	{
		#region properties
		public ObservableCollection<Repository> _repositories;
		public ObservableCollection<Repository> Repositories
		{
			get => _repositories;
			set => Set(() => Repositories, ref _repositories, value);
		}

		public ObservableCollection<User> _users;
		public ObservableCollection<User> Users
		{
			get => _users;
			set => Set(() => Users, ref _users, value);
		}

		public ObservableCollection<Issue> _issues;
		public ObservableCollection<Issue> Issues
		{
			get => _issues;
			set => Set(() => Issues, ref _issues, value);
		}

		public ObservableCollection<SearchCode> _searchCodes;
		public ObservableCollection<SearchCode> SearchCodes
		{
			get => _searchCodes;
			set => Set(() => SearchCodes, ref _searchCodes, value);
		}

		/// <summary>
		/// 'No Results' TextBlock will be displayed if this property is true
		/// </summary>
		public bool _zeroResultCount;
		public bool ZeroResultCount
		{
			get => _zeroResultCount;
			set => Set(() => ZeroResultCount, ref _zeroResultCount, value);
		}

		public string queryString;
		public string QueryString
		{
			get => queryString;
			set => Set(() => QueryString, ref queryString, value);
		}

		public int _resultCount;
		public int ResultCount
		{
			get => _resultCount;
			set => Set(() => ResultCount, ref _resultCount, value);
		}

		public bool _isSearchingRepo;
		public bool IsSearchingRepo
		{
			get => _isSearchingRepo;
			set => Set(() => IsSearchingRepo, ref _isSearchingRepo, value);
		}

		public bool _isSearchingCode;
		public bool IsSearchingCode
		{
			get => _isSearchingCode;
			set => Set(() => IsSearchingCode, ref _isSearchingCode, value);
		}

		public bool _isSearchingIssues;
		public bool IsSearchingIssues
		{
			get => _isSearchingIssues;
			set => Set(() => IsSearchingIssues, ref _isSearchingIssues, value);
		}
		public bool _isSearchingUsers;
		public bool IsSearchingUsers
		{
			get => _isSearchingUsers;
			set => Set(() => IsSearchingUsers, ref _isSearchingUsers, value);
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

		public int _SelectedSearchItemIndex;
		public int SelectedSearchItemIndex
		{
			get => _SelectedSearchItemIndex;
			set => Set(() => SelectedSearchItemIndex, ref _SelectedSearchItemIndex, value);
		}

		/// <summary>
		/// All Languages in GitHub
		/// </summary>
		public IEnumerable<Language> AvailableLanguages { get; } = Enum.GetValues(typeof(Language)).Cast<Language>();

		public int _SelectedLanguageIndex;
		public int SelectedLanguageIndex
		{
			get => _SelectedLanguageIndex;
			set => Set(() => SelectedLanguageIndex, ref _SelectedLanguageIndex, value);
		}
		#endregion

		public RelayCommand _loadCommand;
		public RelayCommand LoadCommand 
			=> _loadCommand
			?? (_loadCommand = new RelayCommand(() =>
											 {
												 ZeroResultCount = true;
												 SelectedSearchItemIndex = 0;
												 SelectedLanguageIndex = -1;
												 ChangeVisibilityOfListViews(SelectedSearchItemIndex);

											 }));

		public async Task SearchRepos()
		{
			if (!string.IsNullOrWhiteSpace(QueryString))
			{

				IsLoading = true;

				if (SelectedLanguageIndex != -1)
				{
					Repositories = await SearchUtility.SearchRepos(QueryString, (Language)SelectedLanguageIndex);
				}
				else
				{
					Repositories = await SearchUtility.SearchRepos(QueryString);
				}

				if (Repositories != null)
				{
					ZeroResultCount = Repositories.Count == 0 ? true : false;
				}
				else
				{
					ZeroResultCount = true;
				}

				IsLoading = false;

			}
		}
		public async Task SearchUsers()
		{
			if (!string.IsNullOrWhiteSpace(QueryString))
			{
				IsLoading = true;

				if (SelectedLanguageIndex != -1)
				{
					Users = await SearchUtility.SearchUsers(QueryString, (Language)SelectedLanguageIndex);
				}
				else
				{
					Users = await SearchUtility.SearchUsers(QueryString);
				}

				if (Users != null)
				{
					ZeroResultCount = Users.Count == 0 ? true : false;
				}
				else
				{
					ZeroResultCount = true;
				}

				IsLoading = false;
			}
		}
		public async Task SearchCode()
		{
			if (!string.IsNullOrWhiteSpace(QueryString))
			{
				IsLoading = true;

				if (SelectedLanguageIndex != -1)
				{
					SearchCodes = await SearchUtility.SearchCode(QueryString, (Language)SelectedLanguageIndex);
				}
				else
				{
					SearchCodes = await SearchUtility.SearchCode(QueryString);
				}

				if (SearchCodes != null)
				{
					ZeroResultCount = SearchCodes.Count == 0 ? true : false;
				}
				else
				{
					ZeroResultCount = true;
				}

				IsLoading = false;
			}
		}
		public async Task SearchIssues()
		{
			if (!string.IsNullOrWhiteSpace(QueryString))
			{
				IsLoading = true;

				if (SelectedLanguageIndex != -1)
				{
					Issues = await SearchUtility.SearchIssues(QueryString, (Language)SelectedLanguageIndex);
				}
				else
				{
					Issues = await SearchUtility.SearchIssues(QueryString);
				}

				if (Issues != null)
				{
					ZeroResultCount = Issues.Count == 0 ? true : false;
				}
				else
				{
					ZeroResultCount = true;
				}

				IsLoading = false;
			}
		}

		public async void SearchItem_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (e.AddedItems.Count != 0)
			{
				ChangeVisibilityOfListViews((int)e.AddedItems.First());
				switch ((int)e.AddedItems.First())
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
		public async void Language_SelectionChanged(object sender, SelectionChangedEventArgs e) 
			=> await SearchResultsReload();
		public async void QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
		{
			ChangeVisibilityOfListViews(SelectedSearchItemIndex);
			await SearchResultsReload();
		}
		public void RepoDetailNavigateCommand(object sender, ItemClickEventArgs e) 
			=> SimpleIoc
				.Default
				.GetInstance<IAsyncNavigationService>()
				.NavigateAsync(typeof(RepoDetailView), e.ClickedItem as Repository);

		public void UserDetailNavigateCommand(object sender, ItemClickEventArgs e) 
			=> SimpleIoc
				.Default
				.GetInstance<IAsyncNavigationService>()
				.NavigateAsync(typeof(DeveloperProfileView), e.ClickedItem as User);

		public void CodeNavigate(object sender, ItemClickEventArgs e)
		{
			if (e.ClickedItem is SearchCode item)
			{
				SimpleIoc
					.Default
					.GetInstance<IAsyncNavigationService>()
					.NavigateAsync(typeof(FileContentView), item.Repository.FullName, (item.Repository, item.Path, item.Repository.DefaultBranch));
			}
		}

		public void IssueNavigate(object sender, ItemClickEventArgs e)
		{
			Issue issue = e.ClickedItem as Issue;

			/* The 'Repository' field of the Issue is null (Octokit API returns null), 
			 * so we have to extract Owner Login and Repository name from the Html Url
			 */
			var array = issue.HtmlUrl.Split('/');
			var owner = array[3];
			var repo = array[4];

			SimpleIoc
				.Default
				.GetInstance<IAsyncNavigationService>()
				.NavigateAsync(typeof(IssueDetailView), (owner, repo, e.ClickedItem as Issue));
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
