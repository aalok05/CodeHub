using CodeHub.ViewModels;
using Windows.UI.Xaml;


namespace CodeHub.Views
{
    public sealed partial class GeneralSettingsView : SettingsDetailPageBase
    {
        private SettingsViewModel ViewModel;
        public GeneralSettingsView()
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

    }
}
