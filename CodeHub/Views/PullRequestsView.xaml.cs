using CodeHub.Helpers;
using CodeHub.ViewModels;
using GalaSoft.MvvmLight.Messaging;
using Octokit;
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


namespace CodeHub.Views
{
    public sealed partial class PullRequestsView : Windows.UI.Xaml.Controls.Page
    {
        public PullRequestsViewmodel ViewModel { get; set; }

        public PullRequestsView()
        {
            this.InitializeComponent();
            ViewModel = new PullRequestsViewmodel();

            this.DataContext = ViewModel;

            NavigationCacheMode = NavigationCacheMode.Required;
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.NavigationMode != NavigationMode.Back)
            {
                await ViewModel.Load((Repository)e.Parameter);
                PullRequestPivot.SelectedItem = PullRequestPivot.Items[0];
            }
        }
    }
}
