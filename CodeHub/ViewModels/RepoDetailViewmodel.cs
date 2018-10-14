using CodeHub.Helpers;
using CodeHub.Services;
using CodeHub.Views;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using Octokit;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Xaml.Controls;

namespace CodeHub.ViewModels
{
	public class RepoDetailViewmodel : AppViewmodel
	{
		public Repository _repository;
		public Repository Repository
		{
			get => _repository;
			set => Set(() => Repository, ref _repository, value);
		}

		private int _WatchersCount;
		public int WatchersCount
		{
			get => _WatchersCount;
			set => Set(() => WatchersCount, ref _WatchersCount, value);
		}

		public bool _NoReadme;
		public bool NoReadme
		{
			get => _NoReadme;
			set => Set(() => NoReadme, ref _NoReadme, value);
		}

		public bool _isStar;
		public bool IsStar
		{
			get => _isStar;
			set => Set(() => IsStar, ref _isStar, value);
		}

		public bool _IsWatching;
		public bool IsWatching
		{
			get => _IsWatching;
			set => Set(() => IsWatching, ref _IsWatching, value);
		}

		public bool _IsStarLoading;
		public bool IsStarLoading
		{
			get => _IsStarLoading;
			set => Set(() => IsStarLoading, ref _IsStarLoading, value);
		}

		public bool _IsWatchLoading;
		public bool IsWatchLoading
		{
			get => _IsWatchLoading;
			set => Set(() => IsWatchLoading, ref _IsWatchLoading, value);
		}

		public bool _IsForkLoading;
		public bool IsForkLoading
		{
			get => _IsForkLoading;
			set => Set(() => IsForkLoading, ref _IsForkLoading, value);
		}

		public bool _IsContributorsLoading;
		public bool IsContributorsLoading
		{
			get => _IsContributorsLoading;
			set => Set(() => IsContributorsLoading, ref _IsContributorsLoading, value);
		}

		public bool _IsReleasesLoading;
		public bool IsReleasesLoading
		{
			get => _IsReleasesLoading;
			set => Set(() => IsReleasesLoading, ref _IsReleasesLoading, value);
		}

		public ObservableCollection<RepositoryContributor> _Contributors;
		public ObservableCollection<RepositoryContributor> Contributors
		{
			get => _Contributors;
			set => Set(() => Contributors, ref _Contributors, value);
		}

		public ObservableCollection<Release> _Releases;
		public ObservableCollection<Release> Releases
		{
			get => _Releases;
			set => Set(() => Releases, ref _Releases, value);
		}

		public async Task Load(object repo)
		{
			if (GlobalHelper.IsInternet())
			{
				IsLoading = true;

				if (repo is string s)
				{
					//Splitting repository name and owner name
					var names = s.Split('/');
					Repository = await RepositoryUtility.GetRepository(names[0], names[1]);
				}
				else
				{
					if ((repo as Repository).FullName == null)
					{
						Repository = await RepositoryUtility.GetRepository((repo as Repository).Id);
					}
					else
					{
						Repository = repo as Repository;
					}
				}
				if (Repository != null)
				{
					WatchersCount = Repository.SubscribersCount;
					IsStar = await RepositoryUtility.CheckStarred(Repository);
					IsWatching = await RepositoryUtility.CheckWatched(Repository);

					if (Repository.SubscribersCount == 0)
					{
						WatchersCount = (await RepositoryUtility.GetRepository(Repository.Id)).SubscribersCount;
					}
				}
				IsLoading = false;
			}
		}

		private RelayCommand _sourceCodeNavigate;
		public RelayCommand SourceCodeNavigate 
			=> _sourceCodeNavigate
			?? (_sourceCodeNavigate = new RelayCommand(() =>
												 {
													 if (Repository != null)
													 {
														 SimpleIoc
															.Default
															.GetInstance<IAsyncNavigationService>()
															.NavigateAsync(typeof(SourceCodeView), Repository, Repository.FullName);
													 }
												 }));

		private RelayCommand _profileTapped;
		public RelayCommand ProfileTapped 
			=> _profileTapped
			?? (_profileTapped = new RelayCommand(() =>
											 {
												 SimpleIoc
													.Default
													.GetInstance<IAsyncNavigationService>()
													.NavigateAsync(typeof(DeveloperProfileView), Repository.Owner);
											 }));

		private RelayCommand _issuesTapped;
		public RelayCommand IssuesTapped 
			=> _issuesTapped
			?? (_issuesTapped = new RelayCommand(() =>
											 {
												 SimpleIoc
													.Default
													.GetInstance<IAsyncNavigationService>()
													.NavigateAsync(typeof(IssuesView), Repository);
											 }));

