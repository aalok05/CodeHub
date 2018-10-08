using CodeHub.Controls;
using CodeHub.Helpers;
using CodeHub.Models;
using CodeHub.Services;
using CodeHub.ViewModels;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Toolkit.Uwp.Helpers;
using Octokit;
using RavinduL.LocalNotifications;
using RavinduL.LocalNotifications.Notifications;
using System;
using System.Threading;
using System.Threading.Tasks;
using UICompositionAnimations;
using UICompositionAnimations.Enums;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Background;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Notifications.Management;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using static CodeHub.Helpers.GlobalHelper;

namespace CodeHub.Views
{
    public sealed partial class MainPage : Windows.UI.Xaml.Controls.Page
    {
        public MainViewmodel ViewModel { get; set; }
        public CustomFrame AppFrame { get { return mainFrame; } }
        private readonly SemaphoreSlim HeaderAnimationSemaphore = new SemaphoreSlim(1);
        private LocalNotificationManager notifManager;

        public MainPage(IActivatedEventArgs args, Type pageType = null, object model = null)
        {
            InitializeComponent();

            var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            coreTitleBar.ExtendViewIntoTitleBar = true;

            coreTitleBar.LayoutMetricsChanged += delegate
            {
                AppTitleBar.Height = coreTitleBar.Height;
            };

            // Set a XAML element as title bar
            Window.Current.SetTitleBar(AppTitleBar);

            ViewModel = new MainViewmodel(args);
            DataContext = ViewModel;

            #region registering for messages
            Messenger.Default.Register<LocalNotificationMessageType>(this, RecieveLocalNotificationMessage);
            Messenger.Default.Register(this, (SetHeaderTextMessageType m) => SetHeadertext(m.PageName));
            Messenger.Default.Register(this, (AdsEnabledMessageType m) => ViewModel.ToggleAdsVisiblity());
            Messenger.Default.Register(this, async (UpdateUnreadNotificationsCountMessageType m) =>
            {
                ViewModel.UpdateUnreadNotificationIndicator(m.Count);
                await AppViewmodel.UnreadNotifications?.ShowToasts();
            });
            Messenger.Default.Register(this, async (ShowWhatsNewPopupMessageType m) => await ShowWhatsNewPopupVisiblity());
            Messenger.Default.Register<User>(this, ViewModel.RecieveSignInMessage);
            #endregion

            notifManager = new LocalNotificationManager(NotificationGrid);

            Loaded += MainPage_Loaded;
            SizeChanged += MainPage_SizeChanged;

            try
            {
                SimpleIoc.Default.Register<IAsyncNavigationService>(() => { return new NavigationService(AppFrame); });
            }
            catch
            {

            }

            SystemNavigationManager.GetForCurrentView().BackRequested += SystemNavigationManager_BackRequested;

            NavigationCacheMode = NavigationCacheMode.Enabled;
        }

        private void MainPage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (Window.Current.Bounds.Width < 1024)
            {
                ViewModel.DisplayMode = SplitViewDisplayMode.Overlay;
                ViewModel.IsPaneOpen = false;
                HamRelative.Background = (AcrylicBrush)Windows.UI.Xaml.Application.Current.Resources["LowOpacityElementAcrylicBrush"];
            }
            else
            {
                ViewModel.DisplayMode = SplitViewDisplayMode.Inline;
                ViewModel.IsPaneOpen = true;
                HamRelative.Background = (AcrylicBrush)Windows.UI.Xaml.Application.Current.Resources["SystemControlChromeHighAcrylicWindowMediumBrush"];
            }
        }

        private async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            await ViewModel.Initialize();

            var startupTask = await StartupTask.GetAsync("CodeHubStartupTask");
            var state = startupTask.State;

            if (startupTask.State == StartupTaskState.Disabled)
            {
                state = await startupTask.RequestEnableAsync();
            }


            // Get the listener
            var listener = UserNotificationListener.Current;

            // And request access to the user's notifications (must be called from UI thread)
            var accessStatus = await listener.RequestAccessAsync();

