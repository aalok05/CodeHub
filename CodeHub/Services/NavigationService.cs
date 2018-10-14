using CodeHub.Controls;
using CodeHub.Helpers;
using CodeHub.Views;
using CodeHub.Views.Settings;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml.Navigation;

namespace CodeHub.Services
{
    /// <summary>
    /// A navigation service that implements the IAsyncNavigationService interface
    /// </summary>
    public class NavigationService : IAsyncNavigationService
    {
        public Type CurrentSourcePageType { get; private set; }

        /// <summary>
        /// Gets the frame instance to use when navigating
        /// </summary>
        private readonly CustomFrame Frame;

        /// <summary>
        /// Gets the internal semaphore to synchronize the navigation
        /// </summary>
        private readonly SemaphoreSlim NavigationSemaphore = new SemaphoreSlim(1);

        public NavigationService(CustomFrame frame)
        {
            Frame = frame;
            Frame.Navigated += OnFrameNavigated;
        }

        // Refreshes the navigation back button
        private async void OnFrameNavigated(object sender, NavigationEventArgs navigationEventArgs)
        {
            CurrentSourcePageType = Frame.CurrentSourcePageType;
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = await CanGoBackAsync()
                ? AppViewBackButtonVisibility.Visible : AppViewBackButtonVisibility.Collapsed;
        }

        // Navigates to a target page
        private async Task<bool> NavigateCoreAsync(Type type, string pageTitle, object parameter)
        {
            await NavigationSemaphore.WaitAsync();
            bool result;

            Messenger.Default.Send(new GlobalHelper.SetHeaderTextMessageType { PageName = pageTitle });
            result = await Frame.Navigate(type, parameter);

            GlobalHelper.NavigationStack.Push(pageTitle);

            NavigationSemaphore.Release();

            if (!GlobalHelper.IsInternet())
            {
                //Sending NoInternet message to all viewModels
                Messenger.Default.Send(new GlobalHelper.NoInternet().SendMessage());
            }

            return result;
        }

        /// <summary>
        /// Navigates to the target page with a given parameter
        /// </summary>
        /// <param name="pageType">The type of the target page</param>
        /// <param name="parameter">The navigation parameter</param>
        /// <param name="title">The desired title of the target page.</param>
        /// <param name="backPageType">The optional type of the back page</param>
        /// <param name="backPageParameter">The optional parameter for the back page</param>
        /// <param name="backPageTitle">The desired but optional title of the back page.</param>
        /// <param name="shouldClearBackStack">Set it to 'False' if you don't want to clear the back stack.</param>
        public Task<bool> NavigateAsync(Type pageType, object parameter = null, string pageTitle = null, Type backPageType = null, object backPageParameter = null, string backPageTitle = null, bool shouldClearBackStack = true)
        {
            pageTitle = !StringHelper.IsNullOrEmptyOrWhiteSpace(pageTitle)
                      ? pageTitle
                      : ChoosePageTitleByPageType(pageType);

            if (backPageType != null)
            {
                if (shouldClearBackStack)
                {
                    ClearBackStack();
                }
                if (CurrentSourcePageType != backPageType)
                {
                    AddToBackStack(backPageType, backPageParameter, backPageTitle);
                }
            }

            return CurrentSourcePageType == pageType 
                                          ? Task.FromResult(false) 
                                          : NavigateCoreAsync(pageType, pageTitle, parameter);
        }

        // Navigation with parameters without animations
        public async void NavigateWithoutAnimations(Type type, string pageTitle, object parameter)
        {
            await NavigationSemaphore.WaitAsync();

            Frame.NavigateWithoutAnimations(type, parameter);
            GlobalHelper.NavigationStack.Push(pageTitle);
            NavigationSemaphore.Release();
        }

        // Straight, synchronous navigation without animations
        public async void NavigateWithoutAnimations(Type type, string pageTitle)
        {
            await NavigationSemaphore.WaitAsync();

            Frame.NavigateWithoutAnimations(type);
            GlobalHelper.NavigationStack.Push(pageTitle);
            NavigationSemaphore.Release();
        }

        // Checks if the app can navigate back
        public async Task<bool> CanGoBackAsync()
        {
            await NavigationSemaphore.WaitAsync();
            bool check = Frame.CanGoBack;
            NavigationSemaphore.Release();
            return check;
        }

