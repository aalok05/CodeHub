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
                                                  else
                                                  {
                                                      /*Silent loading */
                                                      await LoadEvents();
                                                  }

                                              }
                                          }));
            }
        }
        public async void RefreshCommand(object sender, EventArgs e)
        {
            
            if (!GlobalHelper.IsInternet())
            {
                //Sending NoInternet message to all viewModels
                Messenger.Default.Send(new GlobalHelper.LocalNotificationMessageType { Message="No Internet", Glyph= "\uE704" });
            }
            else
            {
               
                Messenger.Default.Send(new GlobalHelper.HasInternetMessageType()); //Sending Internet available message to all viewModels
                isLoading = true;
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
                await LoadEvents();
            }
            isLoading = false;

        }

        private async Task LoadEvents()
        {
            Events = await UserUtility.GetUserActivity();
            if(Events!=null)
            {
                ZeroEventCount = (Events.Count == 0) ? true : false;
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
