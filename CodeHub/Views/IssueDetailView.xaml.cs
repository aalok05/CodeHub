using CodeHub.Helpers;
using CodeHub.ViewModels;
using GalaSoft.MvvmLight.Messaging;
using Octokit;
using System;
using Windows.UI.Xaml.Navigation;

namespace CodeHub.Views
{
    public sealed partial class IssueDetailView : Windows.UI.Xaml.Controls.Page
    {
        public IssueDetailViewmodel ViewModel;
        public IssueDetailView()
        {
            this.InitializeComponent();
            ViewModel = new IssueDetailViewmodel();
           
            this.DataContext = ViewModel;

            NavigationCacheMode = NavigationCacheMode.Required;
        }
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            Messenger.Default.Send(new GlobalHelper.SetHeaderTextMessageType { PageName = "Issues" });

            commentsListView.SelectedIndex = -1;

            if (e.NavigationMode != NavigationMode.Back)
            {
                //NavigationCacheMode = NavigationCacheMode.Disabled; //clearing the page cache

                if (ViewModel.Comments != null)
                {
                    ViewModel.Comments.Clear();
                }
                await ViewModel.Load((e.Parameter as Tuple<string,string, Issue>));
            }
        }
    }
}
