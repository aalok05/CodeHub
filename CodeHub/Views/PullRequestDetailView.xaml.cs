using CodeHub.Helpers;
using CodeHub.ViewModels;
using GalaSoft.MvvmLight.Messaging;
using Octokit;
using System;
using System.Threading.Tasks;
using UICompositionAnimations;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml;
using UICompositionAnimations.Enums;

namespace CodeHub.Views
{

    public sealed partial class PullRequestDetailView : Windows.UI.Xaml.Controls.Page
    {
        public PullRequestDetailViewmodel ViewModel;
        public PullRequestDetailView()
        {
            this.InitializeComponent();
            ViewModel = new PullRequestDetailViewmodel();

            this.DataContext = ViewModel;

        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            ViewModel.CommentText = string.Empty;

            await ViewModel.Load((e.Parameter as Tuple<Repository, PullRequest>));
            CommentsPivot.SelectedItem = CommentsPivot.Items[0];
        }

        protected async override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            await ToggleCommentDialogVisibility(false);
        }

        private async void CommentDialogOpen_Tapped(object sender, TappedRoutedEventArgs e)
        {
            await ToggleCommentDialogVisibility(true);
        }

        private async void CancelComment_Tapped(object sender, TappedRoutedEventArgs e)
        {
            await ToggleCommentDialogVisibility(false);
        }

        private async void Comment_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(ViewModel.CommentText))
            {
                await ToggleCommentDialogVisibility(false);
                ViewModel.CommentCommand.Execute(null);
            }
        }

        private async Task ToggleCommentDialogVisibility(bool visible)
        {
            if (visible)
            {
                CommentDialog.SetVisualOpacity(0);
                CommentDialog.Visibility = Visibility.Visible;
                await CommentDialog.StartCompositionFadeScaleAnimationAsync(0, 1, 1.1f, 1, 150, null, 0, EasingFunctionNames.SineEaseInOut);
            }
            else
            {
                await CommentDialog.StartCompositionFadeScaleAnimationAsync(1, 0, 1, 1.1f, 150, null, 0, EasingFunctionNames.SineEaseInOut);
                CommentDialog.Visibility = Visibility.Collapsed;
            }
        }
    }
}
