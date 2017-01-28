using CodeHub.ViewModels;
using System;
using Windows.ApplicationModel;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace CodeHub.Views
{
    public sealed partial class AboutView : SettingsDetailPageBase
    {
        private AboutViewmodel ViewModel;
        public AboutView()
        {
            this.InitializeComponent();

            ViewModel = new AboutViewmodel();
            this.DataContext = ViewModel;
        }
        private void OnCurrentStateChanged(object sender, VisualStateChangedEventArgs e)
        {
            TryNavigateBackForDesktopState(e.NewState.Name);
        }

        private async void RateButton_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(
                new Uri($"ms-windows-store://review/?PFN={Package.Current.Id.FamilyName}"));
        }
    }
}
