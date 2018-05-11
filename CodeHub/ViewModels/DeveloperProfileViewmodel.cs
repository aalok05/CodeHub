using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using CodeHub.Helpers;
using CodeHub.Services;
using CodeHub.Views;
using Octokit;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Input;

namespace CodeHub.ViewModels
{
    public class DeveloperProfileViewmodel : AppViewmodel
    {
        #region properties

        public int ReposPaginationIndex { get; set; }
        public int StarredReposPaginationIndex { get; set; }
        public double ReposMaxScrollViewerOffset { get; set; }
        public double StarredReposMaxScrollViewerOffset { get; set; }

        public ObservableCollection<Activity> _events;
        public ObservableCollection<Activity> Events
        {
            get
            {
                return _events;
            }
            set
            {
                Set(() => Events, ref _events, value);
            }
        }

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
        public ObservableCollection<Repository> _starredRepositories;
        public ObservableCollection<Repository> StarredRepositories
        {
            get
            {
                return _starredRepositories;
            }
            set
            {
                Set(() => StarredRepositories, ref _starredRepositories, value);
            }

        }

        public ObservableCollection<User> _followers;
        public ObservableCollection<User> Followers
        {
            get
            {
                return _followers;
            }
            set
            {
                Set(() => Followers, ref _followers, value);
            }
        }

        public ObservableCollection<User> _following;
        public ObservableCollection<User> Following
        {
            get
            {
                return _following;
            }
            set
            {
                Set(() => Following, ref _following, value);
            }
        }

        public bool _IsFollowersLoading;
        public bool IsFollowersLoading
        {
            get
            {
                return _IsFollowersLoading;
            }
            set
            {
                Set(() => IsFollowersLoading, ref _IsFollowersLoading, value);
            }
        }

        public bool _IsFollowingLoading;
        public bool IsFollowingLoading
        {
            get
            {
                return _IsFollowingLoading;
            }
            set
            {
                Set(() => IsFollowingLoading, ref _IsFollowingLoading, value);
            }
        }

        public User _developer;
        public User Developer
        {
            get
            {
                return _developer;
            }
            set
            {
                Set(() => Developer, ref _developer, value);
            }
        }

        public bool _isFollowing;
        public bool IsFollowing
        {
            get
            {
                return _isFollowing;
            }
            set
            {
                Set(() => IsFollowing, ref _isFollowing, value);
            }
        }

        public bool _IsEventsLoading;
        public bool IsEventsLoading
        {
            get
            {
                return _IsEventsLoading;
            }
            set
            {
                Set(() => IsEventsLoading, ref _IsEventsLoading, value);
            }
        }

        public bool _IsReposLoading;
        public bool IsReposLoading
        {
            get
            {
                return _IsReposLoading;
            }
            set
            {
                Set(() => IsReposLoading, ref _IsReposLoading, value);
            }
        }

        public bool _IsStarredReposLoading;
        public bool IsStarredReposLoading
        {
            get
            {
                return _IsStarredReposLoading;
            }
            set
            {
                Set(() => IsStarredReposLoading, ref _IsStarredReposLoading, value);
            }
        }

        public bool _canFollow;
        public bool CanFollow
        {
            get
            {
                return _canFollow;
            }
            set
            {
                Set(() => CanFollow, ref _canFollow, value);
            }
        }

        public bool _followProgress;
        public bool FollowProgress
        {
            get
            {
                return _followProgress;
            }
            set
            {
                Set(() => FollowProgress, ref _followProgress, value);
            }
        }

        public bool _isUserEditable;

        /// <summary>
        /// Indicates whether a profile can be edited by current user
        /// </summary>
        public bool IsUserEditable
        {
            get
            {
                return _isUserEditable;
            }
            set
            {
                Set(() => IsUserEditable, ref _isUserEditable, value);
            }
        }
        #endregion

