using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using CodeHub.Helpers;
using CodeHub.Models;
using CodeHub.Services;
using CodeHub.ViewModels;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using static CodeHub.Helpers.GlobalHelper;
using Octokit;

namespace CodeHub.Views
{
    public sealed partial class MainPage : Windows.UI.Xaml.Controls.Page
    {
        public MainViewmodel ViewModel { get; set; }
        public Frame AppFrame { get { return this.mainFrame; } }
        public MainPage()
        {
            this.InitializeComponent();

            ViewModel = new MainViewmodel();
            this.DataContext = ViewModel;

            SizeChanged += MainPage_SizeChanged;

            Messenger.Default.Register<NoInternetMessageType>(this, ViewModel.RecieveNoInternetMessage); //Listening for No Internet message
            Messenger.Default.Register<HasInternetMessageType>(this, ViewModel.RecieveInternetMessage); //Listening Internet available message
            Messenger.Default.Register(this, delegate(SetHeaderTextMessageType m)
            {
                ViewModel.setHeadertext(m.PageName);
            });  //Setting Header Text to the current page name

            SimpleIoc.Default.Register<INavigationService>(() =>
            { return new NavigationService(mainFrame); });

            NavigationCacheMode = NavigationCacheMode.Enabled;

            SystemNavigationManager.GetForCurrentView().BackRequested += SystemNavigationManager_BackRequested;
        }

        private void MainPage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if(e.NewSize.Width < 720)
            {
                if (ViewModel.isLoggedin)
                {
                    BottomAppBar.Visibility = Visibility.Visible;
                }
                else
                {
                    BottomAppBar.Visibility = Visibility.Collapsed;
                }
            }
        }
        private void SystemNavigationManager_BackRequested(object sender, BackRequestedEventArgs e)
        {
            bool handled = e.Handled;
            this.BackRequested(ref handled);
            e.Handled = handled;
        }
        private void HamButton_Click(object sender, RoutedEventArgs e)
        {
            HamSplitView.IsPaneOpen = !HamSplitView.IsPaneOpen;
        }
        private async void HamListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            HamSplitView.IsPaneOpen = false;
            await mainFrame.Navigate((e.ClickedItem as HamItem).DestPage, ViewModel.User);
            foreach (var i in ViewModel.HamItems)
            {
                i.IsSelected = false;
            }
            (e.ClickedItem as HamItem).IsSelected = true;

        }
        private void SettingsItem_ItemClick(object sender, TappedRoutedEventArgs e)
        {
            ViewModel.NavigateToSettings();
            HamSplitView.IsPaneOpen = false;
        }
        private void BackRequested(ref bool handled)
        {
            if (this.AppFrame == null)
                return;

            if (this.AppFrame.CanGoBack && !handled)
            {
                handled = true;
                this.AppFrame.GoBack();
            }
        }
        private async void AppBarTrending_Tapped(object sender, TappedRoutedEventArgs e)
        {
            await mainFrame.Navigate(ViewModel.HamItems[0].DestPage, ViewModel.User);
            foreach (var i in ViewModel.HamItems)
            {
                i.IsSelected = false;
            }
            ViewModel.HamItems[0].IsSelected = true;
        }
        private async void AppBarProfile_Tapped(object sender, TappedRoutedEventArgs e)
        {
            await mainFrame.Navigate(ViewModel.HamItems[1].DestPage, ViewModel.User);
            foreach (var i in ViewModel.HamItems)
            {
                i.IsSelected = false;
            }
            ViewModel.HamItems[1].IsSelected = true;
        }
        private async void AppBarMyRepos_Tapped(object sender, TappedRoutedEventArgs e)
        {
            await mainFrame.Navigate(ViewModel.HamItems[2].DestPage, ViewModel.User);
            foreach (var i in ViewModel.HamItems)
            {
                i.IsSelected = false;
            }
            ViewModel.HamItems[2].IsSelected = true;
        }
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            ViewModel.isLoggedin = (bool)e.Parameter;

            await mainFrame.Navigate(ViewModel.HamItems[0].DestPage, ViewModel.User);
            foreach (var i in ViewModel.HamItems)
            {
                i.IsSelected = false;
            }
            ViewModel.HamItems[0].IsSelected = true;

            //Listening for Sign In message
            Messenger.Default.Register<User>(this, RecieveSignInMessage);

            //listen for sign out message
            Messenger.Default.Register<SignOutMessageType>(this, RecieveSignOutMessage);

            /* This has to be done because the visibilty of BottomAppBar
             * is dependent on screen size as well as isLoggedin property
             * If visibility is bound with isLoggedin, it will disregard 
             * VisualStateManager at first loading of Page.
             * */
            if (Window.Current.Bounds.Width < 720)
            {
                if (ViewModel.isLoggedin)
                {
                    BottomAppBar.Visibility = Visibility.Visible;
                }
                else
                {
                    BottomAppBar.Visibility = Visibility.Collapsed;
                }
            }
        }
        public void RecieveSignOutMessage(SignOutMessageType empty)
        {
            if (Window.Current.Bounds.Width < 720)
            {
                BottomAppBar.Visibility = Visibility.Collapsed;
            }
        }
        public void RecieveSignInMessage(User user)
        {
            if (Window.Current.Bounds.Width < 720)
            {
                BottomAppBar.Visibility = Visibility.Visible;
            }
        }
    }
}
