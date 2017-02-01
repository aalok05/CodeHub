using System;
using CodeHub.Helpers;
using CodeHub.Services;

namespace CodeHub.ViewModels
{
    /// <summary>
    /// ViewModel for the appearance page
    /// </summary>
    public class AppearenceSettingsViewModel : AppViewmodel
    {
        public bool _HideSystemTray = SettingsService.Get<bool>(SettingsKeys.HideSystemTray);

        /// <summary>
        /// Gets or sets whether or not the system tray should be hidden
        /// </summary>
        public bool HideSystemTray
        {
            get { return _HideSystemTray; }
            set
            {
                if (_HideSystemTray != value)
                {
                    _HideSystemTray = value;
                    SettingsService.Save(SettingsKeys.HideSystemTray, value);
                    if (value) SystemTrayManager.HideAsync().AsTask().Forget();
                    else SystemTrayManager.TryShowAsync().Forget();
                    RaisePropertyChanged();
                }
            }
        }
    }
}
