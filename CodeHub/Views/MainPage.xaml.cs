using System;
using System.Threading;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using CodeHub.Helpers;
using CodeHub.Models;
using CodeHub.Services;
using CodeHub.ViewModels;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using static CodeHub.Helpers.GlobalHelper;
using Octokit;
using CodeHub.Controls;
using UICompositionAnimations;
using UICompositionAnimations.Enums;
using Windows.UI.ViewManagement;
using RavinduL.LocalNotifications;
using RavinduL.LocalNotifications.Presenters;
using Windows.UI.Popups;
using Windows.System.Profile;

namespace CodeHub.Views
{
    public sealed partial class MainPage : Windows.UI.Xaml.Controls.Page
    {
        public MainViewmodel ViewModel { get; set; }
        public CustomFrame AppFrame { get { return this.mainFrame; } }
        private readonly SemaphoreSlim HeaderAnimationSemaphore = new SemaphoreSlim(1);
        private LocalNotificationManager notifManager;

        public MainPage()
        {
            this.InitializeComponent();

            ViewModel = new MainViewmodel();
            this.DataContext = ViewModel;

            SizeChanged += MainPage_SizeChanged;

            #region registering for messages
            Messenger.Default.Register<LocalNotificationMessageType>(this, RecieveLocalNotificationMessage);
            Messenger.Default.Register(this, delegate(SetHeaderTextMessageType m) {  SetHeadertext(m.PageName); });
            Messenger.Default.Register(this, delegate (AdsEnabledMessageType m) {  ToggleAdsVisibility(m.isEnabled); });
            #endregion

            SimpleIoc.Default.Register<IAsyncNavigationService>(() =>
            { return new NavigationService(mainFrame); });
            
            NavigationCacheMode = NavigationCacheMode.Enabled;

            SystemNavigationManager.GetForCurrentView().BackRequested += SystemNavigationManager_BackRequested;

            ConfigureAdsVisibility();
        }
        private async void OnCurrentStateChanged(object sender, VisualStateChangedEventArgs e)
        {
            if(e.NewState != null)
                ViewModel.CurrentState = e.NewState.Name;

            await HeaderText.StartCompositionFadeSlideAnimationAsync(1, 0, TranslationAxis.X, 0, 24, 150, null, null, EasingFunctionNames.Linear);
            await HeaderText.StartCompositionFadeSlideAnimationAsync(0, 1, TranslationAxis.X, 24, 0, 150, null, null, EasingFunctionNames.Linear);
        }
        private void MainPage_SizeChanged(object sender, SizeChangedEventArgs e)
        {

            if (e.NewSize.Width < 720)
            {   
                if (ViewModel.isLoggedin)
                {
                    BottomAppBar.Visibility = Visibility.Visible;
                }
                else
                {
                    BottomAppBar.Visibility = Visibility.Collapsed;
                }
            }
        }
        private void SystemNavigationManager_BackRequested(object sender, BackRequestedEventArgs e)
        {
            if (AppFrame == null) return;
            IAsyncNavigationService service = SimpleIoc.Default.GetInstance<IAsyncNavigationService>();
            if (service != null && AppFrame.CanGoBack && !e.Handled) // The base CanGoBack is synchronous and not reliable here
            {
                e.Handled = true;
                service.GoBackAsync(); // Use the navigation service to make sure the navigation is possible
            }
        }

        #region click events
        private void HamButton_Click(object sender, RoutedEventArgs e)
        {
            //Toggle Hamburger menu
            HamSplitView.IsPaneOpen = !HamSplitView.IsPaneOpen;
        }
        private void HamListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (SimpleIoc.Default.GetInstance<IAsyncNavigationService>().CurrentSourcePageType != (e.ClickedItem as HamItem).DestPage)
            {
                ViewModel.HamItemClicked(e.ClickedItem as HamItem);

                //Don't close the Hamburger menu if visual state is DesktopEx
                if (ViewModel.CurrentState != "DesktopEx")
                    HamSplitView.IsPaneOpen = false;
            }
        }
        private void SettingsItem_ItemClick(object sender, TappedRoutedEventArgs e)
        {
            if (SimpleIoc.Default.GetInstance<IAsyncNavigationService>().CurrentSourcePageType != typeof(SettingsView))
            {
                ViewModel.NavigateToSettings();

                //Don't close the Hamburger menu if visual state is DesktopEx
                if (ViewModel.CurrentState != "DesktopEx")
                    HamSplitView.IsPaneOpen = false;
            }
        }
        #endregion

