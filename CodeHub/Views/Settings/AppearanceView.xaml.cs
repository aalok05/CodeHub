using CodeHub.Services;
using Windows.UI.Xaml;
using CodeHub.Helpers;
using CodeHub.ViewModels;
using Windows.UI.Xaml.Navigation;
using Windows.System.Profile;
using Microsoft.Toolkit.Uwp.Helpers;
using CodeHub.ViewModels.Settings;
using System;

namespace CodeHub.Views.Settings
{
    public sealed partial class AppearanceView : SettingsDetailPageBase
    {
        public AppearanceView()
        {
            InitializeComponent();
            DataContext = new AppearenceSettingsViewModel();
        }

        public AppearenceSettingsViewModel ViewModel => DataContext.To<AppearenceSettingsViewModel>();

        private void OnCurrentStateChanged(object sender, VisualStateChangedEventArgs e)
        {
            if (e.NewState != null)
                TryNavigateBackForDesktopState(e.NewState.Name);
        }
    }
}
