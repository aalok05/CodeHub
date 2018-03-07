using Windows.UI.Xaml;


namespace CodeHub.Views 
{
    public sealed partial class CreditSettingsView : SettingsDetailPageBase
    {
        public CreditSettingsView()
        {
            this.InitializeComponent();
        }

        private void OnCurrentStateChanged(object sender, VisualStateChangedEventArgs e)
        {
            if (e.NewState != null)
                TryNavigateBackForDesktopState(e.NewState.Name);
        }
    }
}
