using CodeHub.ViewModels.Settings;
using Windows.UI.Xaml;


namespace CodeHub.Views
{
    public sealed partial class GeneralSettingsView : SettingsDetailPageBase
    {
        private GeneralSettingsViewModel ViewModel;

        public GeneralSettingsView()
        {
            InitializeComponent();

            ViewModel = new GeneralSettingsViewModel();

            DataContext = ViewModel;
        }
        private void OnCurrentStateChanged(object sender, VisualStateChangedEventArgs e)
        {
            if (e.NewState != null)
                TryNavigateBackForDesktopState(e.NewState.Name);
        }

    }
}
