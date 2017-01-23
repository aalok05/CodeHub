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

            repoListView.SelectedIndex = codeListView.SelectedIndex = userListView.SelectedIndex = issueListView.SelectedIndex = -1;

            Messenger.Default.Send(new GlobalHelper.SetHeaderTextMessageType { PageName = "Search" });

        }

    }
}
