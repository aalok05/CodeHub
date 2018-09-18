using GalaSoft.MvvmLight.Ioc;

namespace CodeHub.Views.Settings
{
	public class SettingsDetailPageBase : Windows.UI.Xaml.Controls.Page
	{
		public void TryNavigateBackForDesktopState(string stateName)
		{
			if (stateName == "Desktop")
			{
				if (SimpleIoc.Default.GetInstance<Services.IAsyncNavigationService>().CurrentSourcePageType != typeof(SettingsView))
				{
					SimpleIoc.Default.GetInstance<Services.IAsyncNavigationService>().GoBackAsync();
				}
			}
		}
	}
}