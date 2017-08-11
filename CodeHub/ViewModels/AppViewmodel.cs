using CodeHub.Services;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using Octokit;
using System;
using System.Threading.Tasks;

namespace CodeHub.ViewModels
{
    public class AppViewmodel : ViewModelBase
    {
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

        public bool _IsNotificationsUnread;
        public bool IsNotificationsUnread
        {
            get
            {
                return _IsNotificationsUnread;
            }
            set
            {
                Set(() => IsNotificationsUnread, ref _IsNotificationsUnread, value);
            }
        }

        public string WhatsNewText
        {
            get
            {
                return "Hi all,\n Here's the changelog for v 2.2.2 \n\n -Added an expand/collapse button in repository page to view readme comfortably \n -Design improvements \n -Fixed a bug where app was crashing when opened without an internet connection ";
            }
        }

        public async void MarkdownTextBlock_LinkClicked(object sender, Microsoft.Toolkit.Uwp.UI.Controls.LinkClickedEventArgs e)
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri(e.Link));
        }

        public void Navigate(Type pageType, string pageTitle)
        {
            SimpleIoc.Default.GetInstance<Services.IAsyncNavigationService>().NavigateAsync(pageType, pageTitle, User);
        }
        public void GoBack()
        {
            SimpleIoc.Default.GetInstance<Services.IAsyncNavigationService>().GoBackAsync();
        }

        public void UpdateUnreadNotificationIndicator(bool IsUnread)
        {
            IsNotificationsUnread = IsUnread;
        }

        public async Task CheckForUnreadNotifications()
        {
            var unread = await NotificationsService.GetAllNotificationsForCurrentUser(false, false);
            if (unread != null)
            {
                if (unread.Count > 0)
                    UpdateUnreadNotificationIndicator(true);
                else
                    UpdateUnreadNotificationIndicator(false);
            }
        }
    }
}
