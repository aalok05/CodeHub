using CodeHub.Helpers;
using CodeHub.Services;
using CodeHub.Views;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using Octokit;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace CodeHub.ViewModels
{
	public class PullRequestDetailViewmodel : AppViewmodel
	{
		public Repository _repository;
		public Repository Repository
		{
			get => _repository;
			set => Set(() => Repository, ref _repository, value);
		}

		public PullRequest _PullRequest;
		public PullRequest PullRequest
		{
			get => _PullRequest;
			set => Set(() => PullRequest, ref _PullRequest, value);
		}

		public ObservableCollection<IssueComment> _comments;
		public ObservableCollection<IssueComment> Comments
		{
			get => _comments;
			set => Set(() => Comments, ref _comments, value);
		}
		public ObservableCollection<PullRequestCommit> _Commits;
		public ObservableCollection<PullRequestCommit> Commits
		{
			get => _Commits;
			set => Set(() => Commits, ref _Commits, value);
		}

		public string _CommentText;
		public string CommentText
		{
			get => _CommentText;
			set => Set(() => CommentText, ref _CommentText, value);
		}

		public bool _IsLoadingCommits;

		public bool IsLoadingCommits
		{
			get => _IsLoadingCommits;
			set => Set(() => IsLoadingCommits, ref _IsLoadingCommits, value);
		}

		public async Task Load(Tuple<Repository, PullRequest> tuple)
		{
			PullRequest = tuple.Item2;
			Repository = tuple.Item1;

			if (GlobalHelper.IsInternet())
			{
				IsLoading = true;
				PullRequest = await PullRequestUtility.GetPullRequest(Repository.Id, PullRequest.Number);
				Comments = await IssueUtility.GetAllCommentsForIssue(Repository.Id, PullRequest.Number);
				IsLoading = false;
			}
		}

		public void CommentTapped(object sender, ItemClickEventArgs e) => 
			SimpleIoc
				.Default
				.GetInstance<IAsyncNavigationService>()
				.NavigateAsync(typeof(CommentView), e.ClickedItem as IssueComment);

		public void CommitTapped(object sender, ItemClickEventArgs e) 
			=> SimpleIoc
				.Default
				.GetInstance<IAsyncNavigationService>()
				.NavigateAsync(typeof(CommitDetailView), (Repository.Id, (e.ClickedItem as PullRequestCommit).Sha));

		private RelayCommand _userTapped;
		public RelayCommand UserTapped 
			=> _userTapped
			?? (_userTapped = new RelayCommand(() =>
										 {
											 SimpleIoc
												.Default
												.GetInstance<IAsyncNavigationService>()
												.NavigateAsync(typeof(DeveloperProfileView), PullRequest.User);
										 }));

		private RelayCommand _CommentCommand;
		public RelayCommand CommentCommand 
			=> _CommentCommand
			?? (_CommentCommand = new RelayCommand(async () =>
											 {
												 if (!string.IsNullOrWhiteSpace(CommentText))
												 {
													 IsLoading = true;
													 var newComment = await IssueUtility
																	.CommentOnIssue(Repository.Id, PullRequest.Number, CommentText);
													 IsLoading = false;
													 if (newComment != null)
													 {
														 Comments.Add(newComment);
														 CommentText = string.Empty;
													 }

												 }
											 }));

		public async void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			var p = sender as Pivot;

			if (p.SelectedIndex == 0)
			{
				IsLoading = true;
				Comments = await IssueUtility.GetAllCommentsForIssue(Repository.Id, PullRequest.Number);
				IsLoading = false;
			}
			else if (p.SelectedIndex == 1)
			{
				IsLoadingCommits = true;
				Commits = await PullRequestUtility.GetAllCommitsForPullRequest(Repository.Id, PullRequest.Number);
				IsLoadingCommits = false;
			}
		}

	}
}
