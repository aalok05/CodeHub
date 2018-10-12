using CodeHub.Helpers;
using CodeHub.Models;
using CodeHub.Services;
using CodeHub.Services.Hilite_me;
using CodeHub.ViewModels;
using CodeHub.Views;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Microsoft.QueryStringDotNET;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.AppService;
using Windows.ApplicationModel.Background;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.ExtendedExecution;
using Windows.Foundation;
using Windows.Foundation.Metadata;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Notifications;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using XamlBrewer.Uwp.Controls;
using static CodeHub.Helpers.GlobalHelper;
using Issue = Octokit.Issue;
using PullRequest = Octokit.PullRequest;
using Repository = Octokit.Repository;


namespace CodeHub
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        //private BackgroundTaskDeferral _SyncDeferral;
        //private BackgroundTaskDeferral _SyncAppDeferral;
        //private BackgroundTaskDeferral _ToastActionDeferral;
        private CancellationTokenSource _TokenSource;
        private ExtendedExecutionSession _ExExecSession;
        private bool _IsTaskRunning;
        private AppServiceConnection _NotificationServiceConnection;
        private AppServiceDeferral _NotificationAppServiceDeferral;
        private BackgroundTaskDeferral _Deferral;

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            InitializeComponent();

            Suspending += OnSuspending;
            UnhandledException += Application_UnhandledException;
            // Theme setup
            RequestedTheme = SettingsService.Get<bool>(SettingsKeys.AppLightThemeEnabled) ? ApplicationTheme.Light : ApplicationTheme.Dark;
            SettingsService.Save(SettingsKeys.HighlightStyleIndex, (int)SyntaxHighlightStyleEnum.Monokai, false);
            SettingsService.Save(SettingsKeys.ShowLineNumbers, true, false);
            SettingsService.Save(SettingsKeys.LoadCommitsInfo, false, false);
            SettingsService.Save(SettingsKeys.IsAdsEnabled, false, false);
            SettingsService.Save(SettingsKeys.IsNotificationCheckEnabled, true, false);
            SettingsService.Save(SettingsKeys.HasUserDonated, false, false);

            AppCenter.Start("ecd96e4c-b301-48f3-b640-166a040f1d86", typeof(Analytics), typeof(Crashes));


            _ExExecSession = new ExtendedExecutionSession();
            _ExExecSession.Revoked += ExExecSession_Revoked;
        }

        private void Application_UnhandledException(
            object sender, Windows.UI.Xaml.UnhandledExceptionEventArgs e)
        {
            e.Handled = true;
            ToastHelper.ShowMessage(e.Message, e.Exception.StackTrace);
        }

        protected override void OnActivated(IActivatedEventArgs args)
        {
            base.OnActivated(args);
            OnLaunchedOrActivated(args);
        }

        protected override async void OnBackgroundActivated(BackgroundActivatedEventArgs args)
        {
            var taskInstance = args.TaskInstance;
            taskInstance.Canceled += TaskInstance_Canceled;
            base.OnBackgroundActivated(args);
            _IsTaskRunning = true;
            _Deferral = taskInstance.GetDeferral();
            var triggerDetails = taskInstance.TriggerDetails;
            var taskName = taskInstance.Task.Name;
            switch (taskName)
            {
                case "AppTrigger":
                    if (triggerDetails is ApplicationTriggerDetails appTriggerDetails)
                    {
                        var appArgs = appTriggerDetails.Arguments;
                        if (!appArgs.TryGetValue("action", out object action))
                        {
                            throw new ArgumentNullException(nameof(action));
                        }
                        if (!appArgs.TryGetValue("what", out object what))
                        {
                            throw new ArgumentNullException(nameof(what));
                        }
                        if (!appArgs.TryGetValue("type", out object type))
                        {
                            throw new ArgumentNullException(nameof(type));
                        }
                        if (appArgs.TryGetValue("location", out object location))
                        {
                            throw new ArgumentNullException(nameof(location));
                        }
                        if (appArgs.TryGetValue("filter", out object filter))
                        {
                            throw new ArgumentNullException(nameof(filter));
                        }
                        if (!appArgs.TryGetValue("sendMessage", out object sendMessage))
                        {
                            throw new ArgumentNullException(nameof(type));
                        }

                        if (!(action is string a) || StringHelper.IsNullOrEmptyOrWhiteSpace(a))
                        {
                            throw new ArgumentException($"'{nameof(action)}' has an invalid value");
                        }

                        if (!(what is string w) || StringHelper.IsNullOrEmptyOrWhiteSpace(w))
                        {
                            throw new ArgumentException($"'{nameof(what)}' has an invalid value");
                        }

                        if (!(type is string t) || StringHelper.IsNullOrEmptyOrWhiteSpace(t))
                        {
                            throw new ArgumentNullException(nameof(type));
                        }

                        if (!(location is string l) || StringHelper.IsNullOrEmptyOrWhiteSpace(l))
                        {
                            throw new ArgumentException($"'{nameof(location)}' has an invalid value");
                        }

                        if (!(filter is string f) || StringHelper.IsNullOrEmptyOrWhiteSpace(f))
                        {
                            throw new ArgumentException($"'{nameof(filter)}' has an invalid value");
                        }

                        if (!(sendMessage is bool sm))
                        {
                            throw new ArgumentException($"'{nameof(sendMessage)}' has an invalid value");
                        }

                        if (a == "sync")
                        {
                            if (w == "notifications")
                            {
                                var notifications = new ObservableCollection<Octokit.Notification>();
                                if (l == "online")
                                {
                                    var filters = f.Split(',');
                                    bool isAll = false, isParticipating = false;
                                    if (filter != null && filters.Length > 0)
                                    {
                                        isAll = filters.Contains("all");
                                        isParticipating = filters.Contains("participating");
                                    }

                                    notifications = await NotificationsService.GetAllNotificationsForCurrentUser(isAll, isParticipating);

                                    if (t == "toast")
                                    {
                                        if (!sm)
                                        {
                                            await notifications.ShowToasts(_Deferral);
                                        }
                                        else
                                        {
                                            await notifications.ShowToasts();
                                            if (isAll)
                                            {
                                                SendMessage(new UpdateAllNotificationsCountMessageType { Count = notifications?.Count ?? 0 }, _Deferral);
                                            }
                                            else if (isParticipating)
                                            {
                                                SendMessage(new UpdateParticipatingNotificationsCountMessageType { Count = notifications?.Count ?? 0 }, _Deferral);
                                            }
                                            else if (!isAll && !isParticipating)
                                            {
                                                SendMessage(new UpdateUnreadNotificationsCountMessageType { Count = notifications?.Count ?? 0 }, _Deferral);
                                            }

                                        }
                                    }

                                    else if (t == "tiles")
                                    {
                                        var tile = await notifications[0].BuildTiles();
                                        TileUpdateManager
                                            .CreateTileUpdaterForApplication()
                                            .Update(tile);
                                    }
                                }
                            }
                        }
                    }
                    break;
                case "SyncNotifications":
                    _ExExecSession.RunActionAsExtentedAction(() =>
                    {
                        ExecutionService.RunActionInUiThread(async () =>
                        {
                            try
                            {
                                AppViewmodel.UnreadNotifications = await NotificationsService.GetAllNotificationsForCurrentUser(false, false);
                                SendMessage(new UpdateUnreadNotificationsCountMessageType { Count = AppViewmodel.UnreadNotifications?.Count ?? 0 });
                                await AppViewmodel.UnreadNotifications?.ShowToasts();

                            }
                            catch (Exception ex)
                            {
                                ToastHelper.ShowMessage(ex.Message, ex.ToString());
                                return;
                            }
                        });
                    }, ExExecSession_Revoked, _Deferral);
                    break;
                case "ToastNotificationAction":
                    if (!(triggerDetails is ToastNotificationActionTriggerDetail toastTriggerDetails))
                    {
                        throw new ArgumentException();
                    }

                    var toastArgs = QueryString.Parse(toastTriggerDetails.Argument);
                    var notificationId = toastArgs["notificationId"];
                    if (StringHelper.IsNullOrEmptyOrWhiteSpace(notificationId))
                    {
                        throw new ArgumentNullException("notificationId");
                    }
                    try
                    {
                        await NotificationsService.MarkNotificationAsRead(notificationId);
                    }
                    catch (Exception ex)
                    {
                        ToastHelper.ShowMessage(ex.Message, ex.ToString());
                    }
                    _ExExecSession.RunActionAsExtentedAction(() =>
                    {
                        ExecutionService.RunActionInUiThread(async () =>
                            {
                                try
                                {
                                    AppViewmodel.UnreadNotifications = await NotificationsService.GetAllNotificationsForCurrentUser(false, false);
                                    SendMessage(new UpdateUnreadNotificationsCountMessageType
                                    {
                                        Count = AppViewmodel.UnreadNotifications?.Count ?? 0
                                    });

                                }
                                catch (Exception ex)
                                {
                                    ToastHelper.ShowMessage(ex.Message, ex.ToString());
                                }
                                finally
                                {
                                    AppViewmodel.UnreadNotifications?.ShowToasts();
                                }
                            });
                    }, ExExecSession_Revoked, _Deferral);
                    break;

                    //case "ToastNotificationChangedTask":
                    //var toastChangedTriggerDetails = taskInstance.TriggerDetails as ToastNotificationHistoryChangedTriggerDetail;
                    //var collectionId = toastChangedTriggerDetails.CollectionId;
                    //var changedType = toastChangedTriggerDetails.ChangeType;
                    //if (changedType == ToastHistoryChangedType.Removed)
                    //{

                    //}
                    //break;
            }
        }

        private void TaskInstance_Canceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            _Deferral?.Complete();

            ToastHelper.ShowMessage($"{sender.Task.Name} has been canceled", reason.ToString());

            //switch (sender.Task.Name)
            //{
            //    case "SyncNotifications":
            //    case "SyncNotificationsApp":
            //        _SyncDeferral?.Complete();
            //        _SyncAppDeferral?.Complete();
            //        break;
            //    case "ToastNotificationBackgroundTask":
            //        _ToastActionDeferral?.Complete();
            //        break;
            //}
        }

        private async void SendMessage<T>(T messageType, BackgroundTaskDeferral deferral = null)
            where T : MessageTypeBase, new()
        {
            try
            {
                if (messageType is UpdateUnreadNotificationsCountMessageType uMsgType)
                {
                    await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        Messenger.Default?.Send(uMsgType);
                    });
                }
            }
            catch (Exception ex)
            {
                ToastHelper.ShowMessage(ex.Message, ex.ToString());
            }
            finally
            {
                if (deferral != null)
                {
                    deferral.Complete();
                }
            }

        }

        /// <summary>
        /// Invoked when the application is  launched normally by the end user. Other entry points will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the         launch request and process.
        /// </param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            base.OnLaunched(e);
            OnLaunchedOrActivated(e);
        }

        private void Activate()
        {
            if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.ApplicationView"))
            {
                var view = ApplicationView.GetForCurrentView();
                view.SetPreferredMinSize(new Size(width: 800, height: 600));

                var titleBar = ApplicationView.GetForCurrentView().TitleBar;
                if (titleBar != null)
                {
                    titleBar.ButtonBackgroundColor = titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;

                    titleBar.ForegroundColor =
                    titleBar.ButtonInactiveForegroundColor =
                    titleBar.ButtonForegroundColor =
                    Colors.White;
                }
            }
        }

        private async void OnLaunchedOrActivated(IActivatedEventArgs args)
        {
#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                DebugSettings.EnableFrameRateCounter = true;
            }

