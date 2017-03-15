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
       // public bool SystemTrayOptionAvailable => SystemTrayManager.IsAPIAvailable;

        public bool _HideSystemTray = SettingsService.Get<bool>(SettingsKeys.HideSystemTray);

        /// <summary>
        /// Gets or sets whether or not the system tray should be hidden
        /// </summary>
        //public bool HideSystemTray
        //{
        //    get { return _HideSystemTray; }
        //    set
        //    {
        //        if (_HideSystemTray != value)
        //        {
        //            _HideSystemTray = value;
        //            SettingsService.Save(SettingsKeys.HideSystemTray, value);
        //            if (value) SystemTrayManager.HideAsync().AsTask().Forget();
        //            else SystemTrayManager.TryShowAsync().Forget();
        //            RaisePropertyChanged();
        //        }
        //    }
        //}

        public bool _AppLightThemeEnabled = SettingsService.Get<bool>(SettingsKeys.AppLightThemeEnabled);

        /// <summary>
        /// Gets or sets if Light theme is enabled
        /// </summary>
        public bool AppLightThemeEnabled
        {
            get { return _AppLightThemeEnabled; }
            set
            {
                if (_AppLightThemeEnabled != value)
                {
                    _AppLightThemeEnabled = value;
                    SettingsService.Save(SettingsKeys.AppLightThemeEnabled, value);
                    RaisePropertyChanged();
                }
            }
        }
    }
}
