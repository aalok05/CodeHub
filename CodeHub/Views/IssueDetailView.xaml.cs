using CodeHub.Helpers;
using CodeHub.ViewModels;
using GalaSoft.MvvmLight.Messaging;
using Octokit;
using System;
using UICompositionAnimations;
using UICompositionAnimations.Enums;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using System.Threading.Tasks;

namespace CodeHub.Views
{
    public sealed partial class IssueDetailView : Windows.UI.Xaml.Controls.Page
    {
        public IssueDetailViewmodel ViewModel;
        public IssueDetailView()
        {
            this.InitializeComponent();
            ViewModel = new IssueDetailViewmodel();

            this.DataContext = ViewModel;

            NavigationCacheMode = NavigationCacheMode.Enabled;
        }
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            Messenger.Default.Send(new GlobalHelper.SetHeaderTextMessageType { PageName = "Issue" });

            commentsListView.SelectedIndex = -1;
            ViewModel.CommentText = string.Empty;

            if (e.NavigationMode != NavigationMode.Back)
            {
                if (ViewModel.Comments != null)
                {
                    ViewModel.Comments.Clear();
                }

                ConfigureStateSymbol((e.Parameter as Tuple<Repository, Issue>).Item2);
                await ViewModel.Load((e.Parameter as Tuple<Repository, Issue>));
                CommentsPivot.SelectedItem = CommentsPivot.Items[0];
            }
        }

        protected async override void OnNavigatedFrom(NavigationEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.Back)
            {
                NavigationCacheMode = NavigationCacheMode.Disabled;
            }

            await ToggleEditIssuePanelVisibility(false);
        }

        public void ConfigureStateSymbol(Issue issue)
        {
            if (issue.State == ItemState.Open)
            {
                statePanel.Background = GlobalHelper.GetSolidColorBrush("2CBE4EFF");

                if (issue.PullRequest != null)
                {
                    //open issue
                    stateSymbol.Data = GlobalHelper.GetGeomtery("M7 2.3c3.14 0 5.7 2.56 5.7 5.7s-2.56 5.7-5.7 5.7A5.71 5.71 0 0 1 1.3 8c0-3.14 2.56-5.7 5.7-5.7zM7 1C3.14 1 0 4.14 0 8s3.14 7 7 7 7-3.14 7-7-3.14-7-7-7zm1 3H6v5h2V4zm0 6H6v2h2v-2z");
                }
                else
                {
                    //open PR
                    stateSymbol.Data = GlobalHelper.GetGeomtery("M11 11.28V5c-.03-.78-.34-1.47-.94-2.06C9.46 2.35 8.78 2.03 8 2H7V0L4 3l3 3V4h1c.27.02.48.11.69.31.21.2.3.42.31.69v6.28A1.993 1.993 0 0 0 10 15a1.993 1.993 0 0 0 1-3.72zm-1 2.92c-.66 0-1.2-.55-1.2-1.2 0-.65.55-1.2 1.2-1.2.65 0 1.2.55 1.2 1.2 0 .65-.55 1.2-1.2 1.2zM4 3c0-1.11-.89-2-2-2a1.993 1.993 0 0 0-1 3.72v6.56A1.993 1.993 0 0 0 2 15a1.993 1.993 0 0 0 1-3.72V4.72c.59-.34 1-.98 1-1.72zm-.8 10c0 .66-.55 1.2-1.2 1.2-.65 0-1.2-.55-1.2-1.2 0-.65.55-1.2 1.2-1.2.65 0 1.2.55 1.2 1.2zM2 4.2C1.34 4.2.8 3.65.8 3c0-.65.55-1.2 1.2-1.2.65 0 1.2.55 1.2 1.2 0 .65-.55 1.2-1.2 1.2z");

                }
            }
            else
            {
                statePanel.Background = GlobalHelper.GetSolidColorBrush("CB2431FF");

                if (issue.PullRequest != null)
                {
                    //closed issue
                    stateSymbol.Data = GlobalHelper.GetGeomtery("M7 10h2v2H7v-2zm2-6H7v5h2V4zm1.5 1.5l-1 1L12 9l4-4.5-1-1L12 7l-1.5-1.5zM8 13.7A5.71 5.71 0 0 1 2.3 8c0-3.14 2.56-5.7 5.7-5.7 1.83 0 3.45.88 4.5 2.2l.92-.92A6.947 6.947 0 0 0 8 1C4.14 1 1 4.14 1 8s3.14 7 7 7 7-3.14 7-7l-1.52 1.52c-.66 2.41-2.86 4.19-5.48 4.19v-.01z");
                }
                else
                {
                    //closed PR
                    stateSymbol.Data = GlobalHelper.GetGeomtery("M11 11.28V5c-.03-.78-.34-1.47-.94-2.06C9.46 2.35 8.78 2.03 8 2H7V0L4 3l3 3V4h1c.27.02.48.11.69.31.21.2.3.42.31.69v6.28A1.993 1.993 0 0 0 10 15a1.993 1.993 0 0 0 1-3.72zm-1 2.92c-.66 0-1.2-.55-1.2-1.2 0-.65.55-1.2 1.2-1.2.65 0 1.2.55 1.2 1.2 0 .65-.55 1.2-1.2 1.2zM4 3c0-1.11-.89-2-2-2a1.993 1.993 0 0 0-1 3.72v6.56A1.993 1.993 0 0 0 2 15a1.993 1.993 0 0 0 1-3.72V4.72c.59-.34 1-.98 1-1.72zm-.8 10c0 .66-.55 1.2-1.2 1.2-.65 0-1.2-.55-1.2-1.2 0-.65.55-1.2 1.2-1.2.65 0 1.2.55 1.2 1.2zM2 4.2C1.34 4.2.8 3.65.8 3c0-.65.55-1.2 1.2-1.2.65 0 1.2.55 1.2 1.2 0 .65-.55 1.2-1.2 1.2z");
                }
            }
        }

        private async void EditIssue_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            await ToggleEditIssuePanelVisibility(true);
        }

        private async void CancelEditIssue_Tapped(object sender, TappedRoutedEventArgs e)
        {
            await ToggleEditIssuePanelVisibility(false);
        }

        private async void EditIssueSaved_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (ViewModel.NewIssueTitleText != ViewModel.Issue.Title || ViewModel.NewIssueBodyText != ViewModel.Issue.Body)
            {
                await ViewModel.EditIssue();
                EditIssueDialog.Visibility = Visibility.Collapsed;
            }
        }

        private async Task ToggleEditIssuePanelVisibility(bool visible)
        {
            if (visible)
            {
                ViewModel.NewIssueBodyText = ViewModel.Issue.Body;
                ViewModel.NewIssueTitleText = ViewModel.Issue.Title;
                EditIssueDialog.SetVisualOpacity(0);
                EditIssueDialog.Visibility = Visibility.Visible;
                await EditIssueDialog.StartCompositionFadeScaleAnimationAsync(0, 1, 1.1f, 1, 150, null, 0, EasingFunctionNames.SineEaseInOut);
            }
            else
            {
                await EditIssueDialog.StartCompositionFadeScaleAnimationAsync(1, 0, 1, 1.1f, 150, null, 0, EasingFunctionNames.SineEaseInOut);
                EditIssueDialog.Visibility = Visibility.Collapsed;
            }
        }
    }
}
