using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;
using CodeHub.ViewModels;
using GalaSoft.MvvmLight.Messaging;
using CodeHub.Helpers;
using Octokit;
using GalaSoft.MvvmLight.Ioc;
using Windows.UI.Xaml.Controls;

namespace CodeHub.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class FeedView : Windows.UI.Xaml.Controls.Page
    {
        public FeedViewmodel ViewModel;
        public FeedView()
        {
            this.InitializeComponent();
            ViewModel = new FeedViewmodel();
            this.DataContext = ViewModel;
            Loading += FeedView_Loading;

            NavigationCacheMode = NavigationCacheMode.Required;
        }

        private void FeedView_Loading(FrameworkElement sender, object args)
        {
            Messenger.Default.Register<User>(this, ViewModel.RecieveSignInMessage); //Listening for Sign In message
            Messenger.Default.Register<GlobalHelper.SignOutMessageType>(this, ViewModel.RecieveSignOutMessage); //listen for sign out message
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            Messenger.Default.Send(new GlobalHelper.SetHeaderTextMessageType { PageName = "News Feed" });
        }
        private void Feed_PullProgressChanged(object sender, Microsoft.Toolkit.Uwp.UI.Controls.RefreshProgressEventArgs e)
        {
            refreshindicator.Opacity = e.PullProgress;
            refreshindicator.Background = e.PullProgress < 1.0 ? GlobalHelper.GetSolidColorBrush("4078C0FF") : GlobalHelper.GetSolidColorBrush("47C951FF");
        }

        private void Actor_Click(object sender, RoutedEventArgs e)
        {
            SimpleIoc.Default.GetInstance<Services.IAsyncNavigationService>().NavigateAsync(typeof(DeveloperProfileView), "Profile", (sender as HyperlinkButton).Content);
        }
        private void Repo_Click(object sender, RoutedEventArgs e)
        {
            SimpleIoc.Default.GetInstance<Services.IAsyncNavigationService>().NavigateAsync(typeof(RepoDetailView),"Repository", (sender as HyperlinkButton).Content);
        }
    }
}