		private RelayCommand _PullRequestsTapped;
		public RelayCommand PullRequestsTapped 
			=> _PullRequestsTapped
			?? (_PullRequestsTapped = new RelayCommand(() =>
											{
												SimpleIoc
													.Default
													.GetInstance<IAsyncNavigationService>()
													.NavigateAsync(typeof(PullRequestsView), Repository);
											}));

		private RelayCommand _StarCommand;
		public RelayCommand StarCommand 
			=> _StarCommand
			?? (_StarCommand = new RelayCommand(async () =>
											 {
												 if (!IsStar)
												 {
													 IsStarLoading = true;
													 if (await RepositoryUtility.StarRepository(Repository))
													 {
														 IsStarLoading = false;
														 IsStar = true;
														 GlobalHelper.NewStarActivity = true;
													 }
												 }
												 else
												 {
													 IsStarLoading = true;
													 if (await RepositoryUtility.UnstarRepository(Repository))
													 {
														 IsStarLoading = false;
														 IsStar = false;
														 GlobalHelper.NewStarActivity = true;
													 }
												 }
												 await RefreshRepository();
											 }));

		private RelayCommand _WatchCommand;
		public RelayCommand WatchCommand 
			=> _WatchCommand
			?? (_WatchCommand = new RelayCommand(async () =>
											 {
												 if (!IsWatching)
												 {
													 IsWatchLoading = true;
													 if (await RepositoryUtility.WatchRepository(Repository))
													 {
														 IsWatchLoading = false;
														 IsWatching = true;
													 }
												 }
												 else
												 {
													 IsWatchLoading = true;
													 if (await RepositoryUtility.UnwatchRepository(Repository))
													 {
														 IsWatchLoading = false;
														 IsWatching = false;
													 }
												 }
												 WatchersCount = Repository.SubscribersCount;
												 if (Repository.SubscribersCount == 0)
												 {
													 WatchersCount = (await RepositoryUtility.GetRepository(Repository.Id)).SubscribersCount;
												 }
											 }));

		private RelayCommand _ForkCommand;
		public RelayCommand ForkCommand
		{
			get
			{
				return _ForkCommand
				    ?? (_ForkCommand = new RelayCommand(
									 async () =>
									 {
										 IsForkLoading = true;
										 var forkedRepo = await RepositoryUtility.ForkRepository(Repository);
										 IsForkLoading = false;
										 if (forkedRepo != null)
										 {
											 Messenger.Default.Send(new GlobalHelper.LocalNotificationMessageType
											 {
												 Message = $"{forkedRepo.FullName} was successfully forked from {Repository.FullName}",
												 Glyph = "\uE081"
											 });

											 SimpleIoc.Default.GetInstance<IAsyncNavigationService>().NavigateAsync(typeof(RepoDetailView), forkedRepo).Forget();
										 }
										 else
										 {
											 Messenger.Default.Send(new GlobalHelper.LocalNotificationMessageType
											 {
												 Message = "Repository could not be forked",
												 Glyph = "\uE783"
											 });
										 }

									 }));
			}
		}

		private RelayCommand _CloneCommand;
		public RelayCommand CloneCommand
		{
			get
			{
				return _CloneCommand
				    ?? (_CloneCommand = new RelayCommand(
									 () =>
									 {
										 var dataPackage = new DataPackage
										 {
											 RequestedOperation = DataPackageOperation.Copy
										 };
										 dataPackage.SetText(Repository.CloneUrl);
										 Clipboard.SetContent(dataPackage);
										 Messenger.Default.Send(
											 new GlobalHelper.LocalNotificationMessageType
											 { Message = Repository.CloneUrl + " copied to clipboard", Glyph = "\uE16F" }
										);
									 }));
			}
		}

		public void UserTapped(object sender, ItemClickEventArgs e) 
			=> SimpleIoc
				.Default
				.GetInstance<IAsyncNavigationService>()
				.NavigateAsync(typeof(DeveloperProfileView), ((RepositoryContributor)e.ClickedItem).Login);

		public async void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			var p = sender as Pivot;

			if (p.SelectedIndex == 1)
			{
				if (GlobalHelper.IsInternet())
				{
					IsContributorsLoading = true;
					if (Repository != null)
					{
						Contributors = await RepositoryUtility.GetContributorsForRepository(Repository.Id);
					}

					IsContributorsLoading = false;
				}
			}
			else if (p.SelectedIndex == 2)
			{
				if (GlobalHelper.IsInternet())
				{
					IsReleasesLoading = true;
					if (Repository != null)
					{
						Releases = await RepositoryUtility.GetReleasesForRepository(Repository.Id);
					}

					IsReleasesLoading = false;
				}
			}
		}

		public async Task RefreshRepository() 
			=> Repository = await RepositoryUtility.GetRepository(Repository.Id);
	}
}