            switch (accessStatus)
            {
                // This means the user has granted access.
                case UserNotificationListenerAccessStatus.Allowed:

                    // Yay! Proceed as normal
                    BackgroundExecutionManager.RemoveAccess();
                    var backgroundAccessStatus = await BackgroundExecutionManager.RequestAccessAsync();
                    if (backgroundAccessStatus == BackgroundAccessStatus.AlwaysAllowed ||
                        backgroundAccessStatus == BackgroundAccessStatus.AllowedSubjectToSystemPolicy)
                    {
                        RegisterBackgroundTasks();
                    }
                    break;

                // This means the user has denied access.
                // Any further calls to RequestAccessAsync will instantly
                // return Denied. The user must go to the Windows settings
                // and manually allow access.
                case UserNotificationListenerAccessStatus.Denied:

                    // Show UI explaining that listener features will not
                    // work until user allows access.
                    break;

                // This means the user closed the prompt without
                // selecting either allow or deny. Further calls to
                // RequestAccessAsync will show the dialog again.
                case UserNotificationListenerAccessStatus.Unspecified:

                    // Show UI that allows the user to bring up the prompt again
                    break;
            }
            if (SystemInformation.IsAppUpdated && ViewModel.IsLoggedin)
            {
                await ShowWhatsNewPopupVisiblity();
            }
        }

        #region click events
        private async void HamListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            await ViewModel.HamItemClicked(e.ClickedItem as HamItem);
        }
        private async void AccountsButton_Click(object sender, RoutedEventArgs e)
        {
            await ShowAccountsPanel();
        }
        private async void CloseAccountsPanel_Tapped(object sender, RoutedEventArgs e)
        {
            await AccountsPanel.StartCompositionFadeScaleAnimationAsync(1, 0, 1, 1.1f, 150, null, 0, EasingFunctionNames.SineEaseInOut);
            ViewModel.IsAccountsPanelVisible = false;
        }
        private async void DeleteAccount_Click(object sender, RoutedEventArgs e)
        {
            await ViewModel.DeleteAccount(((Button)sender).Tag.ToString());
        }
        #endregion

        #region Messaging
        public void RecieveLocalNotificationMessage(LocalNotificationMessageType notif)
        {
            notifManager.Show(new SimpleNotification
            {
                TimeSpan = TimeSpan.FromSeconds(3),
                Text = notif.Message,
                Glyph = notif.Glyph,
                Action = async () => await new MessageDialog(notif.Message).ShowAsync(),
                Background = GetSolidColorBrush("60B53BFF"),
                Foreground = GetSolidColorBrush("FAFBFCFF")
            },
            LocalNotificationCollisionBehaviour.Replace
            );
        }
        #endregion

        #region other methods        

        private void RegisterBackgroundTasks()
        {
            IBackgroundCondition internetAvailableCondition = new SystemCondition(SystemConditionType.InternetAvailable),
                                 userPresentCondition = new SystemCondition(SystemConditionType.UserPresent),
                                 sessionConnectedCondition = new SystemCondition(SystemConditionType.SessionConnected),
                                 backgroundCostNotHighCondition = new SystemCondition(SystemConditionType.BackgroundWorkCostNotHigh);

            var builder = Helpers.BackgroundTaskHelper.BuildBackgroundTask(
                            "ToastNotificationBackgroundTask",
                            new ToastNotificationActionTrigger(),
                            internetAvailableCondition,
                            userPresentCondition,
                            sessionConnectedCondition
                          );
            builder.IsNetworkRequested = true;
            builder.Register();

            var syncBuilder = Helpers.BackgroundTaskHelper.BuildBackgroundTask(
                        "SyncNotifications",
                        new MaintenanceTrigger(15, false),
                        internetAvailableCondition,
                        userPresentCondition,
                        sessionConnectedCondition
                      );
            syncBuilder.IsNetworkRequested = true;
            syncBuilder.Register();
        }

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

        private async void SystemNavigationManager_BackRequested(object sender, BackRequestedEventArgs e)
        {
            IAsyncNavigationService service = SimpleIoc.Default.GetInstance<IAsyncNavigationService>();
            if (service != null && !e.Handled)
            {
                e.Handled = true;
                await service.GoBackAsync();
            }
        }

        private async Task ShowAccountsPanel()
        {
            AccountsPanel.SetVisualOpacity(0);
            ViewModel.IsAccountsPanelVisible = true;
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
