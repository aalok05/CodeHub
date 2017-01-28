using System;
using Windows.UI.Xaml.Navigation;
using CodeHub.ViewModels;
using Octokit;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using GalaSoft.MvvmLight.Messaging;
using CodeHub.Helpers;

namespace CodeHub.Views
{
    public sealed partial class ContentView : Windows.UI.Xaml.Controls.Page
    {
        public ContentViewmodel ViewModel;
        public ContentView()
        {
            this.Loaded += (s, e) => TopScroller.InitializeScrollViewer(ContentListView);
            this.InitializeComponent();
            ViewModel = new ContentViewmodel();
            this.DataContext = ViewModel;
            this.Unloaded += (s, e) => TopScroller.Dispose();
        }
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            //This page recieves repository ,path and branch
            var tuple = e.Parameter as Tuple<Repository, string, string>;

            Messenger.Default.Send(new GlobalHelper.SetHeaderTextMessageType { PageName = tuple.Item1.FullName });

            ContentListView.SelectedIndex = -1;

            if (ViewModel.Content != null)
            {
                ViewModel.Content.Clear();
            }
            await ViewModel.Load(tuple);
        }

        private void TopScroller_OnTopScrollingRequested(object sender, EventArgs e)
        {
            ContentListView.ScrollToTheTop();
        }
    }
}
