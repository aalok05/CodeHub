using GalaSoft.MvvmLight.Messaging;
using CodeHub.Helpers;
using CodeHub.ViewModels;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace CodeHub.Views
{

    public sealed partial class DeveloperProfileView : Page
    {
        public DeveloperProfileViewmodel ViewModel;
        public DeveloperProfileView()
        {
            this.InitializeComponent();
            ViewModel = new DeveloperProfileViewmodel();
           
            this.DataContext = ViewModel;
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            await ViewModel.Load(e.Parameter as Octokit.User);

            if(ViewModel.Developer!= null)
            {
                if (ViewModel.Developer.Type == Octokit.AccountType.Organization)
                {
                    var languageLoader = new Windows.ApplicationModel.Resources.ResourceLoader();

                    Messenger.Default.Send(new GlobalHelper.SetHeaderTextMessageType { PageName = languageLoader.GetString("pageTitle_OrganizationView") });
                    Pivot.Items.Remove(FollowersPivotItem);
                    Pivot.Items.Remove(FollowingPivotItem);
                }
            }
           
        }
    }
}
