using CodeHub.Helpers;
using JetBrains.Annotations;
using Windows.Foundation.Collections;
using Windows.Storage;

namespace CodeHub.Services
{
	/// <summary>
	/// A static class that manages local app settings
	/// </summary>
	public static class SettingsService
	{
		// The settings container
		private static readonly IPropertySet Settings = ApplicationData.Current.LocalSettings.Values;

		/// <summary>
		/// Saves a setting with the given key
		/// </summary>
		/// <typeparam name="T">The type of the settisg to save</typeparam>
		/// <param name="key">The key of the setting to save</param>
		/// <param name="value">The value of the new setting</param>
		/// <param name="overwrite">Indicates wheter or not to overwrite an already existing setting</param>
		public static void Save<T>([NotNull] string key, T value, bool overwrite = true)
		{
			if (Settings.ContainsKey(key))
			{
				if (overwrite)
				{
					Settings[key] = value;
				}
			}
			else
			{
				Settings.Add(key, value);
			}
		}

		/// <summary>
		/// Gets a local setting with the given key
		/// </summary>
		/// <typeparam name="T">The type of the setting to retrieve</typeparam>
		/// <param name="key">The key of the setting to retrieve</param>
		public static T Get<T>([NotNull] string key) => Settings.ContainsKey(key) ? Settings[key].To<T>() : default(T);
	}

	/// <summary>
	/// A simple collection of settings keys used by the app
	/// </summary>
	public static class SettingsKeys
	{
		/* ================================
		 * Add useful setting keys here
		 * ============================== */

		public const string AppLightThemeEnabled = nameof(AppLightThemeEnabled);
		public const string HighlightStyleIndex = nameof(HighlightStyleIndex);
		public const string ShowLineNumbers = nameof(ShowLineNumbers);
		public const string HideSystemTray = nameof(HideSystemTray);
		public const string LoadCommitsInfo = nameof(LoadCommitsInfo);
		public const string IsAdsEnabled = nameof(IsAdsEnabled);
		public const string IsNotificationCheckEnabled = nameof(IsNotificationCheckEnabled);
		public const string CurrentVersion = nameof(CurrentVersion);
		public const string HasUserDonated = nameof(HasUserDonated);
		public const string IsToastEnabled = nameof(IsToastEnabled);
        public const string IsLiveTilesEnabled = nameof(IsLiveTilesEnabled);
        public const string IsLiveTilesBadgeEnabled = nameof(IsLiveTilesBadgeEnabled);
        public const string IsLiveTileUpdateAllBadgesEnabled = nameof(IsLiveTileUpdateAllBadgesEnabled);
    }
}
