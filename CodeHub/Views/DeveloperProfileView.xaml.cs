using GalaSoft.MvvmLight.Messaging;
using CodeHub.Helpers;
using CodeHub.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System.Profile;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using GalaSoft.MvvmLight.Ioc;

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

            //Follow activity happened, refresh UI 
            Messenger.Default.Register<GlobalHelper.FollowActivityMessageType>(this, ViewModel.FollowActivity);
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            await ViewModel.Load(e.Parameter as string);

            if (ViewModel.Developer.Type == Octokit.AccountType.Organization)
                Messenger.Default.Send(new GlobalHelper.SetHeaderTextMessageType { PageName = "Organization" });
            else
                Messenger.Default.Send(new GlobalHelper.SetHeaderTextMessageType { PageName = "Profile" });
        }

        private void Actor_Click(object sender, RoutedEventArgs e)
        {
            SimpleIoc.Default.GetInstance<Services.IAsyncNavigationService>().NavigateAsync(typeof(DeveloperProfileView), "Profile", (sender as HyperlinkButton).Content);
        }
        private void Repo_Click(object sender, RoutedEventArgs e)
        {
            SimpleIoc.Default.GetInstance<Services.IAsyncNavigationService>().NavigateAsync(typeof(RepoDetailView), "Repository", (sender as HyperlinkButton).Content);
        }
    }
}
