using CodeHub.Controls;
using System;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml.Navigation;
using CodeHub.Helpers;
using GalaSoft.MvvmLight.Messaging;
using CodeHub.Views;

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
        private async Task<bool> NavigateCoreAsync(Type type, String pageTitle, object parameter)
        {
            await NavigationSemaphore.WaitAsync();
            bool result;

            Messenger.Default.Send(new GlobalHelper.SetHeaderTextMessageType { PageName = pageTitle });
            result = await Frame.Navigate(type, parameter);

            GlobalHelper.NavigationStack.Push(pageTitle);

            NavigationSemaphore.Release();
            return result;
        }

        /// <summary>
        /// Navigates to the target page
        /// </summary>
        /// <param name="pageType">The type of the target page</param>
        public Task<bool> NavigateAsync(Type pageType)
        {
            string pageTitle = ChoosePageTitleByPageType(pageType);

            return NavigateAsync(pageType, pageTitle);
        }

        /// <summary>
        /// Navigates to the target page with a given parameter
        /// </summary>
        /// <param name="pageType">The type of the target page</param>
        /// <param name="parameter">The navigation parameter</param>
        public Task<bool> NavigateAsync(Type pageType, object parameter)
        {
            string pageTitle = ChoosePageTitleByPageType(pageType);

            return NavigateAsync(pageType, pageTitle, parameter);
        }

        // Navigation without parameters
        public Task<bool> NavigateAsync(Type type, String pageTitle) => NavigateCoreAsync(type, pageTitle, null);

        // Navigation with parameters
        public Task<bool> NavigateAsync(Type type, String pageTitle, object parameter) => NavigateCoreAsync(type, pageTitle, parameter);

        // Navigation with parameters without animations
        public async void NavigateWithoutAnimations(Type type, String pageTitle, object parameter)
        {
            await NavigationSemaphore.WaitAsync();

            Frame.NavigateWithoutAnimations(type, parameter);
            GlobalHelper.NavigationStack.Push(pageTitle);
            NavigationSemaphore.Release();
        }

        // Straight, synchronous navigation without animations
        public async void NavigateWithoutAnimations(Type type, String pageTitle)
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
            else result = false;
            NavigationSemaphore.Release();
            return result;
        }

        /// <summary>
        /// Search for the Page Title with the given Menu type
        /// </summary>
        /// <param name="type">type of the Menu</param>
        /// <returns>string</returns>
        /// <exception cref="Exception">When the given type don't have a Page Title pair</exception> 
        public string ChoosePageTitleByPageType(Type type)
        {
                 if (type == typeof(CommentView))
            {
                return "Comment";
            }
            else if (type == typeof(FeedView))
            {
                return "News Feed";
            }
            else if (type == typeof(IssuesView))
            {
                return "Issues";
            }
            else if (type == typeof(MyOrganizationsView))
            {
                return "My Organizations";
            }
            else if (type == typeof(MyReposView))
            {
                return "My Repositories";
            }
            else if (type == typeof(NotificationsView))
            {
                return "Notifications";
            }
            else if (type == typeof(PullRequestDetailView))
            {
                return "Pull Request";
            }
            else if (type == typeof(PullRequestsView))
            {
                return "Pull Requests";
            }

            throw new Exception("Page Title not found for the given (Page) type: " + type);
        }
    }
}
