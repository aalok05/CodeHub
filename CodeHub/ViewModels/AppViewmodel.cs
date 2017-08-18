using CodeHub.Helpers;
using CodeHub.Services;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using Octokit;
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Store;
using Windows.System.Profile;

namespace CodeHub.ViewModels
{
    public class AppViewmodel : ViewModelBase
    {
        #region properties
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

        private bool _isDesktopAdsVisible;
        public bool IsDesktopAdsVisible
        {
            get { return _isDesktopAdsVisible; }
            set
            {
                Set(() => IsDesktopAdsVisible, ref _isDesktopAdsVisible, value);
            }
        }

        private bool _isMobileAdsVisible;
        public bool IsMobileAdsVisible
        {
            get { return _isMobileAdsVisible; }
            set
            {
                Set(() => IsMobileAdsVisible, ref _isMobileAdsVisible, value);
            }
        }

        public string WhatsNewText
        {
            get
            {
                return "Hi all,\n Here's the changelog for v 2.2.2 \n\n -Added an expand/collapse button in repository page to view readme comfortably \n -Design improvements \n -Fixed a bug where app was crashing when opened without an internet connection ";
            }
        }
        #endregion

        public AppViewmodel()
        {
            ConfigureAdsVisibility();
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

        public bool HasAlreadyDonated()
        {
            LicenseInformation licenseInformation = CurrentAppSimulator.LicenseInformation;

            string[] addOnIds = new string[] { "[Donate_first_tier_id]", "[Donate_second_tier_id]", "[Donate_third_tier_id]", "[Donate_fourth_tier_id]", "[Donate_fifth_tier_id]", "[Donate_sixth_tier_id]" };

            for (int i = 0; i < addOnIds.Length; i++)
            {
                if (licenseInformation.ProductLicenses[addOnIds[i]].IsActive)
                {
                    return true;
                }
            }

            return false;
        }

        public void ConfigureAdsVisibility()
        {
            if (HasAlreadyDonated())
            {
                GlobalHelper.HasAlreadyDonated = true;
                ToggleAdsVisiblity();
            }
            else
            {
                SettingsService.Save<bool>(SettingsKeys.IsAdsEnabled, true);

                if (AnalyticsInfo.VersionInfo.DeviceFamily == "Windows.Mobile")
                {
                    IsMobileAdsVisible = true;
                    IsDesktopAdsVisible = false;
                }
                else
                {
                    IsDesktopAdsVisible = true;
                    IsMobileAdsVisible = false;
                }
            }
        }

        public void ToggleAdsVisiblity()
        {
            if (SettingsService.Get<bool>(SettingsKeys.IsAdsEnabled))
            {
                if (AnalyticsInfo.VersionInfo.DeviceFamily == "Windows.Mobile")
                {
                    IsMobileAdsVisible = true;
                    IsDesktopAdsVisible = false;
                }
                else
                {
                    IsDesktopAdsVisible = true;
                    IsMobileAdsVisible = false;
                }
            }
            else IsMobileAdsVisible = IsDesktopAdsVisible = false;
        }
    }
}