        #region App Bar Events
        private void AppBarTrending_Tapped(object sender, TappedRoutedEventArgs e)
        {
            //Navigate to Trending page using the BottomAppBar
            if (SimpleIoc.Default.GetInstance<IAsyncNavigationService>().CurrentSourcePageType != ViewModel.HamItems[0].DestPage)
                ViewModel.HamItemClicked(ViewModel.HamItems[0]);
        }
        private void AppBarNewsFeed_Tapped(object sender, TappedRoutedEventArgs e)
        {
            //Navigate to News feed page using the BottomAppBar
            if (SimpleIoc.Default.GetInstance<IAsyncNavigationService>().CurrentSourcePageType != ViewModel.HamItems[1].DestPage)
                ViewModel.HamItemClicked(ViewModel.HamItems[1]);
        }
        private void AppBarProfile_Tapped(object sender, TappedRoutedEventArgs e)
        {
            //Navigate to Profile page using the BottomAppBar
            if (SimpleIoc.Default.GetInstance<IAsyncNavigationService>().CurrentSourcePageType != ViewModel.HamItems[2].DestPage)
                ViewModel.HamItemClicked(ViewModel.HamItems[2]);
        }
        private void AppBarMyRepos_Tapped(object sender, TappedRoutedEventArgs e)
        {
            //Navigate to My Repositories page using the BottomAppBar
            if (SimpleIoc.Default.GetInstance<IAsyncNavigationService>().CurrentSourcePageType != ViewModel.HamItems[3].DestPage)
                ViewModel.HamItemClicked(ViewModel.HamItems[3]);
        }
        private void AppBarMyOrganizations_Tapped(object sender, TappedRoutedEventArgs e)
        {
            //Navigate to My Organizations page using the BottomAppBar
            if (SimpleIoc.Default.GetInstance<IAsyncNavigationService>().CurrentSourcePageType != ViewModel.HamItems[4].DestPage)
                ViewModel.HamItemClicked(ViewModel.HamItems[4]);
        }
        #endregion

        #region Messaging
        public void RecieveLocalNotificationMessage(LocalNotificationMessageType notif)
        {
            notifManager.Show(new SimpleNotificationPresenter
            (
                TimeSpan.FromSeconds(3),
                text: notif.Message,
                action: async () => await new MessageDialog(notif.Message).ShowAsync(),
                glyph: notif.Glyph
            )
            {
                Background = GetSolidColorBrush("60B53BFF"),
                Foreground = GetSolidColorBrush("FAFBFCFF"),
            },
            LocalNotificationCollisionBehaviour.Replace);
        }
        public void RecieveSignOutMessage(SignOutMessageType empty)
        {
            if (ViewModel.CurrentState == "Mobile")
            {
                BottomAppBar.Visibility = Visibility.Collapsed;
            }
        }
        public void RecieveSignInMessage(User user)
        {
            if (ViewModel.CurrentState == "Mobile")
            {
                BottomAppBar.Visibility = Visibility.Visible;
            }
            if (SimpleIoc.Default.GetInstance<IAsyncNavigationService>().CurrentSourcePageType != typeof(HomeView))
            {
                SimpleIoc.Default.GetInstance<IAsyncNavigationService>().NavigateAsync(typeof(HomeView), "Trending");
            }
        }
        #endregion

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            ViewModel.isLoggedin = (bool)e.Parameter;

            /* This has to be done because the visibilty of BottomAppBar is dependent on screen size as well as isLoggedin property
             * If visibility is bound with isLoggedin, it will disregard VisualStateManager at first loading of Page.
             */
            if (ViewModel.isLoggedin)
            {
                SimpleIoc.Default.GetInstance<IAsyncNavigationService>().NavigateAsync(typeof(HomeView), "Trending");
                if (Window.Current.Bounds.Width < 720)
                {
                    FindName("BottomAppBar");
                    ViewModel.CurrentState = "Mobile";
                }
                else
                    ViewModel.CurrentState = "Desktop";
            }

            //Listening for Sign In message
            Messenger.Default.Register<User>(this, RecieveSignInMessage);

            //listen for sign out message
            Messenger.Default.Register<SignOutMessageType>(this, RecieveSignOutMessage);

            notifManager = new LocalNotificationManager(NotificationGrid);
        }

        #region other methods

        /// <summary>
        /// Sets the Header Text to pageName
        /// </summary>
        /// <param name="pageName"></param>
        public async void SetHeadertext(string pageName)
        {
            await HeaderAnimationSemaphore.WaitAsync();
            if (ViewModel.HeaderText?.Equals(pageName.ToUpper()) != true)
            {
                await HeaderText.StartCompositionFadeSlideAnimationAsync(1, 0, TranslationAxis.X, 0, 24, 150, null, null, EasingFunctionNames.Linear);
                ViewModel.HeaderText = pageName.ToUpper();
                await HeaderText.StartCompositionFadeSlideAnimationAsync(0, 1, TranslationAxis.X, 24, 0, 150, null, null, EasingFunctionNames.Linear);
            }
            HeaderAnimationSemaphore.Release();
        }

        /// <summary>
        /// Sets the visibility of Ad units according to the app settings
        /// </summary>
        public void ConfigureAdsVisibility()
        {
            if (SettingsService.Get<bool>(SettingsKeys.IsAdsEnabled))
            {
                if (AnalyticsInfo.VersionInfo.DeviceFamily == "Windows.Mobile")
                    adControlDesktop.Visibility = Visibility.Collapsed;
                else
                    adControlMobile.Visibility = Visibility.Collapsed;
            }
            else
            {
                adControlMobile.Visibility = adControlDesktop.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Toggles visibility of Ad units 
        /// </summary>
        /// <param name="isEnabled"></param>
        public void ToggleAdsVisibility(bool isEnabled)
        {
            if (isEnabled)
            {
                if (AnalyticsInfo.VersionInfo.DeviceFamily == "Windows.Mobile")
                    adControlMobile.Visibility = Visibility.Visible;
                else
                    adControlDesktop.Visibility = Visibility.Visible;
            }
            else
            {
                adControlMobile.Visibility = adControlDesktop.Visibility = Visibility.Collapsed;
            }
        }
        #endregion

    }
}
