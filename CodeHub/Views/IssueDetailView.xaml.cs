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
    public sealed partial class IssueDetailView : Windows.UI.Xaml.Controls.Page
    {
        public IssueDetailViewmodel ViewModel;
        public IssueDetailView()
        {
            this.InitializeComponent();
            ViewModel = new IssueDetailViewmodel();
           
            this.DataContext = ViewModel;

            NavigationCacheMode = NavigationCacheMode.Enabled;
        }
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            Messenger.Default.Send(new GlobalHelper.SetHeaderTextMessageType { PageName = "Issues" });

            commentsListView.SelectedIndex = -1;

            if (e.NavigationMode != NavigationMode.Back)
            {
                if (ViewModel.Comments != null)
                {
                    ViewModel.Comments.Clear();
                }
                await ViewModel.Load((e.Parameter as Tuple<string,string, Issue>));
            }
        }
    }
}
