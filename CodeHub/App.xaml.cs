using CodeHub.Views;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using CodeHub.Services;
using Windows.UI.ViewManagement;
using Windows.UI;
using Windows.Foundation.Metadata;
using Windows.Foundation;
using Windows.UI.Xaml.Media;
using CodeHub.Helpers;
using CodeHub.Services.Hilite_me;
using System.Threading.Tasks;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using XamlBrewer.Uwp.Controls;
using Windows.UI.Xaml.Controls;

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
            SettingsService.Save(SettingsKeys.HighlightStyleIndex, (int)SyntaxHighlightStyleEnum.Monokai, false);
            SettingsService.Save(SettingsKeys.ShowLineNumbers, true, false);
            SettingsService.Save(SettingsKeys.LoadCommitsInfo, false, false);
            SettingsService.Save(SettingsKeys.IsAdsEnabled, false, false);
            SettingsService.Save(SettingsKeys.IsNotificationCheckEnabled, true, false);
            SettingsService.Save(SettingsKeys.HasUserDonated, false, false);

            AppCenter.Start("4c8deb54-8947-45cb-9c75-98c3489a7ed6", typeof(Analytics), typeof(Crashes));
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

            if (!e.PrelaunchActivated)
            {
                if (Window.Current.Content == null)
                {
                    Window.Current.Content = new MainPage(null);
                    (Window.Current.Content as Page).OpenFromSplashScreen(e.SplashScreen.ImageLocation);
                }

                if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.ApplicationView"))
                {
                    var view = ApplicationView.GetForCurrentView();
                    view.SetPreferredMinSize(new Size(width: 800, height: 600));

                    var titleBar = ApplicationView.GetForCurrentView().TitleBar;
                    if (titleBar != null)
                    {
                        titleBar.BackgroundColor =
                        titleBar.ButtonBackgroundColor =
                        titleBar.InactiveBackgroundColor =
                        titleBar.ButtonInactiveBackgroundColor =
                        (Color)App.Current.Resources["SystemChromeLowColor"];

                        titleBar.ForegroundColor = (Color)App.Current.Resources["SystemChromeHighColor"];
                    }
                }

                // Ensure the current window is active
                Window.Current.Activate();
            }
        }

        protected async override void OnActivated(IActivatedEventArgs args)
        {
            if (args.Kind == ActivationKind.Protocol)
            {
                if (args.PreviousExecutionState == ApplicationExecutionState.Running)
                {
                    await HandleProtocolActivationArguments(args);
                }
                else
                {
                    if (SettingsService.Get<bool>(SettingsKeys.AppLightThemeEnabled))
                        XAMLHelper.AssignValueToXAMLResource("OddAlternatingRowsBrush", new SolidColorBrush { Color = Color.FromArgb(0x08, 0, 0, 0) });

                    if (Window.Current.Content == null)
                    {
                        Window.Current.Content = new MainPage(args);
                    }

                    if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.ApplicationView"))
                    {
                        var view = ApplicationView.GetForCurrentView();
                        view.SetPreferredMinSize(new Size(width: 800, height: 600));

                        var titleBar = ApplicationView.GetForCurrentView().TitleBar;
                        if (titleBar != null)
                        {
                            titleBar.BackgroundColor =
                            titleBar.ButtonBackgroundColor =
                            titleBar.InactiveBackgroundColor =
                            titleBar.ButtonInactiveBackgroundColor =
                            (Color)App.Current.Resources["SystemChromeLowColor"];

                            titleBar.ForegroundColor = (Color)App.Current.Resources["SystemChromeHighColor"];

                        }
                    }
                    Window.Current.Activate();
                }
            }
        }

        private async Task HandleProtocolActivationArguments(IActivatedEventArgs args)
        {
            if (!string.IsNullOrWhiteSpace(GlobalHelper.UserLogin))
            {
                ProtocolActivatedEventArgs eventArgs = args as ProtocolActivatedEventArgs;

                switch (eventArgs.Uri.Host.ToLower())
                {
                    case "repository":
                        await GalaSoft.MvvmLight.Ioc.SimpleIoc.Default.GetInstance<IAsyncNavigationService>().NavigateAsync(typeof(RepoDetailView), eventArgs.Uri.Segments[1] + eventArgs.Uri.Segments[2]);
                        break;

                    case "user":
                        await GalaSoft.MvvmLight.Ioc.SimpleIoc.Default.GetInstance<IAsyncNavigationService>().NavigateAsync(typeof(DeveloperProfileView), eventArgs.Uri.Segments[1]);
                        break;

                }
            }
        }
    }
}
