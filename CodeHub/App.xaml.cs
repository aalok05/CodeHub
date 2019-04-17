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
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Background;
using Windows.ApplicationModel.ExtendedExecution;
using Windows.Foundation;
using Windows.Foundation.Metadata;
using Windows.UI;
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
    public sealed partial class App : Application
    {
        private ExtendedExecutionSession _ExExecSession;
        private BackgroundTaskDeferral _AppTriggerDeferral;
        private BackgroundTaskDeferral _SyncDeferral;
        private BackgroundTaskDeferral _ToastDeferral;
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            InitializeComponent();

            Suspending += OnSuspending;
            //UnhandledException += Application_UnhandledException;

            // Theme setup
            RequestedTheme = SettingsService.Get<bool>(SettingsKeys.AppLightThemeEnabled) ? ApplicationTheme.Light : ApplicationTheme.Dark;
            SettingsService.Save(SettingsKeys.HighlightStyleIndex, (int) SyntaxHighlightStyleEnum.Monokai, false);
            SettingsService.Save(SettingsKeys.ShowLineNumbers, true, false);
            SettingsService.Save(SettingsKeys.LoadCommitsInfo, false, false);
            SettingsService.Save(SettingsKeys.IsAdsEnabled, false, false);
            SettingsService.Save(SettingsKeys.IsNotificationCheckEnabled, true, false);
            SettingsService.Save(SettingsKeys.HasUserDonated, false, false);

            AppCenter.Start("ecd96e4c-b301-48f3-b640-166a040f1d86", typeof(Analytics), typeof(Crashes));

            _ExExecSession = new ExtendedExecutionSession();
            _ExExecSession.Revoked += ExExecSession_Revoked;
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

        //private void Application_UnhandledException(
        //    object sender, Windows.UI.Xaml.UnhandledExceptionEventArgs e)
        //{
        //    e.Handled = true;
        //    ToastHelper.ShowMessage(e.Message, e.Exception.StackTrace);
        //}

        private void ExExecSession_Revoked(object sender, ExtendedExecutionRevokedEventArgs args)
        {
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
            var triggerDetails = taskInstance.TriggerDetails;
            var taskName = taskInstance.Task.Name;
            switch (taskName)
            {
                case "AppTrigger":
                    _AppTriggerDeferral = taskInstance.GetDeferral();
                    if (!(triggerDetails is ApplicationTriggerDetails appTriggerDetails))
                    {
                        throw new InvalidOperationException();
                    }
                    await _ExExecSession.RunActionAsExtentedAction(() =>
                    {
                        ExecutionService.RunActionInUiThread(async () =>
                        {
                            try
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
                                if (!appArgs.TryGetValue("location", out object location))
                                {
                                    throw new ArgumentNullException(nameof(location));
                                }
                                if (!appArgs.TryGetValue("filter", out object filter))
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
                                            bool isAll = false, isParticipating = false, isUnread = true;
                                            if (filter != null && filters.Length > 0)
                                            {
                                                isAll = filters.Contains("all", StringComparer.OrdinalIgnoreCase);
                                                isParticipating = filters.Contains("participating", StringComparer.OrdinalIgnoreCase);
                                                isUnread = filters.Contains("unread", StringComparer.OrdinalIgnoreCase);
                                            }
                                            notifications = await NotificationsService.GetAllNotificationsForCurrentUser(isAll, isParticipating);

                                            if (t == "toast")
                                            {
                                                if (sm)
                                                {
                                                    if (isAll)
                                                    {
                                                        NotificationsViewmodel.AllNotifications = notifications;
                                                        SendMessage(new UpdateAllNotificationsCountMessageType { Count = notifications?.Count ?? 0 });
                                                    }
                                                    else if (isParticipating)
                                                    {
                                                        NotificationsViewmodel.ParticipatingNotifications = notifications;
                                                        SendMessage(new UpdateParticipatingNotificationsCountMessageType { Count = notifications?.Count ?? 0 });
                                                    }
                                                    else if (isUnread)
                                                    {
                                                        AppViewmodel.UnreadNotifications = notifications;
                                                        SendMessage(new UpdateUnreadNotificationsCountMessageType { Count = notifications?.Count ?? 0 });
                                                    }
                                                }
                                                if (SettingsService.Get<bool>(SettingsKeys.IsToastEnabled))
                                                {
                                                    await AppViewmodel.UnreadNotifications?.ShowToasts();
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
                            catch (Exception ex)
                            {
                                ToastHelper.ShowMessage(ex.Message, ex.ToString());
                            }
                        });
                    }, ExExecSession_Revoked, _AppTriggerDeferral);
                    break;
                case "SyncNotifications":
                    _SyncDeferral = taskInstance.GetDeferral();
                    await _ExExecSession.RunActionAsExtentedAction(() =>
                    {
                        ExecutionService.RunActionInUiThread(async () =>
                        {
                            await BackgroundTaskService.LoadUnreadNotifications(true);
                        });
                    }, ExExecSession_Revoked, _SyncDeferral);
                    break;
                case "ToastNotificationAction":
                    _ToastDeferral = taskInstance.GetDeferral();
                    if (!(triggerDetails is ToastNotificationActionTriggerDetail toastTriggerDetails))
                    {
                        throw new ArgumentException();
                    }
                    await _ExExecSession.RunActionAsExtentedAction(() =>
                    {
                        ExecutionService.RunActionInUiThread(async () =>
                        {
                            try
                            {
                                var toastArgs = QueryString.Parse(toastTriggerDetails.Argument);
                                var notificationId = toastArgs["notificationId"] as string;
                                await NotificationsService.MarkNotificationAsRead(notificationId);
                                await BackgroundTaskService.LoadUnreadNotifications(true);
                            }
                            catch (Exception ex)
                            {
                                ToastHelper.ShowMessage(ex.Message, ex.ToString());
                            }

                        });
                    }, ExExecSession_Revoked, _ToastDeferral);
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

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        private void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            ToastHelper.ShowMessage($"Failed to load Page {e.SourcePageType.FullName}", e.Exception.ToString());
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

        private async void OnLaunchedOrActivated(IActivatedEventArgs args)
        {
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
                BackgroundTaskService.RegisterAppTriggerBackgroundTasks();
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
                    var mainPageType = typeof(FeedView);
                    var backPageType = typeof(NotificationsView);
                    if (Window.Current.Content == null)
                    {
                        Window.Current.Content = new MainPage(args);
                    }
                    else
                    {
                        var svc = SimpleIoc
                          .Default
                          .GetInstance<IAsyncNavigationService>();
                        try
                        {
                            var toastArgs = QueryString.Parse(toastActivatedEventArgs.Argument);
                            var notificationId = toastArgs["notificationId"] as string;
                            var repoId = long.Parse(toastArgs["repoId"]);

                            string group = null,
                                   tag = $"N{notificationId}+R{repoId}";

                            var repo = await RepositoryUtility.GetRepository(repoId);

                            switch (toastArgs["action"])
                            {
                                case "showIssue":
                                    var issueNumber = int.Parse(toastArgs["issueNumber"]);

                                    var issue = await IssueUtility.GetIssue(repo.Id, issueNumber);
                                    tag += $"+I{issueNumber}";
                                    group = "Issues";
                                    await svc.NavigateAsync(typeof(IssueDetailView), new Tuple<Repository, Issue>(repo, issue), backPageType: backPageType);

                                    break;

                                case "showPr":
                                    var prNumber = int.Parse(toastArgs["prNumber"]);
                                    var pr = await PullRequestUtility.GetPullRequest(repoId, prNumber);
                                    tag += $"+P{pr.Number}";
                                    group = "PullRequests";
                                    await svc.NavigateAsync(typeof(PullRequestDetailView), new Tuple<Repository, PullRequest>(repo, pr), backPageType: backPageType);

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
                        catch
                        {
                            await svc.NavigateAsync(mainPageType);
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
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();

            deferral.Complete();
        }

        private void SendMessage<T>(T messageType, BackgroundTaskDeferral deferral = null)
            where T : MessageTypeBase, new()
        {
            try
            {
                if (messageType is UpdateUnreadNotificationsCountMessageType uMsgType)
                {
                    ExecutionService.RunActionInCoreWindow(() =>
                    {
                        Messenger.Default?.Send(uMsgType);
                    });
                }
                if (messageType is UpdateParticipatingNotificationsCountMessageType pMsgType)
                {
                    ExecutionService.RunActionInCoreWindow(() =>
                    {
                        Messenger.Default?.Send(pMsgType);
                    });
                }
                if (messageType is UpdateAllNotificationsCountMessageType aMsgType)
                {
                    ExecutionService.RunActionInCoreWindow(() =>
                    {
                        Messenger.Default?.Send(aMsgType);
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

        private void TaskInstance_Canceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            _AppTriggerDeferral?.Complete();
            _SyncDeferral?.Complete();
            _ToastDeferral?.Complete();
            _ExExecSession.Revoked -= ExExecSession_Revoked;
            _ExExecSession.Dispose();
            _ExExecSession = null;
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
    }
}