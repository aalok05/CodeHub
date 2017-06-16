using CodeHub.ViewModels;
using System;
using Windows.ApplicationModel;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace CodeHub.Views
{
    public sealed partial class AboutSettingsView : SettingsDetailPageBase
    {
        private SettingsViewModel ViewModel;
        public AboutSettingsView()
        {
            this.InitializeComponent();

            ViewModel = new SettingsViewModel();
            this.DataContext = ViewModel;
        }
        private void OnCurrentStateChanged(object sender, VisualStateChangedEventArgs e)
        {
            if (e.NewState != null)
                TryNavigateBackForDesktopState(e.NewState.Name);
        }

        private async void RateButton_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(
                new Uri($"ms-windows-store://review/?PFN={Package.Current.Id.FamilyName}"));
        }

        private async void TwitterButton_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(
               new Uri("https://twitter.com/devaalok"));
        }

        private async void GithubButton_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(
               new Uri("https://github.com/aalok05/CodeHub"));
        }

        private async void EmailButton_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(
               new Uri("mailto:contact@devnextdoor.com"));
        }
    }
}
