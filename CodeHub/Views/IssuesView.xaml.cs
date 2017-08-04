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
    }
}
