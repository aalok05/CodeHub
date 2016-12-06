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

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            Messenger.Default.Send(new GlobalHelper.SetHeaderTextMessageType { PageName = "Settings" });

            if (SettingsService.GetSetting("AppTheme") == "Dark")
            {
                ThemeToggleSwitch.IsOn = false;

            }
            else
                ThemeToggleSwitch.IsOn = true;

        }

        /*We are not using Toggled event as we do not want to show alert when switch is toggled programmatically*/
        private void ThemeToggleSwitch_PointerReleased(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
        
            if (SettingsService.GetSetting("AppTheme") == "Dark")
            {
                SettingsService.SaveSetting("AppTheme", "Light");
            }
            else
            {
                SettingsService.SaveSetting("AppTheme", "Dark");
            }
            e.Handled = true;

        }
    }
}
