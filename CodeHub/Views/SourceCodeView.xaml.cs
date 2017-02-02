using System;
using CodeHub.Helpers;
using CodeHub.ViewModels;
using GalaSoft.MvvmLight.Messaging;
using Octokit;
using Windows.UI.Xaml.Navigation;

namespace CodeHub.Views
{

    public sealed partial class SourceCodeView : Windows.UI.Xaml.Controls.Page
    {
        public SourceCodeViewmodel ViewModel;
        public SourceCodeView()
        {
            this.Loaded += (s, e) => TopScroller.InitializeScrollViewer(ContentListView);
            this.InitializeComponent();
            ViewModel = new SourceCodeViewmodel();
            this.DataContext = ViewModel;
            this.Unloaded += (s, e) => TopScroller.Dispose();
            NavigationCacheMode = NavigationCacheMode.Required;
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.Back)
            {
                Messenger.Default.Send(new GlobalHelper.SetHeaderTextMessageType { PageName = (e.Parameter as Repository).FullName });
                ContentListView.SelectedIndex = -1;
                return;
            }
            if(e.Parameter as Repository != ViewModel.Repository && ViewModel.Content!=null)
            {
                ViewModel.Content.Clear();
            }
            await ViewModel.Load(e.Parameter as Repository);
        }

        private void TopScroller_OnTopScrollingRequested(object sender, EventArgs e)
        {
            ContentListView.ScrollToTheTop();
        }
    }
}
