using GalaSoft.MvvmLight.Messaging;
using CodeHub.Helpers;
using CodeHub.ViewModels;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Composition;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;
using Octokit;
using System.Numerics;
using Windows.System.Profile;
using Windows.Devices.Input;

namespace CodeHub.Views
{
    public sealed partial class TrendingView : Windows.UI.Xaml.Controls.Page
    {
        public TrendingViewmodel ViewModel;

        private ScrollViewer TodayScrollViewer;
        private ScrollViewer WeekScrollViewer;
        private ScrollViewer MonthScrollViewer;

        public TrendingView()
        { 
            InitializeComponent();
            ViewModel = new TrendingViewmodel();
            DataContext = ViewModel;

            Unloaded += TrendingView_Unloaded;

            NavigationCacheMode = NavigationCacheMode.Required;
        }

        private void TrendingView_Unloaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if(TodayScrollViewer != null)
                TodayScrollViewer.ViewChanged -= OnTodayScrollViewerViewChanged;

            if (WeekScrollViewer != null)
                WeekScrollViewer.ViewChanged -= OnWeekScrollViewerViewChanged;

            if (MonthScrollViewer != null)
                MonthScrollViewer.ViewChanged -= OnMonthScrollViewerViewChanged;

            TodayScrollViewer = WeekScrollViewer = MonthScrollViewer = null;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            todayListView.SelectedIndex = weekListView.SelectedIndex = monthListView.SelectedIndex =  - 1;

            MouseCapabilities mouseCapabilities = new MouseCapabilities();
            bool hasMouse = mouseCapabilities.MousePresent != 0;

        }

        private async void OnTodayScrollViewerViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            if (ViewModel.CanLoadMoreToday)
            {
                ScrollViewer sv = (ScrollViewer)sender;

                var verticalOffset = sv.VerticalOffset;
                var maxVerticalOffset = sv.ScrollableHeight; //sv.ExtentHeight - sv.ViewportHeight;

                if (maxVerticalOffset < 0 || verticalOffset == maxVerticalOffset)
                {
                    // Scrolled to bottom
                    if (GlobalHelper.IsInternet())
                        await ViewModel.TodayIncrementalLoad();
                }
            }


        }
        private async void OnWeekScrollViewerViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            if (ViewModel.CanLoadMoreWeek)
            {
                ScrollViewer sv = (ScrollViewer)sender;

                var verticalOffset = sv.VerticalOffset;
                var maxVerticalOffset = sv.ScrollableHeight; //sv.ExtentHeight - sv.ViewportHeight;

                if (maxVerticalOffset < 0 || verticalOffset == maxVerticalOffset)
                {
                    // Scrolled to bottom
                    if (GlobalHelper.IsInternet())
                        await ViewModel.WeekIncrementalLoad();
                }
            }
        }
        private async void OnMonthScrollViewerViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            if (ViewModel.CanLoadMoreMonth)
            {
                ScrollViewer sv = (ScrollViewer)sender;

                var verticalOffset = sv.VerticalOffset;
                var maxVerticalOffset = sv.ScrollableHeight; //sv.ExtentHeight - sv.ViewportHeight;

                if (maxVerticalOffset < 0 || verticalOffset == maxVerticalOffset)
                {
                    // Scrolled to bottom
                    if (GlobalHelper.IsInternet())
                        await ViewModel.MonthIncrementalLoad();
                }
            }
        }

        private void todayListView_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (TodayScrollViewer != null)
                TodayScrollViewer.ViewChanged -= OnTodayScrollViewerViewChanged;

            TodayScrollViewer = todayListView.FindChild<ScrollViewer>();
            TodayScrollViewer.ViewChanged += OnTodayScrollViewerViewChanged;
        }

        private void weekListView_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (WeekScrollViewer != null)
                WeekScrollViewer.ViewChanged -= OnWeekScrollViewerViewChanged;

            WeekScrollViewer = weekListView.FindChild<ScrollViewer>();
            WeekScrollViewer.ViewChanged += OnWeekScrollViewerViewChanged;
        }

        private void monthListView_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (MonthScrollViewer != null)
                MonthScrollViewer.ViewChanged -= OnMonthScrollViewerViewChanged;

            MonthScrollViewer = monthListView.FindChild<ScrollViewer>();
            MonthScrollViewer.ViewChanged += OnMonthScrollViewerViewChanged;
        }
    }
}
