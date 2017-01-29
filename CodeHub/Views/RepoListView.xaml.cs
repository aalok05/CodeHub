using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using CodeHub.ViewModels;
using CodeHub.Helpers;
using GalaSoft.MvvmLight.Messaging;

namespace CodeHub.Views
{
    public sealed partial class RepoListView : Page
    {
        public RepoListViewmodel ViewModel { get; set; }
        public RepoListView()
        {
            this.InitializeComponent();
            ViewModel = new RepoListViewmodel();
         
            this.DataContext = ViewModel;

            NavigationCacheMode = NavigationCacheMode.Required;
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            Messenger.Default.Send(new GlobalHelper.SetHeaderTextMessageType { PageName = "Repositories" });

            repoListView.SelectedIndex = -1;

            if (e.NavigationMode != NavigationMode.Back)
            {
                if (ViewModel.Repositories != null)
                {
                    ViewModel.Repositories.Clear();
                }
                await ViewModel.Load((string)e.Parameter);
            }
        }
        private void AllRepos_PullProgressChanged(object sender, Microsoft.Toolkit.Uwp.UI.Controls.RefreshProgressEventArgs e)
        {
            refreshindicator.Opacity = e.PullProgress;
            refreshindicator.Background = e.PullProgress < 1.0 ? GlobalHelper.GetSolidColorBrush("4078C0FF") : GlobalHelper.GetSolidColorBrush("47C951FF");
        }
    }
}
