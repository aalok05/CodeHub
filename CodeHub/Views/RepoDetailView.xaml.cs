using GalaSoft.MvvmLight.Messaging;
using CodeHub.Helpers;
using CodeHub.ViewModels;
using Octokit;
using System.Collections.Generic;
using Windows.System.Profile;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;

namespace CodeHub.Views
{
    public sealed partial class RepoDetailView : Windows.UI.Xaml.Controls.Page
    {
        public RepoDetailViewmodel ViewModel;
        public RepoDetailView()
        {
            this.InitializeComponent();
            ViewModel = new RepoDetailViewmodel();
           
            this.DataContext = ViewModel;
        }
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            
            Messenger.Default.Send(new GlobalHelper.SetHeaderTextMessageType { PageName = "Repository" });
            await ViewModel.Load(e.Parameter as Repository);
        }
    }
}