        public async Task Load(object user)
        {
            if (GlobalHelper.IsInternet())
            {
                isLoading = true;

                // Get the user from login name
                if (user is string login)
                {
                    if (!string.IsNullOrWhiteSpace(login))
                    {
                        Developer = await UserService.GetUserInfo(login);
                    }
                }
                else
                {
                    Developer = user as User;
                    if(Developer != null && Developer.Name == null)
                    {
                        // Get full details of the user
                        Developer = await UserService.GetUserInfo(Developer.Login);
                    }
                }


                if (Developer != null)
                {
                    if (Developer.Type == AccountType.Organization || Developer.Login == GlobalHelper.UserLogin)
                    {
                        // Organisations can't be followed
                        CanFollow = false;

                        // User can edit it's own profile
                        IsUserEditable = Developer.Login == GlobalHelper.UserLogin;
                    }
                    else
                    {
                        CanFollow = true;
                        FollowProgress = true;
                        if (await UserService.CheckFollow(Developer.Login))
                        {
                            IsFollowing = true;
                        }
                        FollowProgress = false;
                    }

                    IsEventsLoading = true;
                    Events = await ActivityService.GetUserPerformedActivity(Developer.Login);
                    IsEventsLoading = false;
                }
                isLoading = false;
            }
        }

        private RelayCommand _followCommand;
        public RelayCommand FollowCommand
        {
            get
            {
                return _followCommand
                    ?? (_followCommand = new RelayCommand(
                                          async () =>
                                          {
                                              FollowProgress = true;
                                              if (await UserService.FollowUser(Developer.Login))
                                              {
                                                  IsFollowing = true;
                                              }
                                              FollowProgress = false;
                                          }));
            }
        }

        private RelayCommand _unFollowCommand;
        public RelayCommand UnfollowCommand
        {
            get
            {
                return _unFollowCommand
                    ?? (_unFollowCommand = new RelayCommand(
                                          async () =>
                                          {
                                              FollowProgress = true;
                                              await UserService.UnFollowUser(Developer.Login);
                                              IsFollowing = false;
                                              FollowProgress = false;
                                          }));
            }
        }

        public async void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Pivot p = sender as Pivot;

            if (p.SelectedIndex == 0)
            {
                IsEventsLoading = true;
                if(Developer != null)
                    Events = await ActivityService.GetUserPerformedActivity(Developer.Login);
                IsEventsLoading = false;

            }
            else if (p.SelectedIndex == 1)
            {
                IsReposLoading = true;
                await LoadRepos();
                IsReposLoading = false;
            }
            else if (p.SelectedIndex == 2)
            {
                IsStarredReposLoading = true;
                await LoadStarredRepos();
                IsStarredReposLoading = false;
            }
            else if (p.SelectedIndex == 3)
            {
                IsFollowersLoading = true;
                Followers = await UserService.GetAllFollowers(Developer.Login);
                IsFollowersLoading = false;
            }
            else if (p.SelectedIndex == 4)
            {
                IsFollowingLoading = true;
                Following = await UserService.GetAllFollowing(Developer.Login);
                IsFollowingLoading = false;
            }
        }

        public async Task LoadRepos()
        {
            if(Repositories == null)
            {
                Repositories = new ObservableCollection<Repository>();
            }
            ReposPaginationIndex++;
            if (ReposPaginationIndex > 1)
            {
                var repos = await RepositoryUtility.GetRepositoriesForUser(Developer.Login, ReposPaginationIndex);
                if (repos != null)
                {
                    if (repos.Count > 0)
                    {
                        foreach (var i in repos)
                        {
                            Repositories.Add(i);
                        }
                    }
                    else
                    {
                        //no more repos to load
                        ReposPaginationIndex = -1;
                    }
                }
            }
            else if (ReposPaginationIndex == 1)
            {
                Repositories = await RepositoryUtility.GetRepositoriesForUser(Developer.Login, ReposPaginationIndex);
            }
        }

