using CodeHub.Views;
using System;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;
using CodeHub.Services;
using Windows.UI.ViewManagement;
using Windows.UI;
using Windows.Foundation.Metadata;
using Windows.Foundation;
using Windows.UI.Xaml.Media;
using CodeHub.Controls;
using CodeHub.Helpers;
using CodeHub.Services.Hilite_me;

namespace CodeHub
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
          
            // Theme setup
            RequestedTheme = SettingsService.Get<bool>(SettingsKeys.AppLightThemeEnabled) ? ApplicationTheme.Light : ApplicationTheme.Dark;
            SettingsService.Save(SettingsKeys.HighlightStyleIndex, (int)SyntaxHighlightStyle.Monokai, false);
            SettingsService.Save(SettingsKeys.ShowLineNumbers, true, false);
            SettingsService.Save(SettingsKeys.LoadCommitsInfo, true, false);
            SettingsService.Save(SettingsKeys.IsAdsEnabled, false, false);
            SettingsService.Save(SettingsKeys.IsAcrylicBlurEnabled, false, false);
            SettingsService.Save(SettingsKeys.IsNotificationCheckEnabled, true, false);
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            // Set the right theme-depending color for the alternating rows
            if (SettingsService.Get<bool>(SettingsKeys.AppLightThemeEnabled))
                XAMLHelper.AssignValueToXAMLResource("OddAlternatingRowsBrush", new SolidColorBrush { Color = Color.FromArgb(0x08, 0, 0, 0) });

            CustomFrame rootFrame = Window.Current.Content as CustomFrame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new CustomFrame();

                rootFrame.NavigationFailed += OnNavigationFailed;
            }

            if (!e.PrelaunchActivated)
            {
                if (rootFrame.Content == null)
                {
                    Window.Current.Content = new MainPage();
                }

                if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.ApplicationView"))
                {
                    var view = ApplicationView.GetForCurrentView();
                    view.SetPreferredMinSize(new Size(width: 800, height: 600));

                    var titleBar = ApplicationView.GetForCurrentView().TitleBar;
                    if (titleBar != null)
                    {
                        titleBar.BackgroundColor = titleBar.ButtonBackgroundColor = (Color)App.Current.Resources["SystemAltHighColor"];
                    }
                }

                // Ensure the current window is active
                Window.Current.Activate();
            }
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }
    }
}
