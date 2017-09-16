using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using CodeHub.Helpers;
using CodeHub.Services;
using CodeHub.Views;
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
            get
            {
                return _repository;
            }
            set
            {
                Set(() => Repository, ref _repository, value);

            }
        }

        public PullRequest _PullRequest;
        public PullRequest PullRequest
        {
            get
            {
                return _PullRequest;
            }
            set
            {
                Set(() => PullRequest, ref _PullRequest, value);

            }
        }

        public ObservableCollection<IssueComment> _comments;
        public ObservableCollection<IssueComment> Comments
        {
            get
            {
                return _comments;
            }
            set
            {
                Set(() => Comments, ref _comments, value);

            }
        }
        public ObservableCollection<PullRequestCommit> _Commits;
        public ObservableCollection<PullRequestCommit> Commits
        {
            get
            {
                return _Commits;
            }
            set
            {
                Set(() => Commits, ref _Commits, value);

            }
        }

        public string _CommentText;
        public string CommentText
        {
            get
            {
                return _CommentText;
            }
            set
            {
                Set(() => CommentText, ref _CommentText, value);
            }
        }

        public bool _IsLoadingCommits;

        public bool IsLoadingCommits
        {
            get
            {
                return _IsLoadingCommits;
            }
            set
            {
                Set(() => IsLoadingCommits, ref _IsLoadingCommits, value);
            }
        }

        public async Task Load(Tuple<Repository, PullRequest> tuple)
        {
            PullRequest = tuple.Item2;
            Repository = tuple.Item1;

            if (GlobalHelper.IsInternet())
            {
                isLoading = true;
                PullRequest = await PullRequestUtility.GetPullRequest(Repository.Id,PullRequest.Number);
                Comments = await IssueUtility.GetAllCommentsForIssue(Repository.Id, PullRequest.Number);
                isLoading = false;
            }
        }

        public void CommentTapped(object sender, ItemClickEventArgs e)
        {
            SimpleIoc.Default.GetInstance<Services.IAsyncNavigationService>().NavigateAsync(typeof(CommentView), e.ClickedItem as IssueComment);
        }

        private RelayCommand _userTapped;
        public RelayCommand UserTapped
        {
            get
            {
                return _userTapped
                    ?? (_userTapped = new RelayCommand(
                                          () =>
                                          {
                                              SimpleIoc.Default.GetInstance<IAsyncNavigationService>().NavigateAsync(typeof(DeveloperProfileView), PullRequest.User);
                                          }));
            }
        }
        private RelayCommand _CommentCommand;
        public RelayCommand CommentCommand
        {
            get
            {
                return _CommentCommand
                    ?? (_CommentCommand = new RelayCommand(
                                          async () =>
                                          {
                                              if (!string.IsNullOrWhiteSpace(CommentText))
                                              {
                                                  isLoading = true;
                                                  IssueComment newComment = await IssueUtility.CommentOnIssue(Repository.Id, PullRequest.Number, CommentText);
                                                  isLoading = false;
                                                  if (newComment != null)
                                                  {
                                                      Comments.Add(newComment);
                                                      CommentText = string.Empty;
                                                  }

                                              }
                                          }));
            }
        }

        public async void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Pivot p = sender as Pivot;

            if (p.SelectedIndex == 0)
            {
                isLoading = true;
                Comments = await IssueUtility.GetAllCommentsForIssue(Repository.Id, PullRequest.Number);
                isLoading = false;
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
