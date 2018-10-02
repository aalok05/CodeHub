using CodeHub.Helpers;
using CodeHub.Services;
using CodeHub.Views;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using Octokit;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace CodeHub.ViewModels
{
	public class FeedViewmodel : AppViewmodel
	{
		public ObservableCollection<Activity> _events;
		public ObservableCollection<Activity> Events
		{
			get => _events;
			set => Set(() => Events, ref _events, value);
		}

		public int PaginationIndex { get; set; }
		public double MaxScrollViewerOffset { get; set; }

		public bool _zeroEventCount;
		public bool ZeroEventCount
		{
			get => _zeroEventCount;
			set => Set(() => ZeroEventCount, ref _zeroEventCount, value);
		}

		public bool _isIncrementalLoading;
		public bool IsIncrementalLoading
		{
			get => _isIncrementalLoading;
			set => Set(() => IsIncrementalLoading, ref _isIncrementalLoading, value);
		}

		public RelayCommand _loadCommand;
		public RelayCommand LoadCommand 
			=> _loadCommand
			?? (_loadCommand = new RelayCommand(async () =>
											 {

												 if (GlobalHelper.IsInternet())
												 {
													 if (Events == null)
													 {
														 IsLoading = true;
														 await LoadEvents();
														 IsLoading = false;
													 }
												 }
											 }));
		public async void RefreshCommand(object sender, EventArgs e)
		{
			if (!GlobalHelper.IsInternet())
			{
				//Sending NoInternet message to all viewModels
				Messenger.Default.Send(new GlobalHelper.NoInternet().SendMessage());
			}
			else
			{
				Messenger.Default.Send(new GlobalHelper.HasInternetMessageType()); //Sending Internet available message to all viewModels
				IsLoading = true;

				PaginationIndex = 0;
				await LoadEvents();
				MaxScrollViewerOffset = 0;

				IsLoading = false;
			}
		}

		public void RecieveSignOutMessage(GlobalHelper.SignOutMessageType empty)
		{
			IsLoggedin = false;
			User = null;
			Events = null;
		}
		public async void RecieveSignInMessage(User user)
		{

			IsLoading = true;
			if (user != null)
			{
				IsLoggedin = true;
				User = user;
				PaginationIndex = 0;
				await LoadEvents();
			}
			IsLoading = false;

		}

		public async Task LoadEvents()
		{
			PaginationIndex++;
			if (PaginationIndex > 1)
			{
				IsIncrementalLoading = true;

				var events = await UserService.GetUserActivity(PaginationIndex);
				if (events != null)
				{
					if (events.Count > 0)
					{
						foreach (var i in events)
						{
							Events.Add(i);
						}
					}
					else
					{
						//no more feed items left to load
						PaginationIndex = -1;
					}

				}
				IsIncrementalLoading = false;
			}
			else if (PaginationIndex == 1)
			{
				Events = await UserService.GetUserActivity(PaginationIndex);
				if (Events != null)
				{
					if (Events.Count == 0)
					{
						PaginationIndex = 0;
						ZeroEventCount = true;
					}
					else
					{
						ZeroEventCount = false;
					}
				}
			}
		}

		public void FeedListView_ItemClick(object sender, ItemClickEventArgs e)
		{
			var activity = e.ClickedItem as Activity;

			switch (activity.Type)
			{
				case "IssueCommentEvent":
					SimpleIoc
						.Default
						.GetInstance<IAsyncNavigationService>()
						.NavigateAsync(typeof(IssueDetailView), new Tuple<Repository, Issue>(activity.Repo, ((IssueCommentPayload)activity.Payload).Issue));
					break;

				case "IssuesEvent":
					SimpleIoc
						.Default
						.GetInstance<IAsyncNavigationService>()
						.NavigateAsync(typeof(IssueDetailView), new Tuple<Repository, Issue>(activity.Repo, ((IssueEventPayload)activity.Payload).Issue));
					break;

				case "PullRequestReviewCommentEvent":
					SimpleIoc
						.Default
						.GetInstance<IAsyncNavigationService>()
						.NavigateAsync(typeof(PullRequestDetailView), new Tuple<Repository, PullRequest>(activity.Repo, ((PullRequestCommentPayload)activity.Payload).PullRequest));
					break;

				case "PullRequestEvent":
				case "PullRequestReviewEvent":
					SimpleIoc
						.Default
						.GetInstance<IAsyncNavigationService>()
						.NavigateAsync(typeof(PullRequestDetailView), new Tuple<Repository, PullRequest>(activity.Repo, ((PullRequestEventPayload)activity.Payload).PullRequest));
					break;

				case "ForkEvent":
					SimpleIoc
						.Default
						.GetInstance<IAsyncNavigationService>()
						.NavigateAsync(typeof(RepoDetailView), ((ForkEventPayload)activity.Payload).Forkee);
					break;
				case "CommitCommentEvent":
					SimpleIoc
						.Default
						.GetInstance<IAsyncNavigationService>()
						.NavigateAsync(typeof(CommitDetailView), new Tuple<long, string>(activity.Repo.Id, ((CommitCommentPayload)activity.Payload).Comment.CommitId));
					break;

				case "PushEvent":
					SimpleIoc
						.Default
						.GetInstance<IAsyncNavigationService>()
						.NavigateAsync(typeof(CommitsView), new Tuple<long, IReadOnlyList<Commit>>(activity.Repo.Id, ((PushEventPayload)activity.Payload).Commits));
					break;


				default:
					SimpleIoc
						.Default
						.GetInstance<IAsyncNavigationService>()
						.NavigateAsync(typeof(RepoDetailView), activity.Repo);
					break;
			}

		}
	}
}
