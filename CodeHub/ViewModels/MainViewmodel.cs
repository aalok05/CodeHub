using CodeHub.Helpers;
using CodeHub.Models;
using CodeHub.Services;
using CodeHub.Views;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.QueryStringDotNET;
using Octokit;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Notifications;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using static CodeHub.Helpers.GlobalHelper;

namespace CodeHub.ViewModels
{
    public class MainViewmodel : AppViewmodel
    {
        #region properties
        public IActivatedEventArgs activatedEventArgs;

        private string _headerText;
        public string HeaderText
        {
            get => _headerText;
            set => Set(() => HeaderText, ref _headerText, value);
        }

        private ObservableCollection<HamItem> _HamItems = new ObservableCollection<HamItem>();
        public ObservableCollection<HamItem> HamItems
        {

            get => _HamItems;
            set => Set(() => HamItems, ref _HamItems, value);
        }

        private ObservableCollection<Models.Account> _InactiveAccounts = new ObservableCollection<Models.Account>();
        public ObservableCollection<Models.Account> InactiveAccounts
        {

            get => _InactiveAccounts;
            set => Set(() => InactiveAccounts, ref _InactiveAccounts, value);
        }
        private Models.Account _activeAccount = new Models.Account();
        public Models.Account ActiveAccount
        {

            get => _activeAccount;
            set => Set(() => ActiveAccount, ref _activeAccount, value);
        }
        private bool _isPaneOpen;
        public bool IsPaneOpen
        {
            get => _isPaneOpen;
            set => Set(() => IsPaneOpen, ref _isPaneOpen, value);
        }
        private bool _isAccountsPanelVisible;
        public bool IsAccountsPanelVisible
        {
            get => _isAccountsPanelVisible;
            set => Set(() => IsAccountsPanelVisible, ref _isAccountsPanelVisible, value);
        }

        private SplitViewDisplayMode _displayMode = SplitViewDisplayMode.Overlay;
        public SplitViewDisplayMode DisplayMode
        {
            get => _displayMode;

            set => Set(() => DisplayMode, ref _displayMode, value);
        }

        #endregion

