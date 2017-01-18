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
                DarkThemeButton.Visibility = Visibility.Collapsed;
                LightThemeButton.Visibility = Visibility.Visible;
            }
            else
            {
                DarkThemeButton.Visibility = Visibility.Visible;
                LightThemeButton.Visibility = Visibility.Collapsed;
            }
                

        }
        private void LightThemeButton_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            SettingsService.SaveSetting("AppTheme", "Light");
            DarkThemeButton.Visibility = Visibility.Visible;
            LightThemeButton.Visibility = Visibility.Collapsed;
            e.Handled = true;
        }

        private void DarkThemeButton_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            SettingsService.SaveSetting("AppTheme", "Dark");
            DarkThemeButton.Visibility = Visibility.Collapsed;
            LightThemeButton.Visibility = Visibility.Visible;
            e.Handled = true;
        }
    }
}
