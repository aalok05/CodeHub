using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using Octokit;
using System;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;
using CodeHub.Services;
using JetBrains.Annotations;

namespace CodeHub.ViewModels
{
    public class AppViewmodel : ViewModelBase
    {
        public bool _hasInternet;
        public bool HasInternet
        {
            get
            {
                return _hasInternet;
            }
            set
            {
                Set(() => HasInternet, ref _hasInternet, value);
            }
        }

        public bool _isLoggedin;
        public bool isLoggedin
        {
            get
            {
                return _isLoggedin;
            }
            set
            {
                Set(() => isLoggedin, ref _isLoggedin, value);
            }
        }

        public bool _isLoading;
        public bool isLoading
        {
            get
            {
                return _isLoading;
            }
            set
            {
                Set(() => isLoading, ref _isLoading, value);
            }
        }

        public User _user;
        public User User
        {
            get
            {
                return _user;
            }
            set
            {
                Set(() => User, ref _user, value);
            }
        }
        private ImageSource _UserAvatar;

        /// <summary>		
        /// Gets the avatar image of the current user (manually loaded to save internet data)		
        /// </summary>		
        public ImageSource UserAvatar
        {
            get { return _UserAvatar; }
            protected set { Set(() => UserAvatar, ref _UserAvatar, value); }
        }

        private ImageSource _UserBlurredAvatar;

        /// <summary>		
        /// Gets the blurred avatar image of the current user		
        /// </summary>		
        public ImageSource UserBlurredAvatar
        {
            get { return _UserBlurredAvatar; }
            protected set { Set(() => UserBlurredAvatar, ref _UserBlurredAvatar, value); }
        }

        /// <summary>		
        /// Updates the two avatar images for the current user		
        /// </summary>		
        /// <param name="overrideUser">An optional, explicit user to load instead of the default one</param>		
        /// <param name="blur">The amount of blur to use</param>		
        protected async Task TryLoadUserAvatarImagesAsync([CanBeNull] User overrideUser = null, int blur = 128)
        {
            // Pick the target user to load		
            User candidate = overrideUser ?? User;
            if (candidate == null) return;

            // Update the images		
            CancellationTokenSource cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
            Tuple<ImageSource, ImageSource> images = await UserUtility.GetDeveloperAvatarOptionsAsync(candidate, blur, cts.Token);
            UserAvatar = images?.Item1;
            UserBlurredAvatar = images?.Item2;
        }

        public void Navigate(Type pageType, string pageTitle)
        {
            SimpleIoc.Default.GetInstance<Services.IAsyncNavigationService>().NavigateAsync(pageType, pageTitle, User);
        }
        public void GoBack()
        {
            SimpleIoc.Default.GetInstance<Services.IAsyncNavigationService>().GoBackAsync();
        }
    }
}
