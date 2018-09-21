using CodeHub.Services;
using GalaSoft.MvvmLight;

namespace CodeHub.ViewModels.Settings
{
	public class NofiticationSettingsViewModel : ObservableObject
	{
		private bool _isToastEnabled = SettingsService.Get<bool>(SettingsKeys.IsToastEnabled);

		public bool IsToastEnabled
		{
			get => _isToastEnabled;
			set
			{
				if (_isToastEnabled != value)
				{
					_isToastEnabled = value;
					SettingsService.Save(SettingsKeys.IsToastEnabled, value);
					RaisePropertyChanged();
				}
			}
		}

		public NofiticationSettingsViewModel()
		{

		}
	}
}