        public MainViewmodel(IActivatedEventArgs args)
        {
            activatedEventArgs = args;
            var languageLoader = new Windows.ApplicationModel.Resources.ResourceLoader();

            var trendingSymbol = "<Geometry xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\">F0 M12,16z M0,0z M5.05,0.31C5.86,2.48 5.46,3.69 4.53,4.62 3.55,5.67 1.98,6.45 0.9,7.98 -0.55,10.03 -0.8,14.51 4.43,15.68 2.23,14.52 1.76,11.16 4.13,9.07 3.52,11.1 4.66,12.4 6.07,11.93 7.46,11.46 8.37,12.46 8.34,13.6 8.32,14.38 8.03,15.04 7.21,15.41 10.63,14.82 11.99,11.99 11.99,9.85 11.99,7.01 9.46,6.63 10.74,4.24 9.22,4.37 8.71,5.37 8.85,6.99 8.94,8.07 7.83,8.79 6.99,8.32 6.32,7.91 6.33,7.13 6.93,6.54 8.18,5.31 8.68,2.45 5.05,0.32L5.03,0.3 5.05,0.31z</Geometry>";
            var profileSymbol = "<Geometry xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\">F0 M12,16z M0,0z M12,14.002A0.998,0.998,0,0,1,11.002,15L1.001,15A1,1,0,0,1,0,13.999L0,13C0,10.367 4,9 4,9 4,9 4.229,8.591 4,8 3.159,7.38 3.056,6.41 3,4 3.173,1.587 4.867,1 6,1 7.133,1 8.827,1.586 9,4 8.944,6.41 8.841,7.38 8,8 7.771,8.59 8,9 8,9 8,9 12,10.367 12,13L12,14.002z</Geometry>";
            var myRepoSymbol = "<Geometry xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\">F0 M12,16z M0,0z M4,9L3,9 3,8 4,8 4,9z M4,6L3,6 3,7 4,7 4,6z M4,4L3,4 3,5 4,5 4,4z M4,2L3,2 3,3 4,3 4,2z M12,1L12,13C12,13.55,11.55,14,11,14L6,14 6,16 4.5,14.5 3,16 3,14 1,14C0.45,14,0,13.55,0,13L0,1C0,0.45,0.45,0,1,0L11,0C11.55,0,12,0.45,12,1z M11,11L1,11 1,13 3,13 3,12 6,12 6,13 11,13 11,11z M11,1L2,1 2,10 11,10 11,1z</Geometry>";
            var organizationsSymbol = "<Geometry xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\">M16 12.999c0 .439-.45 1-1 1H7.995c-.539 0-.994-.447-.995-.999H1c-.54 0-1-.561-1-1 0-2.634 3-4 3-4s.229-.409 0-1c-.841-.621-1.058-.59-1-3 .058-2.419 1.367-3 2.5-3s2.442.58 2.5 3c.058 2.41-.159 2.379-1 3-.229.59 0 1 0 1s1.549.711 2.42 2.088C9.196 9.369 10 8.999 10 8.999s.229-.409 0-1c-.841-.62-1.058-.59-1-3 .058-2.419 1.367-3 2.5-3s2.437.581 2.495 3c.059 2.41-.158 2.38-1 3-.229.59 0 1 0 1s3.005 1.366 3.005 4</Geometry>";
            var feedSymbol = "<Geometry xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\">M7 1C3.14 1 0 4.14 0 8s3.14 7 7 7c.48 0 .94-.05 1.38-.14-.17-.08-.2-.73-.02-1.09.19-.41.81-1.45.2-1.8-.61-.35-.44-.5-.81-.91-.37-.41-.22-.47-.25-.58-.08-.34.36-.89.39-.94.02-.06.02-.27 0-.33 0-.08-.27-.22-.34-.23-.06 0-.11.11-.2.13-.09.02-.5-.25-.59-.33-.09-.08-.14-.23-.27-.34-.13-.13-.14-.03-.33-.11s-.8-.31-1.28-.48c-.48-.19-.52-.47-.52-.66-.02-.2-.3-.47-.42-.67-.14-.2-.16-.47-.2-.41-.04.06.25.78.2.81-.05.02-.16-.2-.3-.38-.14-.19.14-.09-.3-.95s.14-1.3.17-1.75c.03-.45.38.17.19-.13-.19-.3 0-.89-.14-1.11-.13-.22-.88.25-.88.25.02-.22.69-.58 1.16-.92.47-.34.78-.06 1.16.05.39.13.41.09.28-.05-.13-.13.06-.17.36-.13.28.05.38.41.83.36.47-.03.05.09.11.22s-.06.11-.38.3c-.3.2.02.22.55.61s.38-.25.31-.55c-.07-.3.39-.06.39-.06.33.22.27.02.5.08.23.06.91.64.91.64-.83.44-.31.48-.17.59.14.11-.28.3-.28.3-.17-.17-.19.02-.3.08-.11.06-.02.22-.02.22-.56.09-.44.69-.42.83 0 .14-.38.36-.47.58-.09.2.25.64.06.66-.19.03-.34-.66-1.31-.41-.3.08-.94.41-.59 1.08.36.69.92-.19 1.11-.09.19.1-.06.53-.02.55.04.02.53.02.56.61.03.59.77.53.92.55.17 0 .7-.44.77-.45.06-.03.38-.28 1.03.09.66.36.98.31 1.2.47.22.16.08.47.28.58.2.11 1.06-.03 1.28.31.22.34-.88 2.09-1.22 2.28-.34.19-.48.64-.84.92s-.81.64-1.27.91c-.41.23-.47.66-.66.8 3.14-.7 5.48-3.5 5.48-6.84 0-3.86-3.14-7-7-7L7 1zm1.64 6.56c-.09.03-.28.22-.78-.08-.48-.3-.81-.23-.86-.28 0 0-.05-.11.17-.14.44-.05.98.41 1.11.41.13 0 .19-.13.41-.05.22.08.05.13-.05.14zM6.34 1.7c-.05-.03.03-.08.09-.14.03-.03.02-.11.05-.14.11-.11.61-.25.52.03-.11.27-.58.3-.66.25zm1.23.89c-.19-.02-.58-.05-.52-.14.3-.28-.09-.38-.34-.38-.25-.02-.34-.16-.22-.19.12-.03.61.02.7.08.08.06.52.25.55.38.02.13 0 .25-.17.25zm1.47-.05c-.14.09-.83-.41-.95-.52-.56-.48-.89-.31-1-.41-.11-.1-.08-.19.11-.34.19-.15.69.06 1 .09.3.03.66.27.66.55.02.25.33.5.19.63h-.01z</Geometry>";

            HamItems = new ObservableCollection<HamItem>
            {
                 new HamItem()
                 {
                    Label    = languageLoader.GetString("pageTitle_FeedView"),
                    Symbol   = (Geometry)XamlReader.Load(feedSymbol),
                    DestPage = typeof(FeedView)
                 },
                 new HamItem()
                 {
                    Label    = languageLoader.GetString("pageTitle_TrendingView"),
                    Symbol   = (Geometry)XamlReader.Load(trendingSymbol),
                    DestPage = typeof(TrendingView)
                 },
                 new HamItem()
                 {
                    Label    = languageLoader.GetString("pageTitle_DeveloperProfileView"),
                    Symbol   = (Geometry)XamlReader.Load(profileSymbol),
                    DestPage = typeof(DeveloperProfileView)
                 },
                 new HamItem()
                 {
                    Label    = languageLoader.GetString("pageTitle_MyReposView"),
                    Symbol   = (Geometry)XamlReader.Load(myRepoSymbol),
                    DestPage = typeof(MyReposView)
                 },
                 new HamItem()
                 {
                    Label    = languageLoader.GetString("pageTitle_MyOrganizationsView"),
                    Symbol   = (Geometry)XamlReader.Load(organizationsSymbol),
                    DestPage = typeof(MyOrganizationsView)
                 }
            };

            HamItems[0].IsSelected = true;
        }

