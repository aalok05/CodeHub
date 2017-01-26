using CodeHub.ViewModels;
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
    }
}
