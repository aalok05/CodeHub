using Windows.UI.Xaml;


namespace CodeHub.Views.Settings 
{
    public sealed partial class CreditSettingsView : SettingsDetailPageBase
    {
        public CreditSettingsView()
        {
            InitializeComponent();
        }

        private void OnCurrentStateChanged(object sender, VisualStateChangedEventArgs e)
        {
            if (e.NewState != null)
                TryNavigateBackForDesktopState(e.NewState.Name);
        }
    }
}
