using CodeHub.Helpers;
using CodeHub.Models;
using CodeHub.Services;
using CodeHub.Services.Hilite_me;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CodeHub.ViewModels.Settings
{
	public class AppearenceSettingsViewModel : AppViewmodel
	{
		public bool _HideSystemTray = SettingsService.Get<bool>(SettingsKeys.HideSystemTray);

		public bool _AppLightThemeEnabled = SettingsService.Get<bool>(SettingsKeys.AppLightThemeEnabled);

		public AppearenceSettingsViewModel()
		{
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
		}

		/// <summary>
		/// Gets or sets if Light theme is enabled
		/// </summary>
		public bool AppLightThemeEnabled
		{
			get => _AppLightThemeEnabled;
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

		/// <summary>
		/// Gets the currently selected highlight style
		/// </summary>
		public SyntaxHighlightStyleEnum HighlightStyle => (SyntaxHighlightStyleEnum)SelectedHighlightStyleIndex;

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
			get => _SelectedHighlightStyleIndex;
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
			get => _ShowLineNumbers;
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
	}
}