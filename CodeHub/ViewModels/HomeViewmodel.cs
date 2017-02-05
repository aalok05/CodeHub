using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using CodeHub.Helpers;
using CodeHub.Services;
using CodeHub.Views;
using Microsoft.Toolkit.Uwp;
using Octokit;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using System.Threading;

namespace CodeHub.ViewModels
{
    public class HomeViewmodel : AppViewmodel
    {
        public enum TimeRange
        {
            TODAY, WEEKLY, MONTHLY
        }

        public bool _zeroTodayCount;
        /// <summary>
        /// 'Trending Repositories are being updated by Github' textblock will be displayed if this is true
        /// </summary>
        public bool ZeroTodayCount
        {
            get
            {
                return _zeroTodayCount;
            }
            set
            {
                Set(() => ZeroTodayCount, ref _zeroTodayCount, value);
            }
        }

        public bool _zeroWeeklyCount;
        /// <summary>
        /// 'Trending Repositories are being updated by Github' textblock will be displayed if this is true
        /// </summary>
        public bool ZeroWeeklyCount
        {
            get
            {
                return _zeroWeeklyCount;
            }
            set
            {
                Set(() => ZeroWeeklyCount, ref _zeroWeeklyCount, value);
            }
        }
        public bool _zeroMonthlyCount;
        /// <summary>
        /// 'Trending Repositories are being updated by Github' textblock will be displayed if this is true
        /// </summary>
        public bool ZeroMonthlyCount
        {
            get
            {
                return _zeroMonthlyCount;
            }
            set
            {
                Set(() => ZeroMonthlyCount, ref _zeroMonthlyCount, value);
            }
        }

        public ObservableCollection<Repository> _trendingReposToday;
        public ObservableCollection<Repository> TrendingReposToday
        {
            get
            {
                return _trendingReposToday;
            }
            set
            {
                Set(() => TrendingReposToday, ref _trendingReposToday, value);

            }

        }

        public ObservableCollection<Repository> _trendingReposWeek;
        public ObservableCollection<Repository> TrendingReposWeek
        {
            get
            {
                return _trendingReposWeek;
            }
            set
            {
                Set(() => TrendingReposWeek, ref _trendingReposWeek, value);

            }

        }

        public ObservableCollection<Repository> _trendingReposMonth;
        public ObservableCollection<Repository> TrendingReposMonth
        {
            get
            {
                return _trendingReposMonth;
            }
            set
            {
                Set(() => TrendingReposMonth, ref _trendingReposMonth, value);

            }

        }

        public bool _isIncrementalLoadingToday;
        public bool IsIncrementalLoadingToday  //For the Incremental Loading call
        {
            get
            {
                return _isIncrementalLoadingToday;
            }
            set
            {
                Set(() => IsIncrementalLoadingToday, ref _isIncrementalLoadingToday, value);

            }
        }
        public bool _isIncrementalLoadingWeek;
        public bool IsIncrementalLoadingWeek  //For the Incremental Loading call
        {
            get
            {
                return _isIncrementalLoadingWeek;
            }
            set
            {
                Set(() => IsIncrementalLoadingWeek, ref _isIncrementalLoadingWeek, value);

            }
        }
        public bool _isIncrementalLoadingMonth;
        public bool IsIncrementalLoadingMonth  //For the Incremental Loading call
        {
            get
            {
                return _isIncrementalLoadingMonth;
            }
            set
            {
                Set(() => IsIncrementalLoadingMonth, ref _isIncrementalLoadingMonth, value);

            }
        }

        public bool _isloadingToday;
        public bool IsLoadingToday  //For the first progressRing
        {
            get
            {
                return _isloadingToday;
            }
            set
            {
                Set(() => IsLoadingToday, ref _isloadingToday, value);

            }
        }

        public bool _isloadingWeek;
        public bool IsLoadingWeek   //For the second progressRing
        {
            get
            {
                return _isloadingWeek;
            }
            set
            {
                Set(() => IsLoadingWeek, ref _isloadingWeek, value);

            }
        }

