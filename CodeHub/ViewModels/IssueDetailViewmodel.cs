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
	public class IssueDetailViewmodel : AppViewmodel
	{
		public Repository _repository;
		public Repository Repository
		{
			get => _repository;
			set => Set(() => Repository, ref _repository, value);
		}

		public Issue _issue;
		public Issue Issue
		{
			get => _issue;
			set => Set(() => Issue, ref _issue, value);
		}

		public bool _CanEditIssue;
		public bool CanEditIssue
		{
			get => _CanEditIssue;
			set => Set(() => CanEditIssue, ref _CanEditIssue, value);
		}

		public bool _IsMyRepo;
		public bool IsMyRepo
		{
			get => _IsMyRepo;
			set => Set(() => IsMyRepo, ref _IsMyRepo, value);
		}

		public ObservableCollection<IssueComment> _comments;
		public ObservableCollection<IssueComment> Comments
		{
			get => _comments;
			set => Set(() => Comments, ref _comments, value);
		}

		public string _CommentText;
		public string CommentText
		{
			get => _CommentText;
			set => Set(() => CommentText, ref _CommentText, value);
		}

		public string _NewIssueTitleText;
		public string NewIssueTitleText
		{
			get => _NewIssueTitleText;
			set => Set(() => NewIssueTitleText, ref _NewIssueTitleText, value);
		}

		public string _NewIssueBodyText;
		public string NewIssueBodyText
		{
			get => _NewIssueBodyText;
			set => Set(() => NewIssueBodyText, ref _NewIssueBodyText, value);
		}

		public bool _isEditingIssue;
		public bool IsEditingIssue
		{
			get => _isEditingIssue;
			set => Set(() => IsEditingIssue, ref _isEditingIssue, value);
		}

		public ObservableCollection<Label> _AllLabels;
		/// <summary>
		/// All available labels in the repository
		/// </summary>
		public ObservableCollection<Label> AllLabels
		{
			get => _AllLabels;
			set => Set(() => AllLabels, ref _AllLabels, value);
		}

		public async Task Load(object param)
		{
			if (param as Tuple<Repository, Issue> != null)
			{
				var tuple = param as Tuple<Repository, Issue>;
				Issue = tuple.Item2;
				Repository = tuple.Item1;
			}
			else
			{
				if (param is Tuple<string, string, Issue> tuple)
				{
					Issue = tuple.Item3;
					Repository = await RepositoryUtility.GetRepository(tuple.Item1, tuple.Item2);
				}
			}

			if (GlobalHelper.IsInternet())
			{
				if (Repository != null)
				{
					IsLoading = true;
					Comments = await IssueUtility.GetAllCommentsForIssue(Repository.Id, Issue.Number);
					IsLoading = false;

					if (Repository.Owner == null)
					{
						Repository = await RepositoryUtility.GetRepository(Repository.Id);
					}

					if (Repository.Owner.Login == GlobalHelper.UserLogin || Issue.User.Login == GlobalHelper.UserLogin)
					{
						CanEditIssue = true;
					}

					if (Repository.Owner.Login == GlobalHelper.UserLogin)
					{
						IsMyRepo = true;
					}
				}
			}
		}

		public void CommentTapped(object sender, ItemClickEventArgs e) 
			=> SimpleIoc
				.Default
				.GetInstance<IAsyncNavigationService>()
				.NavigateAsync(typeof(CommentView), e.ClickedItem as IssueComment);

		private RelayCommand _userTapped;
		public RelayCommand UserTapped 
			=> _userTapped
			?? (_userTapped = new RelayCommand(() =>
										 {
											 SimpleIoc
												.Default
												.GetInstance<IAsyncNavigationService>()
												.NavigateAsync(typeof(DeveloperProfileView), Issue.User);
										 }));

		private RelayCommand _CommentCommand;
		public RelayCommand CommentCommand
		{
			get
			{
				async void execute()
				{
					IsLoading = true;
					var newComment = await IssueUtility.CommentOnIssue(Repository.Id, Issue.Number, CommentText);
					IsLoading = false;
					if (newComment != null)
					{
						Comments.Add(newComment);
						CommentText = string.Empty;
					}
				}
				return _CommentCommand ?? (_CommentCommand = new RelayCommand(execute));
			}
		}

		public async Task EditIssue()
		{
			var updatedIssue = new IssueUpdate
			{
				Title = NewIssueTitleText,
				Body = NewIssueBodyText
			};
			IsEditingIssue = true;
			var issue = await IssueUtility.EditIssue(Repository.Id, Issue.Number, updatedIssue);
			IsEditingIssue = false;
			if (issue != null)
			{
				Issue = issue;
			}
		}
	}
}
