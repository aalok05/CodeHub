using System;
using Windows.UI.Xaml.Navigation;
using CodeHub.ViewModels;
using Octokit;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace CodeHub.Views
{
    public sealed partial class ContentView : Windows.UI.Xaml.Controls.Page
    {
        public ContentViewmodel ViewModel;
        public ContentView()
        {
            this.InitializeComponent();
            ViewModel = new ContentViewmodel();
            this.DataContext = ViewModel;

            NavigationCacheMode = NavigationCacheMode.Required;
        }
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            var tuple = e.Parameter as Tuple<Repository, string, string>; //This page recieves repository ,path and branch
            await ViewModel.Load(tuple);
        }
        private void ScrollUpButton_Click(object sender, RoutedEventArgs e)
        {
            ContentListView?.ScrollIntoView(ContentListView.Items[0], ScrollIntoViewAlignment.Leading);
        }
    }
}
