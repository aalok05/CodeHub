using CodeHub.ViewModels;
using System.Threading.Tasks;
using UICompositionAnimations;
using UICompositionAnimations.Enums;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

namespace CodeHub.Views
{
    public sealed partial class IssueDetailView : Windows.UI.Xaml.Controls.Page
    {
        public IssueDetailViewmodel ViewModel;
        public IssueDetailView()
        {
            InitializeComponent();
            ViewModel = new IssueDetailViewmodel();

            DataContext = ViewModel;

        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            ViewModel.CommentText = string.Empty;

            await ViewModel.Load(e.Parameter);
            CommentsPivot.SelectedItem = CommentsPivot.Items[0];
        }

        protected async override void OnNavigatedFrom(NavigationEventArgs e)
        {
            ViewModel.NewIssueBodyText = ViewModel.NewIssueTitleText = string.Empty;

            await ToggleEditIssuePanelVisibility(false);
            await ToggleCommentDialogVisibility(false);
        }

        private async void EditIssue_Tapped(object sender, TappedRoutedEventArgs e)
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

        private async Task ToggleEditIssuePanelVisibility(bool visible)
        {
            if (visible)
            {
                ViewModel.NewIssueBodyText = ViewModel.Issue.Body;
                ViewModel.NewIssueTitleText = ViewModel.Issue.Title;
                EditIssueMarkdownEditorControl.SetMarkdowntext(ViewModel.NewIssueBodyText);
                EditIssueDialog.Opacity = 0;
                EditIssueDialog.Visibility = Visibility.Visible;

                await EditIssueDialog.Animation()
                    .Opacity(0, 1)
                    .Scale(1.1f, 1, Easing.SineEaseInOut)
                    .Duration(150)
                    .StartAsync();
            }
            else
            {
                await EditIssueDialog.Animation()
                    .Opacity(1, 0)
                    .Scale(1, 1.1f, Easing.SineEaseInOut)
                    .Duration(150)
                    .StartAsync();

                EditIssueDialog.Visibility = Visibility.Collapsed;
            }
        }

        private async Task ToggleCommentDialogVisibility(bool visible)
        {
            if (visible)
            {
                CommentDialog.Opacity = 0;
                CommentDialog.Visibility = Visibility.Visible;

                await CommentDialog.Animation()
                    .Opacity(0, 1)
                    .Scale(1.1f, 1, Easing.SineEaseInOut)
                    .Duration(150)
                    .StartAsync();
            }
            else
            {
                await CommentDialog.Animation()
                    .Opacity(1, 0)
                    .Scale(1, 1.1f, Easing.SineEaseInOut)
                    .Duration(150)
                    .StartAsync();

                CommentDialog.Visibility = Visibility.Collapsed;
            }
        }

        private async void Expander_Click(object sender, RoutedEventArgs e)
        {
            if (DetailPanel.Visibility == Visibility.Visible)
            {
                ExpanderIcon.Glyph = "\uE0E5";

                await DetailPanel.Animation()
                    .Opacity(1, 0)
                    .Scale(1, 0.98f, Easing.SineEaseInOut)
                    .Duration(100)
                    .StartAsync();

                DetailPanel.Visibility = Visibility.Collapsed;
            }
            else
            {
                ExpanderIcon.Glyph = "\uE0E4";
                DetailPanel.Visibility = Visibility.Visible;

                await DetailPanel.Animation()
                    .Opacity(0, 1)
                    .Scale(0.98f, 1, Easing.SineEaseInOut)
                    .Duration(100)
                    .StartAsync();
            }
        }

        private async void VisualStateGroup_CurrentStateChanged(object sender, VisualStateChangedEventArgs e)
        {
            if (e.NewState.Name.Equals("Wide") || e.NewState.Name.Equals("Normal"))
            {
                ExpanderIcon.Glyph = "\uE0E4";
                DetailPanel.Opacity = 0;
                DetailPanel.Visibility = Visibility.Visible;

                await DetailPanel.Animation()
                    .Opacity(0, 1)
                    .Scale(0.98f, 1, Easing.SineEaseInOut)
                    .Duration(100)
                    .StartAsync();
            }
        }
    }
}
