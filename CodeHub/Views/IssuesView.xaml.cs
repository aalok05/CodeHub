using GalaSoft.MvvmLight.Messaging;
using CodeHub.Helpers;
using CodeHub.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using Octokit;
using UICompositionAnimations;
using UICompositionAnimations.Enums;

namespace CodeHub.Views
{
    public sealed partial class IssuesView : Windows.UI.Xaml.Controls.Page
    {
        public IssuesViewmodel ViewModel { get; set; }
        public IssuesView()
        {
            this.InitializeComponent();
            ViewModel = new IssuesViewmodel();

            this.DataContext = ViewModel;

            NavigationCacheMode = NavigationCacheMode.Required;
        }
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            Messenger.Default.Send(new GlobalHelper.SetHeaderTextMessageType { PageName = "Issues" });
            createIssuePanel.Visibility = Visibility.Collapsed;

            if (e.NavigationMode != NavigationMode.Back)
            {
                await ViewModel.Load((Repository)e.Parameter);
            }
        }

        private void CancelNewIssueButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            ToggleNewIssuePanelVisibility(false);
        }

        private void AppBarButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            ToggleNewIssuePanelVisibility(true);
        }

        private void Background_Tapped(object sender, TappedRoutedEventArgs e)
        {
            ToggleNewIssuePanelVisibility(false);
        }

        private async void ToggleNewIssuePanelVisibility(bool visible)
        {
            if (visible)
            {
                createIssuePanel.Visibility = Visibility.Visible;
                await createIssuePanel.StartCompositionFadeScaleAnimationAsync(0, 1, 1.1f, 1, 150, null, 0, EasingFunctionNames.SineEaseInOut);
            }
            else
            {
                await createIssuePanel.StartCompositionFadeScaleAnimationAsync(1, 0, 1, 1.1f, 150, null, 0, EasingFunctionNames.SineEaseInOut);
                createIssuePanel.Visibility = Visibility.Collapsed;
                //clearing the text in TextBoxes
                ViewModel.NewIssueTitleText = ViewModel.NewIssueBodyText = string.Empty;
            }
        }
    }
}
