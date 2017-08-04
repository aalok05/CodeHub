using CodeHub.Controls;
using System;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml.Navigation;
using CodeHub.Helpers;
using GalaSoft.MvvmLight.Messaging;

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

        // Navigation without parameters
        public Task<bool> NavigateAsync(Type type, String pageTitle) => NavigateCoreAsync(type, pageTitle, null);

        // Navigation with parameters
        public Task<bool> NavigateAsync(Type type, String pageTitle, object parameter) => NavigateCoreAsync(type, pageTitle, parameter);

        // Navigation with parameters without animations
        public async void NavigateWithoutAnimations(Type type, String pageTitle, object parameter)
        {
            await NavigationSemaphore.WaitAsync();

            Messenger.Default.Send(new GlobalHelper.SetHeaderTextMessageType { PageName = pageTitle });
            Frame.NavigateWithoutAnimations(type, parameter);

            NavigationSemaphore.Release();
        }

        // Straight, synchronous navigation without animations
        public async void NavigateWithoutAnimations(Type type, String pageTitle)
        {
            await NavigationSemaphore.WaitAsync();

            Messenger.Default.Send(new GlobalHelper.SetHeaderTextMessageType { PageName = pageTitle });
            Frame.NavigateWithoutAnimations(type);

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
    }
}
