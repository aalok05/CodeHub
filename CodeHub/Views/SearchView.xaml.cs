using GalaSoft.MvvmLight.Messaging;
using CodeHub.Helpers;
using CodeHub.ViewModels;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml;

namespace CodeHub.Views
{
    public sealed partial class SearchView : Page
    {
        public SearchViewmodel ViewModel;
        public SearchView()
        {
            this.InitializeComponent();
            ViewModel = new SearchViewmodel();
            this.DataContext = ViewModel;

            NavigationCacheMode = NavigationCacheMode.Required;

            MainSearchBox.Loaded += MainSearchBox_Loaded;
        }

        private void MainSearchBox_Loaded(object sender, RoutedEventArgs e)
        {
            MainSearchBox.Focus(FocusState.Programmatic);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            Messenger.Default.Send(new GlobalHelper.SetHeaderTextMessageType { PageName = "Search" });

        }

        private void RepoListView_PullProgressChanged(object sender, Microsoft.Toolkit.Uwp.UI.Controls.RefreshProgressEventArgs e)
        {
            refreshindicator.Opacity = e.PullProgress;
            refreshindicator.Background = e.PullProgress < 1.0 ? GlobalHelper.GetSolidColorBrush("4078C0FF") : GlobalHelper.GetSolidColorBrush("47C951FF");
        }
        private void UserListView_PullProgressChanged(object sender, Microsoft.Toolkit.Uwp.UI.Controls.RefreshProgressEventArgs e)
        {
            refreshindicator2.Opacity = e.PullProgress;
            refreshindicator2.Background = e.PullProgress < 1.0 ? GlobalHelper.GetSolidColorBrush("4078C0FF") : GlobalHelper.GetSolidColorBrush("47C951FF");
        }
    }
}
