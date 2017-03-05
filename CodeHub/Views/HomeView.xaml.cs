using GalaSoft.MvvmLight.Messaging;
using CodeHub.Helpers;
using CodeHub.ViewModels;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Composition;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;
using Octokit;
using System.Numerics;

namespace CodeHub.Views
{
    public sealed partial class HomeView : Windows.UI.Xaml.Controls.Page
    {
        public HomeViewmodel ViewModel;

        public HomeView()
        { 
            this.InitializeComponent();
            ViewModel = new HomeViewmodel();
            this.DataContext = ViewModel;

            Unloaded += HomeView_Unloaded;

            NavigationCacheMode = NavigationCacheMode.Required;
        }

        private void HomeView_Unloaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            todayIncrementalLoadButton.Dispose();
            weekIncrementalLoadButton.Dispose();
            monthIncrementalLoadButton.Dispose();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            Messenger.Default.Send(new GlobalHelper.SetHeaderTextMessageType { PageName = "Trending" });
            todayListView.SelectedIndex = weekListView.SelectedIndex = monthListView.SelectedIndex =  - 1;
        }

        private void Today_PullProgressChanged(object sender, Microsoft.Toolkit.Uwp.UI.Controls.RefreshProgressEventArgs e)
        {
            refreshindicator.Opacity = e.PullProgress;
            refreshindicator.Background = e.PullProgress < 1.0 ? GlobalHelper.GetSolidColorBrush("4078C0FF") : GlobalHelper.GetSolidColorBrush("47C951FF");
        }
        private void Week_PullProgressChanged(object sender, Microsoft.Toolkit.Uwp.UI.Controls.RefreshProgressEventArgs e)
        {
            refreshindicator2.Opacity = e.PullProgress;
            refreshindicator2.Background = e.PullProgress < 1.0 ? GlobalHelper.GetSolidColorBrush("4078C0FF") : GlobalHelper.GetSolidColorBrush("47C951FF");

        }
        private void Month_PullProgressChanged(object sender, Microsoft.Toolkit.Uwp.UI.Controls.RefreshProgressEventArgs e)
        {
            refreshindicator3.Opacity = e.PullProgress;
            refreshindicator3.Background = e.PullProgress < 1.0 ? GlobalHelper.GetSolidColorBrush("4078C0FF") : GlobalHelper.GetSolidColorBrush("47C951FF");
        }

        private void todayListView_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            todayIncrementalLoadButton.InitializeScrollViewer(todayListView);
        }

        private void weekListView_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            weekIncrementalLoadButton.InitializeScrollViewer(weekListView);
        }

        private void monthListView_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            monthIncrementalLoadButton.InitializeScrollViewer(monthListView);
        }
    }
}
