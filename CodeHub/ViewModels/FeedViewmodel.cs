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
using Windows.UI.Xaml.Input;

namespace CodeHub.ViewModels
{
    public class FeedViewmodel : AppViewmodel
    {
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

        private int _PaginationIndex;
        public int PaginationIndex
        {
            get
            {
                return _PaginationIndex;
            }
            set
            {
                Set(() => PaginationIndex, ref _PaginationIndex, value);
            }
        }

        public bool _zeroEventCount;
        public bool ZeroEventCount
        {
            get
            {
                return _zeroEventCount;
            }
            set
            {
                Set(() => ZeroEventCount, ref _zeroEventCount, value);
            }
        }

        public bool _isIncrementalLoading;
        public bool IsIncrementalLoading
        {
            get
            {
                return _isIncrementalLoading;
            }
            set
            {
                Set(() => IsIncrementalLoading, ref _isIncrementalLoading, value);

            }
        }

        public RelayCommand _loadCommand;
        public RelayCommand LoadCommand
        {
            get
            {
                return _loadCommand
                    ?? (_loadCommand = new RelayCommand(
                                          async () =>
                                          {
                                             
                                              if (!GlobalHelper.IsInternet())
                                              {
                                                  //Sending NoInternet message to all viewModels
                                                  Messenger.Default.Send(new GlobalHelper.LocalNotificationMessageType { Message = "No Internet", Glyph = "\uE704" });
                                              }
                                              else
                                              {
                                                  if(Events == null)
                                                  {   
                                                      isLoading = true;
                                                      await LoadEvents();
                                                      isLoading = false;
                                                  }
                                              }
                                          }));
            }
        }
        public async void RefreshCommand(object sender, EventArgs e)
        {
            
            if (!GlobalHelper.IsInternet())
            {
                Messenger.Default.Send(new GlobalHelper.LocalNotificationMessageType { Message="No Internet", Glyph= "\uE704" });
            }
            else
            {
                Messenger.Default.Send(new GlobalHelper.HasInternetMessageType()); //Sending Internet available message to all viewModels
                isLoading = true;

                PaginationIndex = 0;
                await LoadEvents();

                isLoading = false;
            }
        }

        public void RecieveSignOutMessage(GlobalHelper.SignOutMessageType empty)
        {
            isLoggedin = false;
            User = null;
            Events = null;
        }
        public async void RecieveSignInMessage(User user)
        {
         
            isLoading = true;
            if (user != null)
            {
                isLoggedin = true;
                User = user;
                PaginationIndex = 0;
                await LoadEvents();
            }
            isLoading = false;

        }

        public async Task LoadEvents()
        {
            PaginationIndex++;
            if(PaginationIndex > 1)
            {
                IsIncrementalLoading = true;

                var events = await UserUtility.GetUserActivity(PaginationIndex);
                if (events != null)
                {
                    if(events.Count > 0)
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
            else if(PaginationIndex == 1)
            {
                Events = await UserUtility.GetUserActivity(PaginationIndex);
                if (Events != null)
                {
                    if(Events.Count == 0)
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
            Activity activity = e.ClickedItem as Activity;

            switch (activity.Type)
            {
                case "IssueCommentEvent":
                    SimpleIoc.Default.GetInstance<IAsyncNavigationService>().NavigateAsync(typeof(IssueDetailView), "Issue", new Tuple<Repository, Issue>(activity.Repo, ((IssueCommentPayload)activity.Payload).Issue));
                    break;

                case "IssuesEvent":
                    SimpleIoc.Default.GetInstance<IAsyncNavigationService>().NavigateAsync(typeof(IssueDetailView), "Issue", new Tuple<Repository, Issue>(activity.Repo, ((IssueEventPayload)activity.Payload).Issue));
                    break;

                case "PullRequestReviewCommentEvent":
                    SimpleIoc.Default.GetInstance<IAsyncNavigationService>().NavigateAsync(typeof(PullRequestDetailView), "Pull Request", new Tuple<Repository, PullRequest>(activity.Repo, ((PullRequestCommentPayload)activity.Payload).PullRequest));
                    break;

                case "PullRequestEvent":
                case "PullRequestReviewEvent":
                    SimpleIoc.Default.GetInstance<IAsyncNavigationService>().NavigateAsync(typeof(PullRequestDetailView), "Pull Request", new Tuple<Repository, PullRequest>(activity.Repo, ((PullRequestEventPayload)activity.Payload).PullRequest));
                    break;

                default:
                    SimpleIoc.Default.GetInstance<IAsyncNavigationService>().NavigateAsync(typeof(RepoDetailView), "Repository", activity.Repo.Name);
                    break;
            }
            
        }
    }
}
