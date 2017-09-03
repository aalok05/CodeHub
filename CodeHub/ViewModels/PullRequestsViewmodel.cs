using CodeHub.Helpers;
using CodeHub.Services;
using CodeHub.Views;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using Octokit;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace CodeHub.ViewModels
{
    public class PullRequestsViewmodel : AppViewmodel
    {
        #region properties
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

        public bool _isloadingOpen;
        public bool IsLoadingOpen  //For the first progressRing
        {
            get
            {
                return _isloadingOpen;
            }
            set
            {
                Set(() => IsLoadingOpen, ref _isloadingOpen, value);

            }
        }

        public bool _isloadingClosed;
        public bool IsLoadingClosed   //For the second progressRing
        {
            get
            {
                return _isloadingClosed;
            }
            set
            {
                Set(() => IsLoadingClosed, ref _isloadingClosed, value);

            }
        }

        public ObservableCollection<PullRequest> _OpenPullRequests;
        public ObservableCollection<PullRequest> OpenPullRequests
        {
            get
            {
                return _OpenPullRequests;
            }
            set
            {
                Set(() => OpenPullRequests, ref _OpenPullRequests, value);
            }

        }

        public ObservableCollection<PullRequest> _ClosedPullRequests;
        public ObservableCollection<PullRequest> ClosedPullRequests
        {
            get
            {
                return _ClosedPullRequests;
            }
            set
            {
                Set(() => ClosedPullRequests, ref _ClosedPullRequests, value);
            }

        }

        public bool _zeroOpenPullRequests;
        /// <summary>
        /// 'No Issues' TextBlock will display if this is true
        /// </summary>
        public bool ZeroOpenPullRequests
        {
            get
            {
                return _zeroOpenPullRequests;
            }
            set
            {
                Set(() => ZeroOpenPullRequests, ref _zeroOpenPullRequests, value);
            }
        }

        public bool _zeroClosedPullRequests;
        /// <summary>
        /// 'No Pull Requests' TextBlock will display if this is true
        /// </summary>
        public bool ZeroClosedPullRequests
        {
            get
            {
                return _zeroClosedPullRequests;
            }
            set
            {
                Set(() => ZeroClosedPullRequests, ref _zeroClosedPullRequests, value);
            }
        }

        public bool _isIncrementalLoadingOpen;
        public bool IsIncrementalLoadingOpen
        {
            get
            {
                return _isIncrementalLoadingOpen;
            }
            set
            {
                Set(() => IsIncrementalLoadingOpen, ref _isIncrementalLoadingOpen, value);

            }
        }
        public bool _isIncrementalLoadingClosed;
        public bool IsIncrementalLoadingClosed
        {
            get
            {
                return _isIncrementalLoadingClosed;
            }
            set
            {
                Set(() => IsIncrementalLoadingClosed, ref _isIncrementalLoadingClosed, value);

            }
        }

        public int OpenPaginationIndex{ get; set; }
        public int ClosedPaginationIndex{ get; set; }

        public double MaxOpenScrollViewerVerticalffset { get; set; }
        public double MaxClosedScrollViewerVerticalffset { get; set; }

        #endregion

        public async Task Load(Repository repository)
        {
            if (!GlobalHelper.IsInternet())
            {
                //Sending NoInternet message to all viewModels
                Messenger.Default.Send(new GlobalHelper.LocalNotificationMessageType { Message = "No Internet", Glyph = "\uE704" });
            }
            else
            {
                Repository = repository;
                OpenPaginationIndex = ClosedPaginationIndex = 0;

                /*Clear off Pull Requests of the previous repository*/
                if (OpenPullRequests != null)
                    OpenPullRequests.Clear();
                if (ClosedPullRequests != null)
                    ClosedPullRequests.Clear();

                IsLoadingOpen = true;
                OpenPaginationIndex++;
                OpenPullRequests = await RepositoryUtility.GetAllPullRequestsForRepo(Repository.Id, new PullRequestRequest
                {
                    State = ItemStateFilter.Open
                },
                OpenPaginationIndex);
                IsLoadingOpen = false;

                ZeroOpenPullRequests = OpenPullRequests.Count == 0 ? true : false;

            }
        }

        public void PullRequestTapped(object sender, ItemClickEventArgs e)
        {
            SimpleIoc.Default.GetInstance<IAsyncNavigationService>()
                            .NavigateAsync(typeof(PullRequestDetailView), new Tuple<Repository, PullRequest>(Repository, e.ClickedItem as PullRequest));
        }

        public async void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Pivot p = sender as Pivot;

            if (p.SelectedIndex == 0)
            {
                IsLoadingOpen = true;
                OpenPullRequests = await RepositoryUtility.GetAllPullRequestsForRepo(Repository.Id, new PullRequestRequest
                {
                    State = ItemStateFilter.Open
                },
                OpenPaginationIndex = 1);
                IsLoadingOpen = false;

                ZeroOpenPullRequests = OpenPullRequests.Count == 0 ? true : false;
                MaxOpenScrollViewerVerticalffset = 0;
            }
            else if (p.SelectedIndex == 1)
            {
                IsLoadingClosed = true;

                ClosedPullRequests = await RepositoryUtility.GetAllPullRequestsForRepo(Repository.Id, new PullRequestRequest
                {
                    State = ItemStateFilter.Closed
                },
                ClosedPaginationIndex = 1);
                IsLoadingClosed = false;

                ZeroClosedPullRequests = ClosedPullRequests.Count == 0 ? true : false;
                MaxClosedScrollViewerVerticalffset = 0;
            }
        }

        public async Task OpenIncrementalLoad()
        {
            OpenPaginationIndex++;
            IsIncrementalLoadingOpen = true;
            var PRs = await RepositoryUtility.GetAllPullRequestsForRepo(Repository.Id, new PullRequestRequest
            {
                State = ItemStateFilter.Open
            },
            OpenPaginationIndex);

            IsIncrementalLoadingOpen = false;

            if (PRs != null)
            {
                if (PRs.Count > 0)
                {
                    foreach (var i in PRs)
                    {
                        OpenPullRequests.Add(i);
                    }
                }
                else
                {
                    //no more issues left to load
                    OpenPaginationIndex = -1;
                }

            }
        }

        public async Task ClosedIncrementalLoad()
        {
            ClosedPaginationIndex++;
            IsIncrementalLoadingClosed = true;
            var PRs = await RepositoryUtility.GetAllPullRequestsForRepo(Repository.Id, new PullRequestRequest
            {
                State = ItemStateFilter.Closed
            },
            ClosedPaginationIndex);

            IsIncrementalLoadingClosed = false;

            if (PRs != null)
            {
                if (PRs.Count > 0)
                {
                    foreach (var i in PRs)
                    {
                        ClosedPullRequests.Add(i);
                    }
                }
                else
                {
                    //no more issues left to load
                    ClosedPaginationIndex = -1;
                }

            }
        }
    }
}
