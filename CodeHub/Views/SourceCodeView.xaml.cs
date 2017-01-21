using CodeHub.Helpers;
using CodeHub.ViewModels;
using GalaSoft.MvvmLight.Messaging;
using Octokit;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace CodeHub.Views
{

    public sealed partial class SourceCodeView : Windows.UI.Xaml.Controls.Page
    {
        public SourceCodeViewmodel ViewModel;
        public SourceCodeView()
        {
            this.InitializeComponent();
            ViewModel = new SourceCodeViewmodel();

            this.DataContext = ViewModel;
            NavigationCacheMode = NavigationCacheMode.Required;
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            Messenger.Default.Send(new GlobalHelper.SetHeaderTextMessageType { PageName = (e.Parameter as Repository).FullName });

            if (e.NavigationMode == NavigationMode.Back)
            {
                ContentListView.SelectedIndex = -1;
                return;
            }
            if(ViewModel.Content!=null)
            {
                ViewModel.Content.Clear();
            }
            await ViewModel.Load(e.Parameter as Repository);
        }
        private void ScrollUpButton_Click(object sender, RoutedEventArgs e)
        {
            ContentListView?.ScrollIntoView(ContentListView.Items[0], ScrollIntoViewAlignment.Leading);
        }

    }
}
