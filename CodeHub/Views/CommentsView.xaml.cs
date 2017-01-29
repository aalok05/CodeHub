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

    public sealed partial class CommentsView : Windows.UI.Xaml.Controls.Page
    {
        public CommentsViewmodel ViewModel;
        public CommentsView()
        {
            this.InitializeComponent();
            ViewModel = new CommentsViewmodel();
            this.DataContext = ViewModel;
           
            NavigationCacheMode = NavigationCacheMode.Required;
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            Messenger.Default.Send(new GlobalHelper.SetHeaderTextMessageType { PageName = "Comments" });
            if (e.NavigationMode != NavigationMode.Back)
            {
                 ViewModel.Load((e.Parameter as IssueComment));
            }
        }
    }
}
