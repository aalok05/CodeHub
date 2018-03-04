using System;
using CodeHub.Helpers;
using CodeHub.Services;
using GalaSoft.MvvmLight.Messaging;

namespace CodeHub.ViewModels
{
    /// <summary>
    /// ViewModel for the appearance page
    /// </summary>
    public class AppearenceSettingsViewModel : AppViewmodel
    {
        public bool _HideSystemTray = SettingsService.Get<bool>(SettingsKeys.HideSystemTray);

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
