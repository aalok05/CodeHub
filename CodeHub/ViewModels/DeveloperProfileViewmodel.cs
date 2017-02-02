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

namespace CodeHub.ViewModels
{
    public class DeveloperProfileViewmodel : AppViewmodel
    {
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

        public bool _followersLoading;
        public bool FollowersLoading
        {
            get
            {
                return _followersLoading;
            }
            set
            {
                Set(() => FollowersLoading, ref _followersLoading, value);
            }
        }

        public bool _followingLoading;
        public bool FollowingLoading
        {
            get
            {
                return _followingLoading;
            }
            set
            {
                Set(() => FollowingLoading, ref _followingLoading, value);
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
        public async Task Load(string login)
        {
            if (!GlobalHelper.IsInternet())
            {
                Messenger.Default.Send(new GlobalHelper.NoInternetMessageType()); //Sending NoInternet message to all viewModels
            }
            else
            {
                Messenger.Default.Send(new GlobalHelper.HasInternetMessageType()); //Sending Internet available message to all viewModels

                if (!string.IsNullOrWhiteSpace(login))
                {
                    isLoading = true;
                    Developer = await UserUtility.getUserInfo(login);
                    if (Developer != null) await TryLoadUserAvatarImagesAsync(Developer);
                    isLoading = false;
                    if (Developer.Type == AccountType.Organization || Developer.Login == GlobalHelper.UserLogin)
                    {
                        CanFollow = false;
                    }
                    else
                    {
                        CanFollow = true;
                        FollowProgress = true;
                        if (await UserUtility.checkFollow(Developer.Login))
                        {
                            IsFollowing = true;
                        }
                        FollowProgress = false;

                        FollowersLoading = true;
                        Followers = await UserDataService.getAllFollowers(Developer.Login);
                        FollowersLoading = false;

                        FollowingLoading = true;
                        Following = await UserDataService.getAllFollowing(Developer.Login);
                        FollowingLoading = false;
                    }
                }

            }
        }
        public void ProfileTapped(object sender, ItemClickEventArgs e)
        {
            SimpleIoc.Default.GetInstance<Services.INavigationService>().Navigate(typeof(DeveloperProfileView), ((User)e.ClickedItem).Login, "Profile");
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
                                              if (await UserUtility.FollowUser(Developer.Login))
                                              {
                                                  IsFollowing = true;

                                                  Messenger.Default.Send(new GlobalHelper.FollowActivityMessageType());
                                                  GlobalHelper.NewFollowActivity = true;
                                                  
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
                                              await UserUtility.UnFollowUser(Developer.Login);
                                              IsFollowing = false;
                                              FollowProgress = false;

                                              Messenger.Default.Send(new GlobalHelper.FollowActivityMessageType());
                                              GlobalHelper.NewFollowActivity = true;
                                          }));
            }
        }

        private RelayCommand _reposNavigate;
        public RelayCommand NavigateToRepositories
        {
            get
            {
                return _reposNavigate
                    ?? (_reposNavigate = new RelayCommand(
                                          () =>
                                          {
                                              SimpleIoc.Default.GetInstance<INavigationService>().Navigate(typeof(RepoListView), Developer.Login, "Repositories");
                                          }));
            }
        }
        public async void FollowActivity(GlobalHelper.FollowActivityMessageType empty)
        {
            Developer = await UserUtility.getUserInfo(Developer.Login);
            if (Developer != null) await TryLoadUserAvatarImagesAsync(Developer);
            FollowersLoading = true;
            Followers = await UserDataService.getAllFollowers(Developer.Login);
            FollowersLoading = false;
        }
    }
}
