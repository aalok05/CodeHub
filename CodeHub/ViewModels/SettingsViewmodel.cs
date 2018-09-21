using CodeHub.Models;
using CodeHub.Views;
using CodeHub.Views.Settings;
using System.Collections.ObjectModel;
using Windows.ApplicationModel.Resources;

namespace CodeHub.ViewModels
{
	public class SettingsViewModel : AppViewmodel
	{
		public string _currentState;
		public string CurrentState
		{
			get => _currentState;
			set => Set(() => CurrentState, ref _currentState, value);
		}

		public ObservableCollection<SettingsItem> _subMenus;
		public ObservableCollection<SettingsItem> SubMenus
		{
			get => _subMenus;
			set => Set(() => SubMenus, ref _subMenus, value);
		}

		public SettingsViewModel()
		{
			var languageLoader = new ResourceLoader();

			SubMenus = new ObservableCollection<SettingsItem>()
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
					MainText = languageLoader.GetString("menu_Settings_SubMenu_Notifications"),
					SubText = languageLoader.GetString("menu_Settings_SubMenu_Notifications_SubText"),
					GlyphString = "\xe747",
					DestPage = typeof(NotificationSettings)
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
		}
	}
}
