using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using CodeHub.Services;
using Windows.UI.Xaml;
using CodeHub.Helpers;
using System.Collections.ObjectModel;
using System.Linq;
using CodeHub.Models;
using Windows.UI.Xaml.Controls;
using CodeHub.Views;
using CodeHub.Services.Hilite_me;

namespace CodeHub.ViewModels
{
    public class SettingsViewModel : AppViewmodel
    {
        public string _currentState;
        public string CurrentState
        {
            get
            {
                return _currentState;
            }
            set
            {
                Set(() => CurrentState, ref _currentState, value);
            }
        }

        public ObservableCollection<SettingsItem> _settings;
        public ObservableCollection<SettingsItem> Settings
        {
            get
            {
                return _settings;
            }
            set
            {
                Set(() => Settings, ref _settings, value);
            }
        }

        #region about settings properties
        public string Logo => "/Assets/Images/appLogoPurple.png";

        public string DisplayName => Windows.ApplicationModel.Package.Current.DisplayName;

        public string Publisher => Windows.ApplicationModel.Package.Current.PublisherDisplayName;

        public string Version
        {
            get
            {
                var v = Windows.ApplicationModel.Package.Current.Id.Version;
                return $"{v.Major}.{v.Minor}.{v.Build}.{v.Revision}";
            }
        }
        #endregion

        #region about settings properties

        /// <summary>
        /// Gets the collection of the available highlight styles
        /// </summary>
        public IEnumerable<SyntaxHighlightStyle> AvailableHighlightStyles { get; } = Enum.GetValues(typeof(SyntaxHighlightStyle)).Cast<SyntaxHighlightStyle>();

        public int _SelectedHighlightStyleIndex = SettingsService.Get<int>(SettingsKeys.HighlightStyleIndex);

        /// <summary>
        /// Gets or sets the index of the currently selected highlight style
        /// </summary>
        public int SelectedHighlightStyleIndex
        {
            get { return _SelectedHighlightStyleIndex; }
            set
            {
                if (_SelectedHighlightStyleIndex != value)
                {
                    _SelectedHighlightStyleIndex = value;
                    SettingsService.Save(SettingsKeys.HighlightStyleIndex, value);
                    RaisePropertyChanged();
                    RaisePropertyChanged(nameof(HighlightStyle));
                }
            }
        }

        /// <summary>
        /// Gets the currently selected highlight style
        /// </summary>
        public SyntaxHighlightStyle HighlightStyle => (SyntaxHighlightStyle)SelectedHighlightStyleIndex;


        #endregion
        public SettingsViewModel()
        {
            Settings = new ObservableCollection<SettingsItem>()
            {
                new SettingsItem()
                {
                    MainText = "General",
                    SubText = "App preferences",
                    GlyphString = "\xEC7A",
                    DestPage = typeof(GeneralSettingsView)
                },
                new SettingsItem()
                {
                    MainText = "Appearance",
                    SubText = "UI customization",
                    GlyphString = "\xE7F4",
                    DestPage = typeof(AppearanceView)
                },
                new SettingsItem()
                {
                    MainText = "About",
                    SubText = "Developer info and contacts",
                    GlyphString = "\xE7BE",
                    DestPage = typeof(AboutSettingsView)
                },
                new SettingsItem()
                {
                    MainText = "Donate",
                    SubText = "Support the app and the developer",
                    GlyphString = "\xE170",
                    DestPage = typeof(DonateView)
                }
            };
        }

    }
}
