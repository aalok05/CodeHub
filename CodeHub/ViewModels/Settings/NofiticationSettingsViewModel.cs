using CodeHub.Services;
using GalaSoft.MvvmLight;

namespace CodeHub.ViewModels.Settings
{
    public class NofiticationSettingsViewModel : ObservableObject
    {
        private bool _isToastEnabled = SettingsService.Get<bool>(SettingsKeys.IsToastEnabled);
        private bool _isLiveTilesEnabled = SettingsService.Get<bool>(SettingsKeys.IsLiveTilesEnabled);
        private bool _isLiveTilesBadgeEnabled = SettingsService.Get<bool>(SettingsKeys.IsLiveTilesBadgeEnabled);
        private bool _isLiveTileUpdateAllBadgesEnabled = SettingsService.Get<bool>(SettingsKeys.IsLiveTileUpdateAllBadgesEnabled);

        public bool IsToastEnabled
        {
            get => _isToastEnabled;
            set
            {
                if (_isToastEnabled != value)
                {
                    _isToastEnabled = value;
                    SettingsService.Save(SettingsKeys.IsToastEnabled, value);
                    RaisePropertyChanged(() => IsToastEnabled);
                }
            }
        }

        public bool IsLiveTilesEnabled
        {
            get => _isLiveTilesEnabled;
            set
            {
                if (_isLiveTilesEnabled != value)
                {
                    _isLiveTilesEnabled = value;
                    SettingsService.Save(SettingsKeys.IsLiveTilesEnabled, value);
                    RaisePropertyChanged(() => IsLiveTilesEnabled);
                }
                if (!value && IsLiveTilesBadgeEnabled)
                {
                    IsLiveTilesBadgeEnabled = false;
                }
            }
        }

        public bool IsLiveTilesBadgeEnabled
        {
            get => _isLiveTilesBadgeEnabled;
            set
            {
                if (_isLiveTilesBadgeEnabled != value)
                {
                    _isLiveTilesBadgeEnabled = value;
                    SettingsService.Save(SettingsKeys.IsLiveTilesBadgeEnabled, value);
                    RaisePropertyChanged(() => IsLiveTilesBadgeEnabled);
                }
                if (!value && IsAllBadgesUpdateEnabled)
                {
                    IsAllBadgesUpdateEnabled = false;
                }
            }
        }

        public bool IsAllBadgesUpdateEnabled
        {
            get => _isLiveTilesEnabled;
            set
            {
                if (_isLiveTileUpdateAllBadgesEnabled != value)
                {
                    _isLiveTileUpdateAllBadgesEnabled = value;
                    SettingsService.Save(SettingsKeys.IsLiveTileUpdateAllBadgesEnabled, value);
                    RaisePropertyChanged(() => IsAllBadgesUpdateEnabled);
                }
            }
        }

        public NofiticationSettingsViewModel()
        {

        }
    }
}
