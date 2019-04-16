using CodeHub.Helpers;
using CodeHub.Services;
using CodeHub.Views;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using Octokit;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace CodeHub.ViewModels
{
	public class TrendingViewmodel : AppViewmodel
	{
		#region properties
		public enum TimeRange
		{
			TODAY, WEEKLY, MONTHLY
		}

		public bool _zeroTodayCount;
		/// <summary>
		/// 'Trending Repositories are being updated by Github' textblock will be displayed if this is true
		/// </summary>
		public bool ZeroTodayCount
		{
			get => _zeroTodayCount;
			set => Set(() => ZeroTodayCount, ref _zeroTodayCount, value);
		}
		public bool _zeroWeeklyCount;
		/// <summary>
		/// 'Trending Repositories are being updated by Github' textblock will be displayed if this is true
		/// </summary>
		public bool ZeroWeeklyCount
		{
			get => _zeroWeeklyCount;
			set => Set(() => ZeroWeeklyCount, ref _zeroWeeklyCount, value);
		}
		public bool _zeroMonthlyCount;
		/// <summary>
		/// 'Trending Repositories are being updated by Github' textblock will be displayed if this is true
		/// </summary>
		public bool ZeroMonthlyCount
		{
			get => _zeroMonthlyCount;
			set => Set(() => ZeroMonthlyCount, ref _zeroMonthlyCount, value);
		}
		public bool _CanLoadMoreToday;

		public bool CanLoadMoreToday
		{
			get => _CanLoadMoreToday;
			set => Set(() => CanLoadMoreToday, ref _CanLoadMoreToday, value);
		}
		public bool _CanLoadMoreWeek;

		public bool CanLoadMoreWeek
		{
			get => _CanLoadMoreWeek;
			set => Set(() => CanLoadMoreWeek, ref _CanLoadMoreWeek, value);
		}
		public bool _CanLoadMoreMonth;

		public bool CanLoadMoreMonth
		{
			get => _CanLoadMoreMonth;
			set => Set(() => CanLoadMoreMonth, ref _CanLoadMoreMonth, value);
		}

		public Repository _firstTrendingReposToday;
		public Repository FirstTrendingReposToday
		{
			get => _firstTrendingReposToday;
			set => Set(() => FirstTrendingReposToday, ref _firstTrendingReposToday, value);
		}
		public Repository _firstTrendingReposWeek;
		public Repository FirstTrendingReposWeek
		{
			get => _firstTrendingReposWeek;
			set => Set(() => FirstTrendingReposWeek, ref _firstTrendingReposWeek, value);
		}
		public Repository _firstTrendingReposMonth;
		public Repository FirstTrendingReposMonth
		{
			get => _firstTrendingReposMonth;
			set => Set(() => FirstTrendingReposMonth, ref _firstTrendingReposMonth, value);
		}

		public ObservableCollection<Repository> _trendingReposToday;
		public ObservableCollection<Repository> TrendingReposToday
		{
			get => _trendingReposToday;
			set => Set(() => TrendingReposToday, ref _trendingReposToday, value);

		}

		public ObservableCollection<Repository> _trendingReposWeek;
		public ObservableCollection<Repository> TrendingReposWeek
		{
			get => _trendingReposWeek;
			set => Set(() => TrendingReposWeek, ref _trendingReposWeek, value);

		}

		public ObservableCollection<Repository> _trendingReposMonth;
		public ObservableCollection<Repository> TrendingReposMonth
		{
			get => _trendingReposMonth;
			set => Set(() => TrendingReposMonth, ref _trendingReposMonth, value);

		}

		public bool _isIncrementalLoadingToday;

		/// <summary>
		/// Indicates visibility of progressbar for incremental loading
		/// </summary>
		public bool IsIncrementalLoadingToday
		{
			get => _isIncrementalLoadingToday;
			set => Set(() => IsIncrementalLoadingToday, ref _isIncrementalLoadingToday, value);
		}
		public bool _isIncrementalLoadingWeek;

		/// <summary>
		/// Indicates visibility of progressbar for incremental loading
		/// </summary>
		public bool IsIncrementalLoadingWeek
		{
			get => _isIncrementalLoadingWeek;
			set => Set(() => IsIncrementalLoadingWeek, ref _isIncrementalLoadingWeek, value);
		}
		public bool _isIncrementalLoadingMonth;

		/// <summary>
		/// Indicates visibility of progressbar for incremental loading
		/// </summary>
		public bool IsIncrementalLoadingMonth
		{
			get => _isIncrementalLoadingMonth;
			set => Set(() => IsIncrementalLoadingMonth, ref _isIncrementalLoadingMonth, value);
		}
		public bool _isloadingToday;

		/// <summary>
		/// Indicates if progressRing is active
		/// </summary>
		public bool IsLoadingToday
		{
			get => _isloadingToday;
			set => Set(() => IsLoadingToday, ref _isloadingToday, value);
		}
		public bool _isloadingWeek;

		/// <summary>
		/// Indicates if progressRing is active
		/// </summary>
		public bool IsLoadingWeek
		{
			get => _isloadingWeek;
			set => Set(() => IsLoadingWeek, ref _isloadingWeek, value);
		}
		public bool _isloadingMonth;

		/// <summary>
		/// Indicates if progressRing is active
		/// </summary>
		public bool IsLoadingMonth
		{
			get => _isloadingMonth;
			set => Set(() => IsLoadingMonth, ref _isloadingMonth, value);
		}
		#endregion

		public RelayCommand _loadCommand;
		public RelayCommand LoadCommand 
			=> _loadCommand
			?? (_loadCommand = new RelayCommand(async () =>
											 {
												 if (GlobalHelper.IsInternet())
												 {
													 if (TrendingReposToday == null)
													 {
														 await LoadTrendingRepos(TimeRange.TODAY);
													 }
												 }

											 }));

		public async void RefreshTodayCommand(RefreshContainer sender, RefreshRequestedEventArgs args)
		{
			if (!GlobalHelper.IsInternet())
			{
				//Sending NoInternet message to all viewModels
				Messenger.Default.Send(new GlobalHelper.NoInternet().SendMessage());
			}
			else
			{
				IsLoadingToday = true;
				await LoadTrendingRepos(TimeRange.TODAY);
			}
			IsLoadingToday = false;
		}
		public async void RefreshWeekCommand(RefreshContainer sender, RefreshRequestedEventArgs args)
		{
			if (!GlobalHelper.IsInternet())
			{
				//Sending NoInternet message to all viewModels
				Messenger.Default.Send(new GlobalHelper.NoInternet().SendMessage());
			}
			else
			{
				IsLoadingWeek = true;

				await LoadTrendingRepos(TimeRange.WEEKLY);
			}
			IsLoadingWeek = false;
		}
		public async void RefreshMonthCommand(RefreshContainer sender, RefreshRequestedEventArgs args)
		{
			if (!GlobalHelper.IsInternet())
			{
				//Sending NoInternet message to all viewModels
				Messenger.Default.Send(new GlobalHelper.NoInternet().SendMessage());
			}
			else
			{
				IsLoadingMonth = true;
				await LoadTrendingRepos(TimeRange.MONTHLY);
			}
			IsLoadingMonth = false;
		}
		public void RepoDetailNavigateCommand(object sender, ItemClickEventArgs e) 
			=> SimpleIoc
				.Default
				.GetInstance<IAsyncNavigationService>()
				.NavigateAsync(typeof(RepoDetailView), e.ClickedItem as Repository);

		public void FirstRepoTodayNavigate(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
		{
			if (FirstTrendingReposToday != null)
			{
				SimpleIoc.Default.GetInstance<IAsyncNavigationService>().NavigateAsync(typeof(RepoDetailView), FirstTrendingReposToday);
			}
		}
		public void FirstRepoWeekNavigate(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
		{
			if (FirstTrendingReposWeek != null)
			{
				SimpleIoc.Default.GetInstance<IAsyncNavigationService>().NavigateAsync(typeof(RepoDetailView), FirstTrendingReposWeek);
			}
		}
		public void FirstRepoMonthNavigate(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
		{
			if (FirstTrendingReposMonth != null)
			{
				SimpleIoc.Default.GetInstance<IAsyncNavigationService>().NavigateAsync(typeof(RepoDetailView), FirstTrendingReposMonth);
			}
		}

		private async Task LoadTrendingRepos(TimeRange range)
		{
			if (range == TimeRange.TODAY)
			{
				IsLoadingToday = CanLoadMoreToday = true;
				var repos = await RepositoryUtility.GetTrendingRepos(range, true);
				IsLoadingToday = false;

				if (repos != null)
				{
					ZeroTodayCount = false;
					FirstTrendingReposToday = repos[0];
					repos.RemoveAt(0);
					TrendingReposToday = repos;

				}
				else
				{
					ZeroTodayCount = true;
					if (TrendingReposToday != null)
					{
						TrendingReposToday.Clear();
					}
				}
			}
			else if (range == TimeRange.WEEKLY)
			{
				IsLoadingWeek = CanLoadMoreWeek = true;
				var repos = await RepositoryUtility.GetTrendingRepos(range, true);
				IsLoadingWeek = false;

				if (repos != null)
				{
					ZeroWeeklyCount = false;
					FirstTrendingReposWeek = repos[0];
					repos.RemoveAt(0);
					TrendingReposWeek = repos;

				}
				else
				{
					ZeroWeeklyCount = true;
					if (TrendingReposWeek != null)
					{
						TrendingReposWeek.Clear();
					}
				}
			}
			else
			{
				IsLoadingMonth = CanLoadMoreMonth = true;
				var repos = await RepositoryUtility.GetTrendingRepos(range, true);
				IsLoadingMonth = false;

				if (repos != null)
				{
					ZeroMonthlyCount = false;
					FirstTrendingReposMonth = repos[0];
					repos.RemoveAt(0);
					TrendingReposMonth = repos;
				}
				else
				{
					ZeroMonthlyCount = true;
					if (TrendingReposMonth != null)
					{
						TrendingReposMonth.Clear();
					}
				}
			}

		}
		public async Task TodayIncrementalLoad()
		{
			IsIncrementalLoadingToday = true;
			CanLoadMoreToday = false;
			var repos = await RepositoryUtility.GetTrendingRepos(TimeRange.TODAY, false);
			IsIncrementalLoadingToday = false;

			if (repos != null)
			{
				foreach (var i in repos)
				{
					TrendingReposToday.Add(i);
				}
			}
		}

		public async Task WeekIncrementalLoad()
		{
			IsIncrementalLoadingWeek = true;
			CanLoadMoreWeek = false;
			var repos = await RepositoryUtility.GetTrendingRepos(TimeRange.WEEKLY, false);
			IsIncrementalLoadingWeek = false;

			if (repos != null)
			{
				foreach (var i in repos)
				{
					TrendingReposWeek.Add(i);
				}
			}
		}

		public async Task MonthIncrementalLoad()
		{
			IsIncrementalLoadingMonth = true;
			CanLoadMoreMonth = false;
			var repos = await RepositoryUtility.GetTrendingRepos(TimeRange.MONTHLY, false);
			IsIncrementalLoadingMonth = false;

			if (repos != null)
			{
				foreach (var i in repos)
				{
					TrendingReposMonth.Add(i);
				}
			}

		}

		public async void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			var p = sender as Pivot;

			if (p.SelectedIndex == 1 && TrendingReposWeek == null)
			{
				await LoadTrendingRepos(TimeRange.WEEKLY);
			}
			else if (p.SelectedIndex == 2 && TrendingReposMonth == null)
			{
				await LoadTrendingRepos(TimeRange.MONTHLY);
			}

		}
	}
}
