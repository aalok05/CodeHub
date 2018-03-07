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
using GalaSoft.MvvmLight.Messaging;
using System.Windows.Input;
using Windows.Globalization;

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

        private ICommand _shoWWhatsNewCommand;
        public ICommand ShoWWhatsNewCommand
        {
            get
            {
                if (_shoWWhatsNewCommand == null)
                {
                    _shoWWhatsNewCommand = new RelayCommand(() => Messenger.Default.Send(new GlobalHelper.ShowWhatsNewPopupMessageType()));
                }

                return _shoWWhatsNewCommand;
            }
        }
        #endregion

        #region general settings properties

        /// <summary>
        /// Gets the collection of the available highlight styles
        /// </summary>
        public IEnumerable<SyntaxHighlightStyle> AvailableHighlightStyles { get; set; }

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

        public bool _ShowLineNumbers = SettingsService.Get<bool>(SettingsKeys.ShowLineNumbers);

        /// <summary>
        /// Gets or sets whether or not the line numbers should be visible in the highlighted code
        /// </summary>
        public bool ShowLineNumbers
        {
            get { return _ShowLineNumbers; }
            set
            {
                if (_ShowLineNumbers != value)
                {
                    _ShowLineNumbers = value;
                    SettingsService.Save(SettingsKeys.ShowLineNumbers, value);
                    RaisePropertyChanged();
                }
            }
        }

        public bool _LoadCommitsInfo = SettingsService.Get<bool>(SettingsKeys.LoadCommitsInfo);

        /// <summary>
        /// Gets or sets whether or not to load the additional commits info when browsing the contents of a repository
        /// </summary>
        public bool LoadCommitsInfo
        {
            get { return _LoadCommitsInfo; }
            set
            {
                if (_LoadCommitsInfo != value)
                {
                    _LoadCommitsInfo = value;
                    SettingsService.Save(SettingsKeys.LoadCommitsInfo, value);
                    RaisePropertyChanged();
                }
            }
        }

        public bool _IsAdsEnabled = SettingsService.Get<bool>(SettingsKeys.IsAdsEnabled);

        /// <summary>
        /// Gets or sets whether or not the Ads are enabled in the app
        /// </summary>
        public bool IsAdsEnabled
        {
            get { return _IsAdsEnabled; }
            set
            {
                if (_IsAdsEnabled != value)
                {
                    _IsAdsEnabled = value;
                    SettingsService.Save(SettingsKeys.IsAdsEnabled, value);
                    Messenger.Default.Send(new GlobalHelper.AdsEnabledMessageType());
                    RaisePropertyChanged();
                }
            }
        }

        public bool _CanDisableAds = GlobalHelper.HasAlreadyDonated;
        public bool CanDisableAds
        {
            get
            {
                return _CanDisableAds;
            }
            set
            {
                Set(() => CanDisableAds, ref _CanDisableAds, value);
            }
        }

        public bool _IsNotificationCheckEnabled = SettingsService.Get<bool>(SettingsKeys.IsNotificationCheckEnabled);

        /// <summary>
        /// Gets or sets whether API calls for unread notifications will be frequently made
        /// </summary>
        public bool IsNotificationCheckEnabled
        {
            get { return _IsNotificationCheckEnabled; }
            set
            {
                if (_IsNotificationCheckEnabled != value)
                {
                    _IsNotificationCheckEnabled = value;
                    SettingsService.Save(SettingsKeys.IsNotificationCheckEnabled, value);
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets the currently selected highlight style
        /// </summary>
        public SyntaxHighlightStyleEnum HighlightStyle => (SyntaxHighlightStyleEnum)SelectedHighlightStyleIndex;

        public List<Language> AvailableUiLanguages { get; private set; } = new List<Language>();

        private int _selectedUiLanguageIndex;
        public int SelectedUiLanguageIndex
        {
            get { return _selectedUiLanguageIndex; }
            set
            {
                Set(() => SelectedUiLanguageIndex, ref _selectedUiLanguageIndex, value);

                var language = AvailableUiLanguages[value];

                ApplicationLanguages.PrimaryLanguageOverride = language.LanguageTag;
            }
        }
        

        #endregion

        public SettingsViewModel()
        {
            var languageLoader = new Windows.ApplicationModel.Resources.ResourceLoader();

            Settings = new ObservableCollection<SettingsItem>()
            {
                new SettingsItem()
                {
                    MainText = languageLoader.GetString("menu_Settings_SubMenu_General"),
                    SubText = languageLoader.GetString("menu_Settings_SubMenu_General_SubText"),
                    GlyphString = "\xEC7A",
                    DestPage = typeof(GeneralSettingsView)
                },
                new SettingsItem()
                {
                    MainText = languageLoader.GetString("menu_Settings_SubMenu_Appearance"),
                    SubText = languageLoader.GetString("menu_Settings_SubMenu_Appearance_SubText"),
                    GlyphString = "\xE7F4",
                    DestPage = typeof(AppearanceView)
                },
                new SettingsItem()
                {
                    MainText = languageLoader.GetString("menu_Settings_SubMenu_About"),
                    SubText = languageLoader.GetString("menu_Settings_SubMenu_About_SubText"),
                    GlyphString = "\xE7BE",
                    DestPage = typeof(AboutSettingsView)
                },
                new SettingsItem()
                {
                    MainText = languageLoader.GetString("menu_Settings_SubMenu_Donate"),
                    SubText = languageLoader.GetString("menu_Settings_SubMenu_Donate_SubText"),
                    GlyphString = "\xE170",
                    DestPage = typeof(DonateView)
                },
                new SettingsItem()
                {
                    MainText = languageLoader.GetString("menu_Settings_SubMenu_Credits"),
                    SubText = languageLoader.GetString("menu_Settings_SubMenu_Credits_SubText"),
                    GlyphString = "\xE006",
                    DestPage = typeof(CreditSettingsView)
                }
            };

            AvailableHighlightStyles = new ObservableCollection<SyntaxHighlightStyle>
            {
                new SyntaxHighlightStyle{Name = "Borland",
                    ColorOne = GlobalHelper.GetSolidColorBrush("000080FF"),
                    ColorTwo = GlobalHelper.GetSolidColorBrush("000000FF"),
                    ColorThree = GlobalHelper.GetSolidColorBrush("008800FF"),
                    ColorFour = GlobalHelper.GetSolidColorBrush("000000FF"),
                    BackgroundColor = GlobalHelper.GetSolidColorBrush("FFFFFFFF") },
                new SyntaxHighlightStyle{Name = "Colorful",
                    ColorOne = GlobalHelper.GetSolidColorBrush("008800FF"),
                    ColorTwo = GlobalHelper.GetSolidColorBrush("0e84b5FF"),
                    ColorThree = GlobalHelper.GetSolidColorBrush("888888FF"),
                    ColorFour = GlobalHelper.GetSolidColorBrush("BB0066FF"),
                    BackgroundColor = GlobalHelper.GetSolidColorBrush("FFFFFFFF") },
                new SyntaxHighlightStyle{Name = "Emacs",
                    ColorOne = GlobalHelper.GetSolidColorBrush("AA22FFFF"),
                    ColorTwo = GlobalHelper.GetSolidColorBrush("0000FFFF"),
                    ColorThree = GlobalHelper.GetSolidColorBrush("008800FF"),
                    ColorFour = GlobalHelper.GetSolidColorBrush("0000FFFF"),
                    BackgroundColor = GlobalHelper.GetSolidColorBrush("f8f8f8FF") },
                new SyntaxHighlightStyle{Name = "Fruity",
                    ColorOne = GlobalHelper.GetSolidColorBrush("fb660aFF"),
                    ColorTwo = GlobalHelper.GetSolidColorBrush("ffffffFF"),
                    ColorThree = GlobalHelper.GetSolidColorBrush("008800FF"),
                    ColorFour = GlobalHelper.GetSolidColorBrush("ffffffFF"),
                    BackgroundColor = GlobalHelper.GetSolidColorBrush("111111FF") },
                new SyntaxHighlightStyle{Name = "Monokai",
                    ColorOne = GlobalHelper.GetSolidColorBrush("66d9efFF"),
                    ColorTwo = GlobalHelper.GetSolidColorBrush("f8f8f2FF"),
                    ColorThree = GlobalHelper.GetSolidColorBrush("75715eFF"),
                    ColorFour = GlobalHelper.GetSolidColorBrush("a6e22eFF"),
                    BackgroundColor = GlobalHelper.GetSolidColorBrush("272822FF") },
                new SyntaxHighlightStyle{Name = "Native",
                    ColorOne = GlobalHelper.GetSolidColorBrush("6ab825FF"),
                    ColorTwo = GlobalHelper.GetSolidColorBrush("447fcfFF"),
                    ColorThree = GlobalHelper.GetSolidColorBrush("999999FF"),
                    ColorFour = GlobalHelper.GetSolidColorBrush("447fcfFF"),
                    BackgroundColor = GlobalHelper.GetSolidColorBrush("202020FF") },
                new SyntaxHighlightStyle{Name = "Perldoc",
                    ColorOne = GlobalHelper.GetSolidColorBrush("8B008BFF"),
                    ColorTwo = GlobalHelper.GetSolidColorBrush("008b45FF"),
                    ColorThree = GlobalHelper.GetSolidColorBrush("228B22FF"),
                    ColorFour = GlobalHelper.GetSolidColorBrush("008b45FF"),
                    BackgroundColor = GlobalHelper.GetSolidColorBrush("eeeeddFF") },
                new SyntaxHighlightStyle{Name = "Vim",
                    ColorOne = GlobalHelper.GetSolidColorBrush("cdcd00FF"),
                    ColorTwo = GlobalHelper.GetSolidColorBrush("ccccccFF"),
                    ColorThree = GlobalHelper.GetSolidColorBrush("000080FF"),
                    ColorFour = GlobalHelper.GetSolidColorBrush("00cdcdFF"),
                    BackgroundColor = GlobalHelper.GetSolidColorBrush("000000FF") },
                new SyntaxHighlightStyle{Name = "VS",
                    ColorOne = GlobalHelper.GetSolidColorBrush("0000ffFF"),
                    ColorTwo = GlobalHelper.GetSolidColorBrush("000000FF"),
                    ColorThree = GlobalHelper.GetSolidColorBrush("008000FF"),
                    ColorFour = GlobalHelper.GetSolidColorBrush("2b91afFF"),
                    BackgroundColor = GlobalHelper.GetSolidColorBrush("ffffffFF") },
            };

            foreach (var languageTag in ApplicationLanguages.ManifestLanguages)
                AvailableUiLanguages.Add(new Language(languageTag));

            SelectedUiLanguageIndex = GetDefaultLanguageIndex();
        }

        private int GetDefaultLanguageIndex()
        {
            var topUserLanguage = ApplicationLanguages.PrimaryLanguageOverride;

            var language = new Language(topUserLanguage);

            int index = AvailableUiLanguages.FindIndex(l => l.NativeName.Equals(language.NativeName));

            return index;
        }
    }
}
