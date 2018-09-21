using GalaSoft.MvvmLight.Messaging;
using CodeHub.Helpers;
using CodeHub.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using Octokit;
using UICompositionAnimations;
using UICompositionAnimations.Enums;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace CodeHub.Views
{
    public sealed partial class IssuesView : Windows.UI.Xaml.Controls.Page
    {
        public IssuesViewmodel ViewModel { get; set; }

        private ScrollViewer OpenScrollViewer;
        private ScrollViewer ClosedScrollViewer;

        public IssuesView()
        {
            InitializeComponent();
            ViewModel = new IssuesViewmodel();

            DataContext = ViewModel;

            Unloaded += IssuesView_Unloaded;

            NavigationCacheMode = NavigationCacheMode.Required;
        }

        private void IssuesView_Unloaded(object sender, RoutedEventArgs e)
        {
            if (OpenScrollViewer != null)
                OpenScrollViewer.ViewChanged -= OnOpenScrollViewerViewChanged;

            if (ClosedScrollViewer != null)
                ClosedScrollViewer.ViewChanged -= OnClosedScrollViewerViewChanged;

            OpenScrollViewer = ClosedScrollViewer  = null;
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.NavigationMode != NavigationMode.Back)
            {
                await ViewModel.Load((Repository)e.Parameter);
                IssuesPivot.SelectedItem = IssuesPivot.Items[0];
            }
        }
        protected async override void OnNavigatedFrom(NavigationEventArgs e)
        {
            await ToggleNewIssuePanelVisibility(false);
        }

        private async void CancelNewIssueButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            await ToggleNewIssuePanelVisibility(false);
        }

        private async void AddIssueButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            await ToggleNewIssuePanelVisibility(true);
        }

        private async Task ToggleNewIssuePanelVisibility(bool visible)
        {
            //clearing the text in TextBoxes
            ViewModel.NewIssueTitleText = ViewModel.NewIssueBodyText = string.Empty;

            if (visible)
            {
                createIssueDialog.SetVisualOpacity(0);
                createIssueDialog.Visibility = Visibility.Visible;
                await createIssueDialog.StartCompositionFadeScaleAnimationAsync(0, 1, 1.1f, 1, 150, null, 0, EasingFunctionNames.SineEaseInOut);
            }
            else
            {
                await createIssueDialog.StartCompositionFadeScaleAnimationAsync(1, 0, 1, 1.1f, 150, null, 0, EasingFunctionNames.SineEaseInOut);
                createIssueDialog.Visibility = Visibility.Collapsed;
            }
        }

        private async void OnOpenScrollViewerViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            if (ViewModel.OpenPaginationIndex != -1)
            {
                ScrollViewer sv = (ScrollViewer)sender;

                var verticalOffset = sv.VerticalOffset;
                var maxVerticalOffset = sv.ScrollableHeight; //sv.ExtentHeight - sv.ViewportHeight;

                if ((maxVerticalOffset < 0 || verticalOffset == maxVerticalOffset) && verticalOffset > ViewModel.MaxOpenScrollViewerVerticalffset)
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

                if ((maxVerticalOffset < 0 || verticalOffset == maxVerticalOffset) && verticalOffset > ViewModel.MaxClosedScrollViewerVerticalffset)
                {
                    ViewModel.MaxClosedScrollViewerVerticalffset = maxVerticalOffset;

                    // Scrolled to bottom
                    if (GlobalHelper.IsInternet())
                        await ViewModel.ClosedIncrementalLoad();
                }
            }
        }

        private void openIssueListView_Loaded(object sender, RoutedEventArgs e)
        {
            if (OpenScrollViewer != null)
                OpenScrollViewer.ViewChanged -= OnOpenScrollViewerViewChanged;

            OpenScrollViewer = openIssueListView.FindChild<ScrollViewer>();
            OpenScrollViewer.ViewChanged += OnOpenScrollViewerViewChanged;
        }

        private void closedIssueListView_Loaded(object sender, RoutedEventArgs e)
        {
            if (ClosedScrollViewer != null)
                ClosedScrollViewer.ViewChanged -= OnClosedScrollViewerViewChanged;

            ClosedScrollViewer = closedIssueListView.FindChild<ScrollViewer>();
            ClosedScrollViewer.ViewChanged += OnClosedScrollViewerViewChanged;
        }
    }
}