#endif
            // Set the right theme-depending color for the alternating rows
            if (SettingsService.Get<bool>(SettingsKeys.AppLightThemeEnabled))
            {
                XAMLHelper.AssignValueToXAMLResource("OddAlternatingRowsBrush", new SolidColorBrush { Color = Color.FromArgb(0x08, 0, 0, 0) });
            }
            if (args is LaunchActivatedEventArgs launchArgs)
            {
                if (!launchArgs.PrelaunchActivated)
                {
                    if (Window.Current.Content == null)
                    {
                        Window.Current.Content = new MainPage(null);
                        (Window.Current.Content as Page).OpenFromSplashScreen(launchArgs.SplashScreen.ImageLocation);
                    }
                }
                Activate();
                Window.Current.Activate();


                IBackgroundCondition internetAvailableCondition = new SystemCondition(SystemConditionType.InternetAvailable),
                                     userPresentCondition = new SystemCondition(SystemConditionType.UserPresent),
                                     sessionConnectedCondition = new SystemCondition(SystemConditionType.SessionConnected),
                                     backgroundCostNotHighCondition = new SystemCondition(SystemConditionType.BackgroundWorkCostNotHigh);

                var conditions = new[]
                {
                    internetAvailableCondition,
                    //userPresentCondition,
                    //sessionConnectedCondition
                };

                var bgBuilderModel = new BackgroundTaskBuilderModel(
                                     "AppTrigger",
                                     conditions
                                  );

                var builder = BackgroundTaskService.BuildTask(bgBuilderModel, true, true, null);


                builder.Register(BackgroundTaskService.GetAppTrigger(), all: false);
            }
            else if (args is ToastNotificationActivatedEventArgs toastActivatedEventArgs)
            {
                if (args.Kind == ActivationKind.Protocol)
                {
                    if (args.PreviousExecutionState == ApplicationExecutionState.Running)
                    {
                        await HandleProtocolActivationArguments(args);
                    }
                    else
                    {
                        if (Window.Current.Content == null)
                        {
                            Window.Current.Content = new MainPage(args);
                        }
                        Activate();
                    }
                }
                else if (args.Kind == ActivationKind.ToastNotification)
                {
                    if (Window.Current.Content == null)
                    {
                        Window.Current.Content = new MainPage(args);
                    }
                    else
                    {
                        var toastArgs = QueryString.Parse(toastActivatedEventArgs.Argument);

                        var notificationId = toastArgs["notificationId"] ?? throw new ArgumentNullException("notificationId");

                        if (!long.TryParse(toastArgs["repoId"], out long repoId))
                        {
                            await SimpleIoc
                                    .Default
                                    .GetInstance<IAsyncNavigationService>()
                                    .NavigateAsync(typeof(NotificationsView));
                        }

                        string group = null,
                               tag = $"N{notificationId}+R{repoId}";

                        var repo = await RepositoryUtility.GetRepository(repoId);

                        switch (toastArgs["action"])
                        {
                            case "showIssue":

                                if (int.TryParse(toastArgs["issueNumber"], out int issueNumber))
                                {

                                    var issue = await IssueUtility.GetIssue(repo.Id, issueNumber);
                                    await SimpleIoc
                                        .Default
                                        .GetInstance<IAsyncNavigationService>()
                                        .NavigateAsync(typeof(IssueDetailView), new Tuple<Repository, Issue>(repo, issue));
                                    tag += $"+I{issueNumber}";
                                    group = "Issues";
                                }
                                else
                                {
                                    await SimpleIoc
                                        .Default
                                        .GetInstance<IAsyncNavigationService>()
                                        .NavigateAsync(typeof(NotificationsView));
                                }

                                break;

                            case "showPr":

                                if (int.TryParse(toastArgs["prNumber"], out int prNumber))
                                {
                                    var pr = await PullRequestUtility.GetPullRequest(repoId, prNumber);
                                    await SimpleIoc
                                            .Default
                                            .GetInstance<IAsyncNavigationService>()
                                            .NavigateAsync(typeof(PullRequestDetailView), new Tuple<Repository, PullRequest>(repo, pr));
                                    tag += $"+P{pr.Number}";
                                    group = "PullRequests";
                                }
                                else
                                {
                                    await SimpleIoc
                                        .Default
                                        .GetInstance<IAsyncNavigationService>()
                                        .NavigateAsync(typeof(NotificationsView));
                                }
                                break;
                        }
                        if (!StringHelper.IsNullOrEmptyOrWhiteSpace(tag) && !StringHelper.IsNullOrEmptyOrWhiteSpace(group))
                        {
                            ToastNotificationManager.History.Remove(tag, group);
                        }
                        if (!StringHelper.IsNullOrEmptyOrWhiteSpace(notificationId))
                        {
                            await NotificationsService.MarkNotificationAsRead(notificationId);

                        }
                    }

                    Activate();
                    Window.Current.Activate();
                }
            }
            else if (args is StartupTaskActivatedEventArgs startupTaskActivatedEventArgs)
            {
                if (args.Kind == ActivationKind.StartupTask)
                {
                    var payload = ActivationKind.StartupTask.ToString();
                    if (Window.Current.Content == null)
                    {
                        Window.Current.Content = new MainPage(args);
                    }
                (Window.Current.Content as Frame).Navigate(typeof(NotificationsView));
                }
            }
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        private void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            //throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            //_ExExecSession = new ExtendedExecutionSession();
            //_ExExecSession.Revoked += ExExecSession_Revoked;

            //if (_IsTaskRunning)
            //{
            //    await _ExExecSession.RequestExtensionAsync();
            //    while (_IsTaskRunning)
            //    {

            //    }
            //    if (!_IsTaskRunning)
            //    {
            //        _ExExecSession.Dispose();
            //        _ExExecSession = null;
            //        deferral.Complete();
            //    }
            //}

            deferral.Complete();
        }

        private void ExExecSession_Revoked(object sender, ExtendedExecutionRevokedEventArgs args)
        {
            _IsTaskRunning = false;
            _ExExecSession.Dispose();
            _ExExecSession = null;
        }

        private async Task HandleProtocolActivationArguments(IActivatedEventArgs args)
        {
            if (!StringHelper.IsNullOrEmptyOrWhiteSpace(UserLogin))
            {
                ProtocolActivatedEventArgs eventArgs = args as ProtocolActivatedEventArgs;
                var uri = eventArgs.Uri;
                var host = uri.Host;
                switch (host.ToLower())
                {
                    case "repository":
                        await SimpleIoc.Default.GetInstance<IAsyncNavigationService>().NavigateAsync(typeof(RepoDetailView), uri.Segments[1] + uri.Segments[2]);
                        break;

                    case "user":
                        await SimpleIoc.Default.GetInstance<IAsyncNavigationService>().NavigateAsync(typeof(DeveloperProfileView), uri.Segments[1]);
                        break;

                }
            }
        }
    }
}