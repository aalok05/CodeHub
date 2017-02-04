using CodeHub.Services;
using Windows.UI.Xaml;
using CodeHub.Helpers;
using CodeHub.ViewModels;


namespace CodeHub.Views
{
    public sealed partial class AppearanceView : SettingsDetailPageBase
    {
        public AppearanceView()
        {
            this.InitializeComponent();
            this.DataContext = new AppearenceSettingsViewModel();
            if (SettingsService.Get<bool>(SettingsKeys.AppLightThemeEnabled))
            {
                DarkThemeButton.Visibility = Visibility.Visible;
                LightThemeButton.Visibility = Visibility.Collapsed;
            }
            else
            {
                DarkThemeButton.Visibility = Visibility.Collapsed;
                LightThemeButton.Visibility = Visibility.Visible;
            }
        }

        public AppearenceSettingsViewModel ViewModel => this.DataContext.To<AppearenceSettingsViewModel>();

        private void LightThemeButton_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            SettingsService.Save(SettingsKeys.AppLightThemeEnabled, true);
            DarkThemeButton.Visibility = Visibility.Visible;
            LightThemeButton.Visibility = Visibility.Collapsed;
            e.Handled = true;
        }

        private void DarkThemeButton_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            SettingsService.Save(SettingsKeys.AppLightThemeEnabled, false);
            DarkThemeButton.Visibility = Visibility.Collapsed;
            LightThemeButton.Visibility = Visibility.Visible;
            e.Handled = true;
        }

        private void OnCurrentStateChanged(object sender, VisualStateChangedEventArgs e)
        {
            if (e.NewState != null)
                TryNavigateBackForDesktopState(e.NewState.Name);
        }
    }
}
