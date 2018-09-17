using GalaSoft.MvvmLight.Messaging;
using CodeHub.ViewModels;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Octokit;
using CodeHub.Helpers;

namespace CodeHub.Views
{
    public sealed partial class EditProfileView : Windows.UI.Xaml.Controls.Page
    {
        public EditProfileViewmodel ViewModel;

        public EditProfileView()
        {
            InitializeComponent();
            ViewModel = new EditProfileViewmodel();
            DataContext = ViewModel;
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            Messenger.Default.Send(new GlobalHelper.SetHeaderTextMessageType { PageName = "Edit Profile" });
            await ViewModel.Load(e.Parameter);          
        }
    }
}
