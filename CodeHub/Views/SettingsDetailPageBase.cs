using GalaSoft.MvvmLight.Ioc;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;

namespace CodeHub.Views
{
    public class SettingsDetailPageBase : Windows.UI.Xaml.Controls.Page
    {
        public void TryNavigateBackForDesktopState(string stateName)
        {
            if (stateName == "Desktop")
            {
                if (SimpleIoc.Default.GetInstance<Services.INavigationService>().CurrentSourcePageType != typeof(SettingsView))
                {
                    SimpleIoc.Default.GetInstance<Services.INavigationService>().GoBack();
                }
            }
        }
    }
}