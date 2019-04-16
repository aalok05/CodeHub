using GalaSoft.MvvmLight.Messaging;
using CodeHub.Helpers;
using CodeHub.ViewModels;
using Octokit;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;
using Windows.System.Profile;
using Windows.Devices.Input;

namespace CodeHub.Views
{

    public sealed partial class MyReposView : Windows.UI.Xaml.Controls.Page
    {
        public MyReposViewmodel ViewModel { get; set; }
        public MyReposView()
        {
            InitializeComponent();
            ViewModel = new MyReposViewmodel();
            DataContext = ViewModel;
           
            Loading += MyReposView_Loading;

            NavigationCacheMode = NavigationCacheMode.Required;
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            RepoListView.SelectedIndex = StarredRepoListView.SelectedIndex = -1;

            ViewModel.User = (User)e.Parameter;

            MouseCapabilities mouseCapabilities = new MouseCapabilities();
            bool hasMouse = mouseCapabilities.MousePresent != 0;

        }
        private async void MyReposView_Loading(FrameworkElement sender, object args)
        {
            Messenger.Default.Register<User>(this, ViewModel.RecieveSignInMessage); //Listening for Sign In message
            Messenger.Default.Register<GlobalHelper.SignOutMessageType>(this, ViewModel.RecieveSignOutMessage); //listen for sign out message
            await ViewModel.Load();
        }    
    }
}
