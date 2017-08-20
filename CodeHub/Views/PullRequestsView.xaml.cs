using CodeHub.Helpers;
using CodeHub.ViewModels;
using GalaSoft.MvvmLight.Messaging;
using Octokit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;


namespace CodeHub.Views
{
    public sealed partial class PullRequestsView : Windows.UI.Xaml.Controls.Page
    {
        public PullRequestsViewmodel ViewModel { get; set; }

        private ScrollViewer OpenScrollViewer;
        private ScrollViewer ClosedScrollViewer;

        public PullRequestsView()
        {
            this.InitializeComponent();
            ViewModel = new PullRequestsViewmodel();

            this.DataContext = ViewModel;

            Unloaded += PullRequestsView_Unloaded;

            NavigationCacheMode = NavigationCacheMode.Required;
        }

        private void PullRequestsView_Unloaded(object sender, RoutedEventArgs e)
        {
            if (OpenScrollViewer != null)
                OpenScrollViewer.ViewChanged -= OnOpenScrollViewerViewChanged;

            if (ClosedScrollViewer != null)
                ClosedScrollViewer.ViewChanged -= OnClosedScrollViewerViewChanged;

            OpenScrollViewer = ClosedScrollViewer = null;
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.NavigationMode != NavigationMode.Back)
            {
                await ViewModel.Load((Repository)e.Parameter);
                PullRequestPivot.SelectedItem = PullRequestPivot.Items[0];
            }
        }

        private async void OnOpenScrollViewerViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            if (ViewModel.OpenPaginationIndex != -1)
            {
                ScrollViewer sv = (ScrollViewer)sender;

                var verticalOffset = sv.VerticalOffset;
                var maxVerticalOffset = sv.ScrollableHeight; //sv.ExtentHeight - sv.ViewportHeight;

                if ((maxVerticalOffset < 0 || verticalOffset == maxVerticalOffset) && maxVerticalOffset > ViewModel.MaxOpenScrollViewerVerticalffset)
                {
                    ViewModel.MaxOpenScrollViewerVerticalffset = maxVerticalOffset;

                    // Scrolled to bottom
                    if (GlobalHelper.IsInternet())
                        await ViewModel.OpenIncrementalLoad();
                }
            }


        }
        private async void OnClosedScrollViewerViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            if (ViewModel.ClosedPaginationIndex != -1)
            {
                ScrollViewer sv = (ScrollViewer)sender;

                var verticalOffset = sv.VerticalOffset;
                var maxVerticalOffset = sv.ScrollableHeight; //sv.ExtentHeight - sv.ViewportHeight;

                if (maxVerticalOffset < 0 || verticalOffset == maxVerticalOffset)
                {
                    // Scrolled to bottom
                    if (GlobalHelper.IsInternet())
                        await ViewModel.ClosedIncrementalLoad();
                }
            }
        }

        private void openPRListView_Loaded(object sender, RoutedEventArgs e)
        {
            if (OpenScrollViewer != null)
                OpenScrollViewer.ViewChanged -= OnOpenScrollViewerViewChanged;

            OpenScrollViewer = openPRListView.FindChild<ScrollViewer>();
            OpenScrollViewer.ViewChanged += OnOpenScrollViewerViewChanged;
        }

        private void closedPRListView_Loaded(object sender, RoutedEventArgs e)
        {
            if (ClosedScrollViewer != null)
                ClosedScrollViewer.ViewChanged -= OnClosedScrollViewerViewChanged;

            ClosedScrollViewer = closedPRListView.FindChild<ScrollViewer>();
            ClosedScrollViewer.ViewChanged += OnClosedScrollViewerViewChanged;
        }
    }
}
