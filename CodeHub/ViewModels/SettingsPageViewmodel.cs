using GalaSoft.MvvmLight.Command;
using System;
using CodeHub.Services;
using Windows.UI.Xaml;
using CodeHub.Helpers;
using System.Collections.ObjectModel;
using CodeHub.Models;
using Windows.UI.Xaml.Controls;
using CodeHub.Views;

namespace CodeHub.ViewModels
{
    public class SettingsPageViewModel : AppViewmodel
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

        public SettingsPageViewModel()
        {
            Settings = new ObservableCollection<SettingsItem>()
            {
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
                    DestPage = typeof(AboutView)
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
