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
using Windows.UI.Xaml.Input;

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
        public async Task Load(Repository repo)
        {
            Repository = repo;
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
                IsStar = await RepositoryUtility.CheckStarred(Repository);
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
                                              SimpleIoc.Default.GetInstance<Services.INavigationService>().Navigate(typeof(SourceCodeView), Repository);
                                             
                                          }));
            }
        }

        private RelayCommand _starRepo;
        public RelayCommand StarRepo
        {
            get
            {
                return _starRepo
                    ?? (_starRepo = new RelayCommand(
                                          async () =>
                                          {
                                              isLoading = true;
                                              if (await RepositoryUtility.StarRepository(Repository))
                                              {
                                                  isLoading = false;
                                                  IsStar = true;
                                                  GlobalHelper.NewStarActivity = true;
                                              }
                                          }));
            }
        }

        private RelayCommand _unStarRepo;
        public RelayCommand UnstarRepo
        {
            get
            {
                return _unStarRepo
                    ?? (_unStarRepo = new RelayCommand(
                                          async () =>
                                          {
                                              isLoading = true;
                                              if (await RepositoryUtility.UnstarRepository(Repository))
                                              {
                                                  isLoading = false;
                                                  IsStar = false;
                                                  GlobalHelper.NewStarActivity = true;
                                              }
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
                                              SimpleIoc.Default.GetInstance<Services.INavigationService>().Navigate(typeof(DeveloperProfileView), Repository.Owner.Login);
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
                                              SimpleIoc.Default.GetInstance<Services.INavigationService>().Navigate(typeof(IssuesView), Repository);
                                          }));
            }
        }
    }
}