        // Tries to navigate back
        public async Task<bool> GoBackAsync()
        {
            await NavigationSemaphore.WaitAsync();
            bool result;
            if (Frame.CanGoBack)
            {
                GlobalHelper.NavigationStack.Pop();
                Messenger.Default.Send(new GlobalHelper.SetHeaderTextMessageType { PageName = GlobalHelper.NavigationStack.Peek() });
                await Frame.GoBack();
                result = true;
            }
            else
            {
                result = false;
            }

            NavigationSemaphore.Release();
            return result;
        }

        /// <summary>
        /// Adds page to the navigation history of the frame
        /// </summary>
        /// <param name="pageType">The optional type of the back page</param>
        /// <param name="parameter">The optional parameter for the back page</param>
        /// <param name="title">The desired but optional title of the back page.</param>
        public void AddToBackStack(Type pageType, object parameter = null, string title = null)
        {
            Frame.BackStack.Add(new PageStackEntry(pageType, parameter, null));

            title = !StringHelper.IsNullOrEmptyOrWhiteSpace(title)
                  ? title
                  : ChoosePageTitleByPageType(pageType);

            GlobalHelper.NavigationStack.Push(title);
        }

        /// <summary>
        /// Clears the navigation history of the frame
        /// </summary>
        public void ClearBackStack()
            => Frame.BackStack.Clear();

        /// <summary>
        /// Search for the Page Title with the given Menu type
        /// </summary>
        /// <param name="type">type of the Menu</param>
        /// <returns>string</returns>
        /// <exception cref="Exception">When the given type don't have a Page Title pair</exception> 
        public string ChoosePageTitleByPageType(Type type)
        {
            var languageLoader = new Windows.ApplicationModel.Resources.ResourceLoader();

            if (type == typeof(CommentView))
            {
                return languageLoader.GetString("pageTitle_CommentView");
            }
            else if (type == typeof(DeveloperProfileView))
            {
                return languageLoader.GetString("pageTitle_DeveloperProfileView");
            }
            else if (type == typeof(FeedView))
            {
                return languageLoader.GetString("pageTitle_FeedView");
            }
            else if (type == typeof(IssueDetailView))
            {
                return languageLoader.GetString("pageTitle_IssueDetailView");
            }
            else if (type == typeof(IssuesView))
            {
                return languageLoader.GetString("pageTitle_IssuesView");
            }
            else if (type == typeof(MyOrganizationsView))
            {
                return languageLoader.GetString("pageTitle_MyOrganizationsView");
            }
            else if (type == typeof(MyReposView))
            {
                return languageLoader.GetString("pageTitle_MyReposView");
            }
            else if (type == typeof(NotificationsView))
            {
                return languageLoader.GetString("pageTitle_NotificationsView");
            }
            else if (type == typeof(PullRequestDetailView))
            {
                return languageLoader.GetString("pageTitle_PullRequestDetailView");
            }
            else if (type == typeof(PullRequestsView))
            {
                return languageLoader.GetString("pageTitle_PullRequestsView");
            }
            else if (type == typeof(RepoDetailView))
            {
                return languageLoader.GetString("pageTitle_RepoDetailView");
            }
            else if (type == typeof(SearchView))
            {
                return languageLoader.GetString("pageTitle_SearchView");
            }
            else if (type == typeof(SettingsView))
            {
                return languageLoader.GetString("pageTitle_SettingsView");
            }
            else if (type == typeof(TrendingView))
            {
                return languageLoader.GetString("pageTitle_TrendingView");
            }
            else if (type == typeof(GeneralSettingsView))
            {
                return "General";
            }
            else if (type == typeof(AboutSettingsView))
            {
                return "About";
            }
            else if (type == typeof(AppearanceView))
            {
                return "Appearance";
            }
            else if (type == typeof(DonateView))
            {
                return "Donate";
            }
            else if (type == typeof(CreditSettingsView))
            {
                return "Credits";
            }
            else if (type == typeof(CommitDetailView))
            {
                return "Commit";
            }
            else if (type == typeof(CommitsView))
            {
                return "Commits";
            }
            else
            {
                return "";
            }

            //throw new Exception("Page Title not found for the given (Page) type: " + type);
        }
    }
}