        public bool _isloadingMonth;
        public bool IsLoadingMonth  //For the third progressRing
        {
            get
            {
                return _isloadingMonth;
            }
            set
            {
                Set(() => IsLoadingMonth, ref _isloadingMonth, value);

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
                                                  Messenger.Default.Send(new GlobalHelper.NoInternetMessageType()); //Sending NoInternet message to all viewModels           
                                              }
                                              else
                                              {
                                                  Messenger.Default.Send(new GlobalHelper.HasInternetMessageType()); //Sending Internet available message to all viewModels
                                                  if (TrendingReposToday == null)
                                                  {
                                                      IsLoadingToday = IsLoadingWeek = IsLoadingMonth = true;

                                                      await LoadTrendingRepos(TimeRange.TODAY);
                                                      await LoadTrendingRepos(TimeRange.WEEKLY);
                                                      await LoadTrendingRepos(TimeRange.MONTHLY);
                                                  }

                                              }

                                          }));
            }
        }

        public async void RefreshTodayCommand(object sender, EventArgs e)
        {
            if (!GlobalHelper.IsInternet())
            {
                Messenger.Default.Send(new GlobalHelper.NoInternetMessageType()); //Sending NoInternet message to all viewModels
            }
            else
            {
                Messenger.Default.Send(new GlobalHelper.HasInternetMessageType()); //Sending Internet available message to all viewModels
                IsLoadingToday = true;
                await LoadTrendingRepos(TimeRange.TODAY);
            }
            IsLoadingToday = false;
        }
        public async void RefreshWeekCommand(object sender, EventArgs e)
        {
            if (!GlobalHelper.IsInternet())
            {
                Messenger.Default.Send(new GlobalHelper.NoInternetMessageType()); //Sending NoInternet message to all viewModels
            }
            else
            {
                Messenger.Default.Send(new GlobalHelper.HasInternetMessageType()); //Sending Internet available message to all viewModels
                IsLoadingWeek = true;

                await LoadTrendingRepos(TimeRange.WEEKLY);
            }
            IsLoadingWeek = false;
        }
        public async void RefreshMonthCommand(object sender, EventArgs e)
        {
            if (!GlobalHelper.IsInternet())
            {
                Messenger.Default.Send(new GlobalHelper.NoInternetMessageType()); //Sending NoInternet message to all viewModels
            }
            else
            {
                Messenger.Default.Send(new GlobalHelper.HasInternetMessageType()); //Sending Internet available message to all viewModels
                IsLoadingMonth = true;
                await LoadTrendingRepos(TimeRange.MONTHLY);
            }
            IsLoadingMonth = false;
        }
        public void RepoDetailNavigateCommand(object sender, ItemClickEventArgs e)
        {
            SimpleIoc.Default.GetInstance<Services.IAsyncNavigationService>().NavigateAsync(typeof(RepoDetailView), "Repository", e.ClickedItem as Repository);
        }
        private async Task LoadTrendingRepos(TimeRange range)
        {
            if (range == TimeRange.TODAY)
            {
                var repos = await RepositoryUtility.GetTrendingRepos(range, true);
                IsLoadingToday = false;
                if (repos != null)
                {
                    ZeroTodayCount = false;
                    TrendingReposToday = repos;

                    IsIncrementalLoadingToday = true;
                    //Second Incremental call
                    repos = await RepositoryUtility.GetTrendingRepos(range, false);
                    IsIncrementalLoadingToday = false;

                    if (repos != null)
                    {
                        foreach (var i in repos)
                        {
                            TrendingReposToday.Add(i);
                        }
                    }
                }
                else
                {
                    ZeroTodayCount = true;
                    if(TrendingReposToday != null)
                        TrendingReposToday.Clear(); 

                }
            }
            else if (range == TimeRange.WEEKLY)
            {
                var repos = await RepositoryUtility.GetTrendingRepos(range, true);
                IsLoadingWeek = false;
                if (repos != null)
                {
                    ZeroWeeklyCount = false;
                    TrendingReposWeek = repos;

                    IsIncrementalLoadingWeek = true;
                    //Second Incremental call
                    repos = await RepositoryUtility.GetTrendingRepos(range, false);
                    IsIncrementalLoadingWeek = false;

                    if (repos != null)
                    {
                        foreach (var i in repos)
                        {
                            TrendingReposWeek.Add(i);
                        }
                    }
                }
                else
                {
                    ZeroWeeklyCount = true;
                    if(TrendingReposWeek != null)
                         TrendingReposWeek.Clear();
                }
            }
            else
            {
                var repos = await RepositoryUtility.GetTrendingRepos(range, true);
                IsLoadingMonth = false;
                if (repos != null)
                {
                    ZeroMonthlyCount = false;
                    TrendingReposMonth = repos;

                    IsIncrementalLoadingMonth = true;
                    //Second Incremental call
                    repos = await RepositoryUtility.GetTrendingRepos(range, false);
                    IsIncrementalLoadingMonth = false;

                    if (repos != null)
                    {
                        foreach (var i in repos)
                        {
                            TrendingReposMonth.Add(i);
                        }
                    }
                }
                else
                {
                    ZeroMonthlyCount = true;
                    if (TrendingReposMonth != null)
                        TrendingReposMonth.Clear();
                }
            }

        }

    }
}
