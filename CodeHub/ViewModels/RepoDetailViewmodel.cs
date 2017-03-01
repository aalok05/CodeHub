using System;
using System.Threading;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using CodeHub.Helpers;
using CodeHub.Services;
using CodeHub.Views;
using Octokit;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media;
using MarkdownSharp;
using Windows.UI.Core;

namespace CodeHub.ViewModels
{
    public class RepoDetailViewmodel : AppViewmodel
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

        public bool _isStar;
        public bool IsStar
        {
            get
            {
                return _isStar;
            }
            set
            {
                Set(() => IsStar, ref _isStar, value);
            }
        }

        public bool _IsWatching;
        public bool IsWatching
        {
            get
            {
                return _IsWatching;
            }
            set
            {
                Set(() => IsWatching, ref _IsWatching, value);
            }
        }

        public bool _IsForkLoading;
        public bool IsForkLoading
        {
            get
            {
                return _IsForkLoading;
            }
            set
            {
                Set(() => IsForkLoading, ref _IsForkLoading, value);
            }
        }

        public async Task Load(object repo)
        {
            if (!GlobalHelper.IsInternet())
            {
                //Sending NoInternet message to all viewModels
                Messenger.Default.Send(new GlobalHelper.NoInternetMessageType());
            }
            else
            {
                //Sending Internet available message to all viewModels
                Messenger.Default.Send(new GlobalHelper.HasInternetMessageType());

                isLoading = true;
                if (repo.GetType() == typeof(string))
                {
                    //Splitting repository name and owner name
                    var names = (repo as string).Split('/');
                    Repository = await RepositoryUtility.GetRepository(names[0], names[1]);
                }
                else
                {
                    Repository = repo as Repository;
                }

                IsStar = await RepositoryUtility.CheckStarred(Repository);
                IsWatching = await RepositoryUtility.CheckWatched(Repository);

                isLoading = false;
            }
        }

        private RelayCommand _sourceCodeNavigate;
        public RelayCommand SourceCodeNavigate
        {
            get
            {
                return _sourceCodeNavigate
                    ?? (_sourceCodeNavigate = new RelayCommand(
                                          () =>
                                          {
                                              SimpleIoc.Default.GetInstance<Services.IAsyncNavigationService>().NavigateAsync(typeof(SourceCodeView), Repository.FullName, Repository);
                                          }));
            }
        }

        private RelayCommand _profileTapped;
        public RelayCommand ProfileTapped
        {
            get
            {
                return _profileTapped
                    ?? (_profileTapped = new RelayCommand(
                                          () =>
                                          {
                                              SimpleIoc.Default.GetInstance<Services.IAsyncNavigationService>().NavigateAsync(typeof(DeveloperProfileView), "Profile", Repository.Owner.Login);
                                          }));
            }
        }

        private RelayCommand _issuesTapped;
        public RelayCommand IssuesTapped
        {
            get
            {
                return _issuesTapped
                    ?? (_issuesTapped = new RelayCommand(
                                          () =>
                                          {
                                              SimpleIoc.Default.GetInstance<Services.IAsyncNavigationService>().NavigateAsync(typeof(IssuesView), "Issues", Repository);
                                          }));
            }
        }

        private RelayCommand _StarCommand;
        public RelayCommand StarCommand
        {
            get
            {
                return _StarCommand
                    ?? (_StarCommand = new RelayCommand(
                                          async () =>
                                          {
                                              if (!IsStar)
                                              {
                                                  if (await RepositoryUtility.StarRepository(Repository))
                                                  {
                                                      IsStar = true;
                                                      GlobalHelper.NewStarActivity = true;
                                                  }
                                              }
                                              else
                                              {
                                                  if (await RepositoryUtility.UnstarRepository(Repository))
                                                  {
                                                      IsStar = false;
                                                      GlobalHelper.NewStarActivity = true;
                                                  }
                                              }
                                          }));
            }
        }

        private RelayCommand _WatchCommand;
        public RelayCommand WatchCommand
        {
            get
            {
                return _WatchCommand
                    ?? (_WatchCommand = new RelayCommand(
                                          async () =>
                                          {
                                              if (!IsWatching)
                                              {
                                                  if (await RepositoryUtility.WatchRepository(Repository))
                                                  {
                                                      IsWatching = true;
                                                  }
                                              }
                                              else
                                              {
                                                  if (await RepositoryUtility.UnwatchRepository(Repository))
                                                  {
                                                      IsWatching = false;
                                                  }
                                              }
                                          }));
            }
        }

        private RelayCommand _ForkCommand;
        public RelayCommand ForkCommand
        {
            get
            {
                return _ForkCommand
                    ?? (_ForkCommand = new RelayCommand(
                                          async () =>
                                          {
                                              IsForkLoading = true;
                                              Repository forkedRepo = await RepositoryUtility.ForkRepository(Repository);
                                              IsForkLoading = false;
                                              if (forkedRepo != null)
                                              {
                                                  SimpleIoc.Default.GetInstance<IAsyncNavigationService>().NavigateAsync(typeof(RepoDetailView), "Repository", forkedRepo);
                                              }
                                          }));
            }
        }
    }
}