        #region commands

        private RelayCommand _openPaneCommand;
        public RelayCommand OpenPaneCommand
            => _openPaneCommand
            ?? (_openPaneCommand = new RelayCommand(() =>
                                             {
                                                 IsPaneOpen = !IsPaneOpen;
                                             }));

        private RelayCommand _signInCommand;
        public RelayCommand SignInCommand
            => _signInCommand
            ?? (_signInCommand = new RelayCommand(async () =>
                                             {
                                                 var service = new AuthService();
                                                 IsLoading = true;

                                                 if (await service.Authenticate())
                                                 {

                                                     var user = await UserService.GetCurrentUserInfo();
                                                     LoadUser(user);
                                                     await InitializeAccounts();
                                                 }
                                                 IsAccountsPanelVisible = false;
                                                 IsLoading = false;

                                             }));

        private RelayCommand _signOutCommand;
        public RelayCommand SignOutCommand
            => _signOutCommand
            ?? (_signOutCommand = new RelayCommand(async () =>
                                             {
                                                 IsLoading = true;

                                                 if (await AuthService.SignOut(ActiveAccount.Id.ToString()))
                                                 {
                                                     User = null;
                                                     await HamItemClicked(HamItems[0]);

                                                     InactiveAccounts = await AccountsService.GetAllUsers();
                                                     if (InactiveAccounts != null && InactiveAccounts.Count > 0)
                                                     {
                                                         var availableAccounts = InactiveAccounts.Where(x => x.Id != ActiveAccount.Id);
                                                         if (availableAccounts.Count() > 0)
                                                         {
                                                             ActiveAccount = availableAccounts.First();
                                                             await AccountsService.MakeAccountActive(ActiveAccount.Id.ToString());
                                                             await InitializeAccounts();
                                                         }
                                                         else
                                                         {
                                                             ActiveAccount = null;
                                                             IsLoggedin = false;
                                                             Messenger.Default.Send(new SignOutMessageType());
                                                             UserLogin = string.Empty;
                                                         }
                                                     }
                                                     else
                                                     {
                                                         ActiveAccount = null;
                                                         IsLoggedin = false;
                                                         Messenger.Default.Send(new SignOutMessageType());
                                                         UserLogin = string.Empty;
                                                     }
                                                 }

                                                 SimpleIoc.Default.GetInstance<IAsyncNavigationService>().ClearBackStack();
                                                 IsAccountsPanelVisible = false;
                                                 IsLoading = false;

                                             }));
        #endregion

