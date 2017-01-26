using GalaSoft.MvvmLight.Messaging;
using CodeHub.Helpers;
using CodeHub.Models;
using CodeHub.Services;
using CodeHub.ViewModels;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using GalaSoft.MvvmLight.Ioc;

namespace CodeHub.Views
{
    public sealed partial class SettingsView : Page
    {
        public SettingsPageViewModel ViewModel;
        public SettingsView()
        {
            this.InitializeComponent();
            ViewModel = new SettingsPageViewModel();
          
            this.DataContext = ViewModel;
        }

        private void OnCurrentStateChanged(object sender, VisualStateChangedEventArgs e)
        {
            ViewModel.CurrentState = e.NewState.Name;
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            Messenger.Default.Send(new GlobalHelper.SetHeaderTextMessageType { PageName = "Settings" });

            if (Window.Current.Bounds.Width < 720)
            {
                ViewModel.CurrentState = "Mobile";
            }
            else
            {
                ViewModel.CurrentState = "Desktop";
            }
        }

        private async void SettingsListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var setting = e.ClickedItem as SettingsItem;

            switch(setting.MainText)
            {
                case "About":

                    if (ViewModel.CurrentState == "Mobile")
                    {
                        SimpleIoc.Default.GetInstance<Services.INavigationService>().Navigate(typeof(AboutView));
                    }
                    else
                    {
                        await settingsFrame.Navigate(typeof(AboutView));
                    }

                break;
            }
        }
    }
}
