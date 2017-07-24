using System;
using System.Threading;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using CodeHub.Services;
using CodeHub.ViewModels;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using static CodeHub.Helpers.GlobalHelper;
using Octokit;
using CodeHub.Controls;
using UICompositionAnimations;
using UICompositionAnimations.Enums;
using RavinduL.LocalNotifications;
using RavinduL.LocalNotifications.Presenters;
using Windows.UI.Popups;
using UICompositionAnimations.Behaviours;
using Windows.UI.Xaml.Media;
using System.Threading.Tasks;
using CodeHub.Helpers;
using UICompositionAnimations.Brushes;
using UICompositionAnimations.Helpers;
using Windows.UI.Xaml.Controls;
using CodeHub.Models;

namespace CodeHub.Views
{
    public sealed partial class MainPage : Windows.UI.Xaml.Controls.Page
    {
        public MainViewmodel ViewModel { get; set; }
        public CustomFrame AppFrame { get { return mainFrame; } }
        private readonly SemaphoreSlim HeaderAnimationSemaphore = new SemaphoreSlim(1);
        private LocalNotificationManager notifManager;

        public MainPage()
        {
            this.InitializeComponent();

            ViewModel = new MainViewmodel();
            this.DataContext = ViewModel;

            #region registering for messages
            Messenger.Default.Register<LocalNotificationMessageType>(this, RecieveLocalNotificationMessage);
            Messenger.Default.Register(this, delegate(SetHeaderTextMessageType m) {  SetHeadertext(m.PageName); });
            Messenger.Default.Register(this, delegate (AdsEnabledMessageType m) { ViewModel.ConfigureAdsVisibility(); });
            Messenger.Default.Register(this, delegate (HostWindowBlurMessageType m) { ConfigureWindowBlur(); });
            Messenger.Default.Register(this, delegate (UpdateUnreadNotificationMessageType m) { ViewModel.UpdateUnreadNotificationIndicator(m.IsUnread); });
            Messenger.Default.Register<User>(this, ViewModel.RecieveSignInMessage);
            #endregion

            Loaded += MainPage_Loaded;

            SimpleIoc.Default.Register<IAsyncNavigationService>(() =>
            { return new NavigationService(mainFrame); });
            
            NavigationCacheMode = NavigationCacheMode.Enabled;
            SystemNavigationManager.GetForCurrentView().BackRequested += SystemNavigationManager_BackRequested;
        }

        private async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            await ViewModel.Initialize();

            if (ViewModel.isLoggedin)
            {
                await SimpleIoc.Default.GetInstance<IAsyncNavigationService>().NavigateAsync(typeof(FeedView), "News Feed");
                await ViewModel.CheckForUnreadNotifications();

                if (WhatsNewDisplayService.IsNewVersion())
                {
                    ViewModel.isLoading = true;
                    WhatsNewPopup.SetVisualOpacity(0);
                    WhatsNewPopup.Visibility = Visibility.Visible;
                    await WhatsNewPopup.StartCompositionFadeScaleAnimationAsync(0, 1, 1.1f, 1, 150, null, 0, EasingFunctionNames.SineEaseInOut);
                }
            }

            notifManager = new LocalNotificationManager(NotificationGrid);

            ConfigureWindowBlur();
            await ConfigureHamburgerMenuBlur();
        }

        private void OnCurrentStateChanged(object sender, VisualStateChangedEventArgs e)
        {
            if(e.NewState.Name == "Desktop" || e.NewState.Name == "Mobile")
            {
                ViewModel.DisplayMode = SplitViewDisplayMode.Overlay;
            }
            else ViewModel.DisplayMode = SplitViewDisplayMode.Inline;
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
        private void HamListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            ViewModel.HamItemClicked(e.ClickedItem as HamItem);
        }
        private async void SignOutFlyout_Tapped(object sender, TappedRoutedEventArgs e)
        {
            moreButton.Flyout.Hide();
            await ViewModel.SignOut();
        }
        private async void CloseWhatsNew_Tapped(object sender, RoutedEventArgs e)
        {
            await WhatsNewPopup.StartCompositionFadeScaleAnimationAsync(1, 0, 1, 1.1f, 150, null, 0, EasingFunctionNames.SineEaseInOut);
            ViewModel.isLoading = false;
            WhatsNewPopup.Visibility = Visibility.Collapsed;
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
        #endregion
 
        #region other methods

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

        public void ConfigureWindowBlur()
        {
            if (SettingsService.Get<bool>(SettingsKeys.IsAcrylicBlurEnabled) && ApiInformationHelper.IsCreatorsUpdateOrLater && !ApiInformationHelper.IsMobileDevice)
            {
                BlurBorder.Background = XAMLHelper.GetResourceValue<CustomAcrylicBrush>("HostBackdropAcrylicBrush");
            }
            else BlurBorder.Background = (Brush)XAMLHelper.GetGenericResourceValue("ApplicationPageBackgroundThemeBrush");
        }

        public async Task ConfigureHamburgerMenuBlur()
        {
            if (ApiInformationHelper.IsCreatorsUpdateOrLater)
            {
                BlurBorderHamburger.Background = XAMLHelper.GetResourceValue<CustomAcrylicBrush>("InAppAcrylicBrush");
            }
            else await BlurBorderHamburger.AttachCompositionBlurEffect(20, 100, true);
        }
        #endregion
    }
}