        public async Task Initialize()
        {
            var adstask = CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(
                CoreDispatcherPriority.Normal,
                async () =>
                {
                    await ConfigureAdsVisibility();
                });

            await InitializeAccounts();

            if (IsLoggedin)
            {
                if (activatedEventArgs is ProtocolActivatedEventArgs protocolEventArgs)
                {
                    //var eventArgs = activatedEventArgs as ProtocolActivatedEventArgs;

                    switch (protocolEventArgs.Uri.Host.ToLower())
                    {
                        case "repository":
                            await SimpleIoc
                                .Default
                                .GetInstance<IAsyncNavigationService>()
                                .NavigateAsync(typeof(RepoDetailView), protocolEventArgs.Uri.Segments[1] + protocolEventArgs.Uri.Segments[2]);
                            break;

                        case "user":
                            await SimpleIoc
                                    .Default
                                    .GetInstance<IAsyncNavigationService>()
                                    .NavigateAsync(typeof(DeveloperProfileView), protocolEventArgs.Uri.Segments[1]);
                            break;

                    }
                }
                else if (activatedEventArgs is ToastNotificationActivatedEventArgs toastEventArgs)
                {
                    var eventArgs = activatedEventArgs as ProtocolActivatedEventArgs;
                    var args = QueryString.Parse(toastEventArgs.Argument);
                    if (long.TryParse(args["repoId"], out long repoId))
                    {
                        var repo = await RepositoryUtility.GetRepository(repoId);
                        string notificationId = args["notificationId"],
                               toastNotificationTag = $"N{notificationId}+R{repoId}",
                               toastNotificationGroup = null;

                        switch (args["action"])
                        {
                            case "showIssue":
                                toastNotificationGroup = "Issues";
                                if (int.TryParse(args["issueNumber"], out int issueNumber))
                                {
                                    var issue = await IssueUtility.GetIssue(repoId, issueNumber);
                                    await SimpleIoc
                                        .Default
                                        .GetInstance<IAsyncNavigationService>()
                                        .NavigateAsync(typeof(IssueDetailView), new Tuple<Repository, Issue>(repo, issue));
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
                                toastNotificationGroup = "PullRequests";
                                if (int.TryParse(args["prNumber"], out int prNumber))
                                {
                                    var pr = await PullRequestUtility.GetPullRequest(repoId, prNumber);
                                    await SimpleIoc
                                            .Default
                                            .GetInstance<IAsyncNavigationService>()
                                            .NavigateAsync(typeof(PullRequestDetailView), new Tuple<Repository, PullRequest>(repo, pr));
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

                        ToastNotificationManager.History.Remove(toastNotificationTag, toastNotificationGroup);
                    }
                    else
                    {
                        await SimpleIoc.Default.GetInstance<IAsyncNavigationService>().NavigateAsync(typeof(NotificationsView));
                    }
                }
                else
                {
                    await SimpleIoc.Default.GetInstance<IAsyncNavigationService>().NavigateAsync(typeof(FeedView));
                }

                if (IsInternet())
                {
                    //await AppTrigger?.RequestAsync();
                    UnreadNotifications = new ObservableCollection<Octokit.Notification>((await NotificationsService.GetAllNotificationsForCurrentUser(false, false)).OrderByDescending(un => un.UpdatedAt));
                    Messenger.Default?.Send(new UpdateUnreadNotificationsCountMessageType { Count = UnreadNotifications?.Count ?? 0 });
                    await UnreadNotifications?.ShowToasts();
                }
            }
        }

        public async Task InitializeAccounts()
        {
            InactiveAccounts = await AccountsService.GetAllUsers();

            if (InactiveAccounts != null && InactiveAccounts.Count > 0)
            {
                var activeAccounts = InactiveAccounts.Where(x => x.IsActive == true);
                if (activeAccounts.Count() != 0)
                {
                    ActiveAccount = activeAccounts.First();

                    InactiveAccounts.Remove(ActiveAccount);
                    IsLoggedin = AuthService.CheckAuth(ActiveAccount.Id.ToString());
                    await Load(ActiveAccount.Id.ToString());
                }
                else
                {
                    IsLoggedin = false;
                }
            }
            else
            {
                IsLoggedin = false;
            }
        }

        public async Task Load(string userId)
        {
            GithubClient = UserService.GetAuthenticatedClient(AuthService.GetToken(userId));

            if (IsInternet())
            {
                if (IsLoggedin)
                {
                    var user = await UserService.GetCurrentUserInfo();
                    LoadUser(user);
                }
            }
        }

        public void LoadUser(User user)
        {
            if (user != null)
            {
                UserLogin = user.Login;
                IsLoggedin = true;
                Messenger.Default.Send(user);
                User = user;
            }
        }

        public async Task DeleteAccount(string userId)
        {
            IsLoading = true;
            await AccountsService.RemoveUser(userId);
            await InitializeAccounts();
            IsLoading = false;
        }

        public async Task HamItemClicked(HamItem item)
        {
            foreach (var i in HamItems)
            {
                i.IsSelected = false;
            }
            item.IsSelected = true;
            await Navigate(item.DestPage);

            if (!(DisplayMode == SplitViewDisplayMode.Inline))
            {
                IsPaneOpen = false;
            }
        }

        public async void SwitchUser_Click(object sender, ItemClickEventArgs e)
        {
            await AccountsService.MakeAccountActive((e.ClickedItem as Models.Account).Id.ToString());
            await InitializeAccounts();
            IsAccountsPanelVisible = false;
        }

        public void MainFrame_Navigated(object sender, NavigationEventArgs e)
        {
            foreach (var i in HamItems)
            {
                if (e.SourcePageType == i.DestPage)
                {
                    i.IsSelected = true;
                }
                else
                {
                    i.IsSelected = false;
                }
            }
        }

        public async void AppBarNewsFeed_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (SimpleIoc.Default.GetInstance<IAsyncNavigationService>().CurrentSourcePageType != HamItems[0].DestPage)
            {
                await HamItemClicked(HamItems[0]);
            }
        }

        public async void AppBarTrending_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (SimpleIoc.Default.GetInstance<IAsyncNavigationService>().CurrentSourcePageType != HamItems[1].DestPage)
            {
                await HamItemClicked(HamItems[1]);
            }
        }

        public async void AppBarProfile_Tapped(object sender, TappedRoutedEventArgs e)
        {
            await HamItemClicked(HamItems[2]);
        }

        public async void AppBarMyRepos_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (SimpleIoc.Default.GetInstance<IAsyncNavigationService>().CurrentSourcePageType != HamItems[3].DestPage)
            {
                await HamItemClicked(HamItems[3]);
            }
        }

