using CodeHub.Services;
using GalaSoft.MvvmLight;

namespace CodeHub.ViewModels.Settings
{
    public class NofiticationSettingsViewModel : ObservableObject
    {
        private bool _isToastEnabled = SettingsService.Get<bool>(SettingsKeys.IsToastEnabled);
        private bool _isLiveTilesEnabled = SettingsService.Get<bool>(SettingsKeys.IsLiveTilesEnabled);
        private bool _isLiveTilesBadgeEnabled = SettingsService.Get<bool>(SettingsKeys.IsLiveTilesBadgeEnabled);
        private bool _isAllBadgesUpdateEnabled = SettingsService.Get<bool>(SettingsKeys.IsLiveTileUpdateAllBadgesEnabled);

        private bool _isLiveTilesBadgeVisible = SettingsService.Get<bool>(SettingsKeys.IsLiveTilesBadgeVisible);
        private bool _isAllBadgesUpdateVisible = SettingsService.Get<bool>(SettingsKeys.IsLiveTileUpdateAllBadgesVisible);

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
                    IsLiveTilesBadgeEnabled = !value ? false : IsLiveTilesBadgeEnabled;
                    IsLiveTilesBadgeVisible = !value ? false : IsLiveTilesBadgeEnabled;
                    SettingsService.Save(SettingsKeys.IsLiveTilesEnabled, value);
                    RaisePropertyChanged(() => IsLiveTilesEnabled);
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
                    IsAllBadgesUpdateEnabled = !value ? false : IsAllBadgesUpdateEnabled;
                    IsAllBadgesUpdateVisible = !value ? false : IsAllBadgesUpdateVisible;
                    SettingsService.Save(SettingsKeys.IsLiveTilesBadgeEnabled, value);
                    RaisePropertyChanged(() => IsLiveTilesBadgeEnabled);
                }
            }
        }

        public bool IsAllBadgesUpdateEnabled
        {
            get => _isAllBadgesUpdateEnabled;
            set
            {
                if (_isAllBadgesUpdateEnabled != value)
                {
                    _isAllBadgesUpdateEnabled = value;
                    SettingsService.Save(SettingsKeys.IsLiveTileUpdateAllBadgesEnabled, value);
                    RaisePropertyChanged(() => IsAllBadgesUpdateEnabled);
                }
            }
        }

        public bool IsLiveTilesBadgeVisible
        {
            get => _isLiveTilesEnabled;
            private set
            {
                if (_isLiveTilesBadgeVisible != _isLiveTilesBadgeEnabled)
                {
                    _isLiveTilesBadgeVisible = _isLiveTilesEnabled;
                    RaisePropertyChanged(() => IsLiveTilesBadgeVisible);
                }
            }
        }
        public bool IsAllBadgesUpdateVisible
        {
            get => IsLiveTilesBadgeVisible;
            private set
            {
                if (_isAllBadgesUpdateVisible != _isLiveTilesBadgeVisible)
                {
                    _isAllBadgesUpdateVisible = _isLiveTilesBadgeVisible;
                    RaisePropertyChanged(() => IsAllBadgesUpdateVisible);
                }
            }
        }

        public NofiticationSettingsViewModel()
        {
        }
    }
}
