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

namespace CodeHub.ViewModels
{
    public class IssuesViewmodel : AppViewmodel
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

        public string _NewIssueTitleText;
        public string NewIssueTitleText
        {
            get
            {
                return _NewIssueTitleText;
            }
            set
            {
                Set(() => NewIssueTitleText, ref _NewIssueTitleText, value);
            }
        }

        public string _NewIssueBodyText;
        public string NewIssueBodyText
        {
            get
            {
                return _NewIssueBodyText;
            }
            set
            {
                Set(() => NewIssueBodyText, ref _NewIssueBodyText, value);
            }
        }

        public bool _zeroOpenIssues;
        /// <summary>
        /// 'No Issues' TextBlock will display if this is true
        /// </summary>
        public bool ZeroOpenIssues
        {
            get
            {
                return _zeroOpenIssues;
            }
            set
            {
                Set(() => ZeroOpenIssues, ref _zeroOpenIssues, value);
            }
        }

        public bool _zeroClosedIssues;
        /// <summary>
        /// 'No Issues' TextBlock will display if this is true
        /// </summary>
        public bool ZeroClosedIssues
        {
            get
            {
                return _zeroClosedIssues;
            }
            set
            {
                Set(() => ZeroClosedIssues, ref _zeroClosedIssues, value);
            }
        }

        public bool _zeroMyIssues;
        /// <summary>
        /// 'No Issues' TextBlock will display if this is true
        /// </summary>
        public bool ZeroMyIssues
        {
            get
            {
                return _zeroMyIssues;
            }
            set
            {
                Set(() => ZeroMyIssues, ref _zeroMyIssues, value);
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

        public bool _isloadingMine;
        public bool IsLoadingMine  //For the third progressRing
        {
            get
            {
                return _isloadingMine;
            }
            set
            {
                Set(() => IsLoadingMine, ref _isloadingMine, value);

            }
        }

        public ObservableCollection<Issue> _openissues;
        public ObservableCollection<Issue> OpenIssues
        {
            get
            {
                return _openissues;
            }
            set
            {
                Set(() => OpenIssues, ref _openissues, value);
            }

        }
        public ObservableCollection<Issue> _closedissues;
        public ObservableCollection<Issue> ClosedIssues
        {
            get
            {
                return _closedissues;
            }
            set
            {
                Set(() => ClosedIssues, ref _closedissues, value);
            }

        }

        public ObservableCollection<Issue> _myissues;
        public ObservableCollection<Issue> MyIssues
        {
            get
            {
                return _myissues;
            }
            set
            {
                Set(() => MyIssues, ref _myissues, value);
            }

        }

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

                /*Clear off Issues of the previous repository*/
                if (OpenIssues != null)
                    OpenIssues.Clear();
                if (ClosedIssues != null)
                    ClosedIssues.Clear();
                if (MyIssues != null)
                    MyIssues.Clear();

                IsLoadingOpen = true;
                OpenIssues = await RepositoryUtility.GetAllIssuesForRepo(Repository.Id, new RepositoryIssueRequest
                {
                    State = ItemStateFilter.Open
                });
                IsLoadingOpen = false;

                ZeroOpenIssues = OpenIssues.Count == 0 ? true : false;

            }
        }

        public void IssueTapped(object sender, ItemClickEventArgs e)
        {
            SimpleIoc.Default.GetInstance<IAsyncNavigationService>()
                            .NavigateAsync(typeof(IssueDetailView), "Issues", new Tuple<Repository, Issue>(Repository, e.ClickedItem as Issue));
        }

        public async void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Pivot p = sender as Pivot;

            if (p.SelectedIndex == 0)
            {
                IsLoadingOpen = true;
                OpenIssues = await RepositoryUtility.GetAllIssuesForRepo(Repository.Id, new RepositoryIssueRequest
                {
                    State = ItemStateFilter.Open
                });
                IsLoadingOpen = false;

                ZeroOpenIssues = OpenIssues.Count == 0 ? true : false;
            }
            else if (p.SelectedIndex == 1)
            {
                IsLoadingClosed = true;

                ClosedIssues = await RepositoryUtility.GetAllIssuesForRepo(Repository.Id, new RepositoryIssueRequest
                {
                    State = ItemStateFilter.Closed
                });
                IsLoadingClosed = false;

                ZeroClosedIssues = ClosedIssues.Count == 0 ? true : false;
            }
            else if (p.SelectedIndex == 2)
            {
                IsLoadingMine = true;
                MyIssues = await UserUtility.GetAllIssuesForRepoByUser(Repository.Id);
                IsLoadingMine = false;

                ZeroMyIssues = MyIssues.Count == 0 ? true : false;
            }
        }

        private RelayCommand _CreateIssue;
        public RelayCommand CreateIssue
        {
            get
            {
                return _CreateIssue
                    ?? (_CreateIssue = new RelayCommand(
                                          async () =>
                                          {
                                              if(!string.IsNullOrWhiteSpace(NewIssueTitleText))
                                              {
                                                  NewIssue newIssue = new NewIssue(NewIssueTitleText);
                                                  newIssue.Body = NewIssueBodyText;
                                                  isLoading = true;
                                                  Issue issue = await IssueUtility.CreateIssue(Repository.Id, newIssue);
                                                  isLoading = false;
                                                  if (issue != null)
                                                  {
                                                      SimpleIoc.Default.GetInstance<IAsyncNavigationService>()
                                                        .NavigateAsync(typeof(IssueDetailView), "Issues", new Tuple<Repository, Issue>(Repository, issue));
                                                  }
                                              }
                                             
                                          }));
            }
        }
    }
}
