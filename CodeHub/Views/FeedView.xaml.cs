using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;
using CodeHub.ViewModels;
using GalaSoft.MvvmLight.Messaging;
using CodeHub.Helpers;
using Octokit;
using GalaSoft.MvvmLight.Ioc;
using Windows.UI.Xaml.Controls;
using System;
using Windows.UI.Xaml.Input;

namespace CodeHub.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class FeedView : Windows.UI.Xaml.Controls.Page
    {
        public FeedViewmodel ViewModel;
        private ScrollViewer FeedScrollViewer;

        public FeedView()
        {
            this.InitializeComponent();
            ViewModel = new FeedViewmodel();
            this.DataContext = ViewModel;
            Loading += FeedView_Loading;

            Unloaded += FeedView_Unloaded;

            NavigationCacheMode = NavigationCacheMode.Required;
        }

        private void FeedView_Unloaded(object sender, RoutedEventArgs e)
        {
            if (FeedScrollViewer != null)
                FeedScrollViewer.ViewChanged -= OnScrollViewerViewChanged;

            FeedScrollViewer = null;
        }

        private void FeedView_Loading(FrameworkElement sender, object args)
        {
            Messenger.Default.Register<User>(this, ViewModel.RecieveSignInMessage); //Listening for Sign In message
            Messenger.Default.Register<GlobalHelper.SignOutMessageType>(this, ViewModel.RecieveSignOutMessage); //listen for sign out message
        }

        private void Feed_PullProgressChanged(object sender, Microsoft.Toolkit.Uwp.UI.Controls.RefreshProgressEventArgs e)
        {
            refreshindicator.Opacity = e.PullProgress;
            refreshindicator.Background = e.PullProgress < 1.0 ? GlobalHelper.GetSolidColorBrush("4078C0FF") : GlobalHelper.GetSolidColorBrush("47C951FF");
        }
        private async void MarkdownTextBlock_LinkClicked(object sender, Microsoft.Toolkit.Uwp.UI.Controls.LinkClickedEventArgs e)
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri(e.Link));
        }

        private void FeedListView_Loaded(object sender, RoutedEventArgs e)
        {
            if (FeedScrollViewer != null)
                FeedScrollViewer.ViewChanged -= OnScrollViewerViewChanged;

            FeedScrollViewer = FeedListView.FindChild<ScrollViewer>();
            FeedScrollViewer.ViewChanged += OnScrollViewerViewChanged;
        }

        private async void OnScrollViewerViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            if (ViewModel.PaginationIndex != -1)
            {
                ScrollViewer sv = (ScrollViewer)sender;

                var verticalOffset = sv.VerticalOffset;
                var maxVerticalOffset = sv.ScrollableHeight; //sv.ExtentHeight - sv.ViewportHeight;

                if (maxVerticalOffset < 0 || verticalOffset == maxVerticalOffset)
                {
                    // Scrolled to bottom
                    if (GlobalHelper.IsInternet())
                    {
                        await ViewModel.LoadEvents();
                    }
                }
            }
        }
    }
}