        public async Task LoadStarredRepos()
        {
            if (StarredRepositories == null)
            {
                StarredRepositories = new ObservableCollection<Repository>();
            }
            StarredReposPaginationIndex++;
            if (StarredReposPaginationIndex > 1)
            {
                var repos = await RepositoryUtility.GetStarredRepositoriesForUser(Developer.Login, StarredReposPaginationIndex);
                if (repos != null)
                {
                    if (repos.Count > 0)
                    {
                        foreach (var i in repos)
                        {
                            StarredRepositories.Add(i);
                        }
                    }
                    else
                    {
                        //no more repos to load
                        StarredReposPaginationIndex = -1;
                    }
                }
            }
            else if (StarredReposPaginationIndex == 1)
            {
                StarredRepositories = await RepositoryUtility.GetStarredRepositoriesForUser(Developer.Login, StarredReposPaginationIndex);
            }
        }

        public void RepoDetailNavigateCommand(object sender, ItemClickEventArgs e)
        {
            SimpleIoc.Default.GetInstance<Services.IAsyncNavigationService>().NavigateAsync(typeof(RepoDetailView), e.ClickedItem as Repository);
        }

        public void UserTapped(object sender, ItemClickEventArgs e)
        {
            SimpleIoc.Default.GetInstance<IAsyncNavigationService>().NavigateAsync(typeof(DeveloperProfileView), e.ClickedItem as User);
        }

        public void ProfileEdit_Tapped(object sender, TappedRoutedEventArgs e)
        {
            SimpleIoc.Default.GetInstance<IAsyncNavigationService>().NavigateAsync(typeof(EditProfileView), this.Developer);
        }

        public void FeedListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            Activity activity = e.ClickedItem as Activity;
            try
            {
                switch (activity.Type)
                {
                    case "IssueCommentEvent":
                        SimpleIoc.Default.GetInstance<IAsyncNavigationService>().NavigateAsync(typeof(IssueDetailView), new Tuple<Repository, Issue>(activity.Repo, ((IssueCommentPayload)activity.Payload).Issue));
                        break;

                    case "IssuesEvent":
                        SimpleIoc.Default.GetInstance<IAsyncNavigationService>().NavigateAsync(typeof(IssueDetailView), new Tuple<Repository, Issue>(activity.Repo, ((IssueEventPayload)activity.Payload).Issue));
                        break;

                    case "PullRequestReviewCommentEvent":
                        SimpleIoc.Default.GetInstance<IAsyncNavigationService>().NavigateAsync(typeof(PullRequestDetailView), new Tuple<Repository, PullRequest>(activity.Repo, ((PullRequestCommentPayload)activity.Payload).PullRequest));
                        break;

                    case "PullRequestEvent":
                    case "PullRequestReviewEvent":
                        SimpleIoc.Default.GetInstance<IAsyncNavigationService>().NavigateAsync(typeof(PullRequestDetailView), new Tuple<Repository, PullRequest>(activity.Repo, ((PullRequestEventPayload)activity.Payload).PullRequest));
                        break;

                    case "ForkEvent":
                        SimpleIoc.Default.GetInstance<IAsyncNavigationService>().NavigateAsync(typeof(RepoDetailView), ((ForkEventPayload)activity.Payload).Forkee);
                        break;
                    case "CommitCommentEvent":
                        SimpleIoc.Default.GetInstance<IAsyncNavigationService>().NavigateAsync(typeof(CommitDetailView), new Tuple<long, string>(activity.Repo.Id, ((CommitCommentPayload)activity.Payload).Comment.CommitId));
                        break;

                    case "PushEvent":
                        SimpleIoc.Default.GetInstance<IAsyncNavigationService>().NavigateAsync(typeof(CommitsView), new Tuple<long, IReadOnlyList<Commit>>(activity.Repo.Id, ((PushEventPayload)activity.Payload).Commits));
                        break;

                    default:
                        SimpleIoc.Default.GetInstance<IAsyncNavigationService>().NavigateAsync(typeof(RepoDetailView), activity.Repo);
                        break;
                }
            }
            catch
            {
                SimpleIoc.Default.GetInstance<IAsyncNavigationService>().NavigateAsync(typeof(RepoDetailView), activity.Repo);
            }
        }
    }
}
