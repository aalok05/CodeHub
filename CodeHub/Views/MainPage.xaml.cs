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
using Windows.ApplicationModel.Activation;

namespace CodeHub.Views
{
    public sealed partial class MainPage : Windows.UI.Xaml.Controls.Page
    {
        public MainViewmodel ViewModel { get; set; }
        public CustomFrame AppFrame { get { return mainFrame; } }
        private readonly SemaphoreSlim HeaderAnimationSemaphore = new SemaphoreSlim(1);
        private LocalNotificationManager notifManager;

        public MainPage(IActivatedEventArgs args)
        {
            this.InitializeComponent();

            ViewModel = new MainViewmodel(args);
            this.DataContext = ViewModel;

            #region registering for messages
            Messenger.Default.Register<LocalNotificationMessageType>(this, RecieveLocalNotificationMessage);
            Messenger.Default.Register(this, delegate(SetHeaderTextMessageType m) {  SetHeadertext(m.PageName); });
            Messenger.Default.Register(this, delegate (AdsEnabledMessageType m) { ViewModel.ToggleAdsVisiblity(); });
            Messenger.Default.Register(this, delegate (HostWindowBlurMessageType m) { ConfigureWindowBlur(); });
            Messenger.Default.Register(this, delegate (UpdateUnreadNotificationMessageType m) { ViewModel.UpdateUnreadNotificationIndicator(m.IsUnread); });
            Messenger.Default.Register(this, async delegate (ShowWhatsNewPopupMessageType m) {await ShowWhatsNewPopupVisiblity(); });
            Messenger.Default.Register<User>(this, ViewModel.RecieveSignInMessage);
            #endregion

            notifManager = new LocalNotificationManager(NotificationGrid);

            Loaded += MainPage_Loaded;
            SizeChanged += MainPage_SizeChanged;

            SimpleIoc.Default.Register<IAsyncNavigationService>(() => { return new NavigationService(AppFrame); });
            SystemNavigationManager.GetForCurrentView().BackRequested += SystemNavigationManager_BackRequested;

            NavigationCacheMode = NavigationCacheMode.Enabled;
        }

        private void MainPage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (Window.Current.Bounds.Width < 1024)
            {
                ViewModel.DisplayMode = SplitViewDisplayMode.Overlay;
                ViewModel.IsPaneOpen = false;

                if (ApiInformationHelper.IsCreatorsUpdateOrLater)
                {
                    BlurBorderHamburger.Background = XAMLHelper.GetResourceValue<CustomAcrylicBrush>("InAppAcrylicBrush");
                }
            }
            else
            {
                ViewModel.DisplayMode = SplitViewDisplayMode.Inline;
                ViewModel.IsPaneOpen = true;

                if (ApiInformationHelper.IsCreatorsUpdateOrLater && !ApiInformationHelper.IsMobileDevice)
                {
                    BlurBorderHamburger.Background = XAMLHelper.GetResourceValue<CustomAcrylicBrush>("HamburgerBackdropAcrylicBrush");
                }
            }
        }

        private async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            ConfigureWindowBlur();
            await ConfigureHamburgerMenuBlur();

            await ViewModel.Initialize();

            if (WhatsNewDisplayService.IsNewVersion() && ViewModel.isLoggedin)
                await ShowWhatsNewPopupVisiblity();
        }

        #region click events
        private void HamListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            ViewModel.HamItemClicked(e.ClickedItem as HamItem);
        }
        private async void AccountsButton_Click(object sender, RoutedEventArgs e)
        {
            await ShowAccountsPanel();
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
                await HeaderText.StartCompositionFadeSlideAnimationAsync(1, 0, TranslationAxis.Y, 0, -24, 150, null, null, EasingFunctionNames.Linear);
                ViewModel.HeaderText = pageName.ToUpper();
                await HeaderText.StartCompositionFadeSlideAnimationAsync(0, 1, TranslationAxis.Y, 24, 0, 150, null, null, EasingFunctionNames.Linear);
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
                if (!ApiInformationHelper.IsMobileDevice && HamSplitView.DisplayMode == SplitViewDisplayMode.Inline)
                {
                    BlurBorderHamburger.Background = XAMLHelper.GetResourceValue<CustomAcrylicBrush>("HamburgerBackdropAcrylicBrush");
                }
                else
                {
                    BlurBorderHamburger.Background = XAMLHelper.GetResourceValue<CustomAcrylicBrush>("InAppAcrylicBrush");
                }
            }
            else await BlurBorderHamburger.AttachCompositionBlurEffect(20, 100, true);
        }

        private void SystemNavigationManager_BackRequested(object sender, BackRequestedEventArgs e)
        {
            IAsyncNavigationService service = SimpleIoc.Default.GetInstance<IAsyncNavigationService>();
            if (service != null && !e.Handled)
            {
                e.Handled = true;
                service.GoBackAsync();
            }
        }
        private async Task ShowAccountsPanel()
        {
            AccountsPanel.SetVisualOpacity(0);
            AccountsPanel.Visibility = Visibility.Visible;
            await AccountsPanel.StartCompositionFadeScaleAnimationAsync(0, 1, 1.1f, 1, 150, null, 0, EasingFunctionNames.SineEaseInOut);
        }
        private async Task ShowWhatsNewPopupVisiblity()
        {
            WhatsNewPopup.SetVisualOpacity(0);
            WhatsNewPopup.Visibility = Visibility.Visible;
            await WhatsNewPopup.StartCompositionFadeScaleAnimationAsync(0, 1, 1.3f, 1, 160, null, 0, EasingFunctionNames.SineEaseInOut);
        }
        #endregion
    }
}
