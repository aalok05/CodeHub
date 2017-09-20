using GalaSoft.MvvmLight.Messaging;
using CodeHub.Helpers;
using CodeHub.ViewModels;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace CodeHub.Views
{

    public sealed partial class DeveloperProfileView : Page
    {
        public DeveloperProfileViewmodel ViewModel;
        private ScrollViewer RepoScrollViewer;

        public DeveloperProfileView()
        {
            this.InitializeComponent();
            ViewModel = new DeveloperProfileViewmodel();

            Unloaded += DeveloperProfileView_Unloaded;
           
            this.DataContext = ViewModel;
        }

        private void DeveloperProfileView_Unloaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (RepoScrollViewer != null)
                RepoScrollViewer.ViewChanged -= OnRepoScrollViewerViewChanged;

            RepoScrollViewer = null;
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            await ViewModel.Load(e.Parameter);

            if(ViewModel.Developer!= null)
            {
                if (ViewModel.Developer.Type == Octokit.AccountType.Organization)
                {
                    var languageLoader = new Windows.ApplicationModel.Resources.ResourceLoader();

                    Messenger.Default.Send(new GlobalHelper.SetHeaderTextMessageType { PageName = languageLoader.GetString("pageTitle_OrganizationView") });
                    Pivot.Items.Remove(FollowersPivotItem);
                    Pivot.Items.Remove(FollowingPivotItem);
                }
            }
           
        }

        private void RepositoryListView_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (RepoScrollViewer != null)
                RepoScrollViewer.ViewChanged -= OnRepoScrollViewerViewChanged;

            RepoScrollViewer = RepositoryListView.FindChild<ScrollViewer>();
            RepoScrollViewer.ViewChanged += OnRepoScrollViewerViewChanged;
        }

        private async void OnRepoScrollViewerViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            if (ViewModel.PaginationIndex != -1)
            {
                ScrollViewer sv = (ScrollViewer)sender;

                var verticalOffset = sv.VerticalOffset;
                var maxVerticalOffset = sv.ScrollableHeight; //sv.ExtentHeight - sv.ViewportHeight;

                if ((maxVerticalOffset < 0 || verticalOffset == maxVerticalOffset) && verticalOffset > ViewModel.MaxScrollViewerOffset)
                {
                    ViewModel.MaxScrollViewerOffset = maxVerticalOffset;

                    // Scrolled to bottom
                    if (GlobalHelper.IsInternet())
                    {
                        await ViewModel.LoadRepos();
                    }
                }
            }
        }
    }
}
