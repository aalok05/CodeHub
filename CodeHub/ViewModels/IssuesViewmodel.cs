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

        public double _progressBarValue;
        public double ProgressBarValue
        {
            get
            {
                return _progressBarValue;
            }
            set
            {
                Set(() => ProgressBarValue, ref _progressBarValue, value);

            }
        }

        #endregion
        public async Task Load(Repository repository)
        {
            if (!GlobalHelper.IsInternet())
            {
                Messenger.Default.Send(new GlobalHelper.NoInternetMessageType()); //Sending NoInternet message to all viewModels           
            }
            else
            {
                Messenger.Default.Send(new GlobalHelper.HasInternetMessageType()); //Sending Internet available message to all viewModels

                
                ProgressBarValue = 0;
                isLoading = true; //For the progressBar at the top of the page
                IsLoadingOpen = IsLoadingClosed = IsLoadingMine = true;

                Repository = repository;

                /*Clear off Issues of the previous repository*/
                if (OpenIssues!=null)
                   OpenIssues.Clear();
                if (ClosedIssues != null)
                     ClosedIssues.Clear();
                if (MyIssues != null)
                     MyIssues.Clear(); 

                OpenIssues = await RepositoryUtility.GetAllIssuesForRepo(Repository.Id, new RepositoryIssueRequest
                {
                    State = ItemStateFilter.Open
                });
                IsLoadingOpen = false;
                ProgressBarValue += 100/3;

                ZeroOpenIssues = OpenIssues.Count == 0 ? true : false;

                ClosedIssues = await RepositoryUtility.GetAllIssuesForRepo(Repository.Id, new RepositoryIssueRequest {
                    State = ItemStateFilter.Closed
                });
                IsLoadingClosed = false;
                ProgressBarValue += 100/3;

                ZeroClosedIssues = ClosedIssues.Count == 0 ? true : false;

                MyIssues = await RepositoryUtility.GetAllIssuesForRepoByUser(Repository.Id);
                ProgressBarValue += 100 / 3;
                IsLoadingMine = false;

                ZeroMyIssues = MyIssues.Count == 0 ? true : false;

                isLoading = false;
             
            }
        }

        public void IssueTapped(object sender, ItemClickEventArgs e)
        {
            SimpleIoc.Default.GetInstance<INavigationService>()
                            .Navigate(typeof(IssueDetailView), new Tuple<string, string, Issue>(Repository.Owner.Login, Repository.Name, e.ClickedItem as Issue), "Issues");
        }
    }
}