        public async void AppBarMyOrganizations_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (SimpleIoc.Default.GetInstance<IAsyncNavigationService>().CurrentSourcePageType != HamItems[4].DestPage)
            {
                await HamItemClicked(HamItems[4]);
            }
        }

        public async void NavigateToSettings()
        {
            foreach (var i in HamItems)
            {
                i.IsSelected = false;
            }
            await Navigate(typeof(SettingsView));
            if (!(DisplayMode == SplitViewDisplayMode.Inline))
            {
                IsPaneOpen = false;
            }
        }

        public async void NavigateToSearch()
        {
            if (SimpleIoc.Default.GetInstance<IAsyncNavigationService>().CurrentSourcePageType != typeof(SearchView))
            {
                foreach (var i in HamItems)
                {
                    i.IsSelected = false;
                }
                await Navigate(typeof(SearchView));
            }
        }

        public async void NavigateToNotifications()
        {
            if (SimpleIoc.Default.GetInstance<IAsyncNavigationService>().CurrentSourcePageType != typeof(NotificationsView))
            {
                foreach (var i in HamItems)
                {
                    i.IsSelected = false;
                }
                await Navigate(typeof(NotificationsView));
            }
        }

        public async void RecieveSignInMessage(User user)
        {
            if (SimpleIoc.Default.GetInstance<IAsyncNavigationService>().CurrentSourcePageType != typeof(FeedView))
            {
                await SimpleIoc.Default.GetInstance<IAsyncNavigationService>().NavigateAsync(typeof(FeedView));
                SimpleIoc.Default.GetInstance<IAsyncNavigationService>().ClearBackStack();
            }
        }
    }
}
