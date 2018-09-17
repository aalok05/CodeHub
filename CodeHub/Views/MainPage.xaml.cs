using CodeHub.Controls;
using CodeHub.Models;
using CodeHub.Services;
using CodeHub.ViewModels;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Toolkit.Uwp.Helpers;
using Octokit;
using RavinduL.LocalNotifications;
using RavinduL.LocalNotifications.Notifications;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UICompositionAnimations;
using UICompositionAnimations.Enums;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using static CodeHub.Helpers.GlobalHelper;

namespace CodeHub.Views
{
	public sealed partial class MainPage : Windows.UI.Xaml.Controls.Page
	{
		public MainViewmodel ViewModel { get; set; }
		public CustomFrame AppFrame { get { return mainFrame; } }
		private readonly SemaphoreSlim HeaderAnimationSemaphore = new SemaphoreSlim(1);
		private LocalNotificationManager notifManager;

		public MainPage(IActivatedEventArgs args)
		{
			InitializeComponent();

			var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
			coreTitleBar.ExtendViewIntoTitleBar = true;

			coreTitleBar.LayoutMetricsChanged += delegate
			{
				AppTitleBar.Height = coreTitleBar.Height;
			};

			// Set a XAML element as title bar
			Window.Current.SetTitleBar(AppTitleBar);

			ViewModel = new MainViewmodel(args);
			DataContext = ViewModel;

			#region registering for messages
			Messenger.Default.Register<LocalNotificationMessageType>(this, RecieveLocalNotificationMessage);
			Messenger.Default.Register(this, delegate (SetHeaderTextMessageType m) { SetHeadertext(m.PageName); });
			Messenger.Default.Register(this, delegate (AdsEnabledMessageType m) { ViewModel.ToggleAdsVisiblity(); });
			Messenger.Default.Register(this, delegate (UpdateUnreadNotificationMessageType m) { ViewModel.UpdateUnreadNotificationIndicator(m.IsUnread); });
			Messenger.Default.Register(this, async delegate (ShowWhatsNewPopupMessageType m) { await ShowWhatsNewPopupVisiblity(); });
			Messenger.Default.Register<User>(this, ViewModel.RecieveSignInMessage);
			#endregion

			notifManager = new LocalNotificationManager(NotificationGrid);

			Loaded += MainPage_Loaded;
			SizeChanged += MainPage_SizeChanged;

			SimpleIoc.Default.Register<IAsyncNavigationService>(() => { return new NavigationService(AppFrame); });
			SystemNavigationManager.GetForCurrentView().BackRequested += SystemNavigationManager_BackRequested;

			NavigationCacheMode = NavigationCacheMode.Enabled;
		}

		private void MainPage_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			if (Window.Current.Bounds.Width < 1024)
			{
				ViewModel.DisplayMode = SplitViewDisplayMode.Overlay;
				ViewModel.IsPaneOpen = false;
				HamRelative.Background = (AcrylicBrush)Windows.UI.Xaml.Application.Current.Resources["LowOpacityElementAcrylicBrush"];
			}
			else
			{
				ViewModel.DisplayMode = SplitViewDisplayMode.Inline;
				ViewModel.IsPaneOpen = true;
				HamRelative.Background = (AcrylicBrush)Windows.UI.Xaml.Application.Current.Resources["SystemControlChromeHighAcrylicWindowMediumBrush"];
			}
		}

		private async void MainPage_Loaded(object sender, RoutedEventArgs e)
		{
			await ViewModel.Initialize();

			if (SystemInformation.IsAppUpdated && ViewModel.IsLoggedin)
			{
				await ShowWhatsNewPopupVisiblity();
			}
			if (ViewModel.UnreadNotifications != null && ViewModel.UnreadNotifications.Count > 0)
			{
				ViewModel.UnreadNotifications.Add(ViewModel.UnreadNotifications.First());
			}

			//if (ViewModel.UnreadNotifications != null && ViewModel.UnreadNotifications.Count > 0)
			//{
			//	foreach (var n in ViewModel.UnreadNotifications.Reverse())
			//	{
			//		await ViewModel.ShowToast(n, Windows.UI.Notifications.ToastNotificationScenario.Reminder);
			//	}
			//}
		}

		#region click events
		private void HamListView_ItemClick(object sender, ItemClickEventArgs e)
		{
			ViewModel.HamItemClicked(e.ClickedItem as HamItem);
		}
		private async void AccountsButton_Click(object sender, RoutedEventArgs e)
		{
			await ShowAccountsPanel();
		}
		private async void CloseAccountsPanel_Tapped(object sender, RoutedEventArgs e)
		{
			await AccountsPanel.StartCompositionFadeScaleAnimationAsync(1, 0, 1, 1.1f, 150, null, 0, EasingFunctionNames.SineEaseInOut);
			ViewModel.IsAccountsPanelVisible = false;
		}
		private async void DeleteAccount_Click(object sender, RoutedEventArgs e)
		{
			await ViewModel.DeleteAccount(((Button)sender).Tag.ToString());
		}
		#endregion

		#region Messaging
		public void RecieveLocalNotificationMessage(LocalNotificationMessageType notif)
		{
			notifManager.Show(new SimpleNotification
			{
				TimeSpan = TimeSpan.FromSeconds(3),
				Text = notif.Message,
				Glyph = notif.Glyph,
				Action = async () => await new MessageDialog(notif.Message).ShowAsync(),
				Background = GetSolidColorBrush("60B53BFF"),
				Foreground = GetSolidColorBrush("FAFBFCFF")
			},
			LocalNotificationCollisionBehaviour.Replace
			);
		}
		#endregion

		#region other methods

		public async void SetHeadertext(string pageName)
		{
			await HeaderAnimationSemaphore.WaitAsync();
			if (ViewModel.HeaderText?.Equals(pageName.ToUpper()) != true)
			{
				await HeaderText.StartCompositionFadeSlideAnimationAsync(1, 0, TranslationAxis.Y, 0, -24, 150, null, null, EasingFunctionNames.Linear);
				ViewModel.HeaderText = pageName.ToUpper();
				await HeaderText.StartCompositionFadeSlideAnimationAsync(0, 1, TranslationAxis.Y, 24, 0, 150, null, null, EasingFunctionNames.Linear);
			}
			HeaderAnimationSemaphore.Release();
		}

		private void SystemNavigationManager_BackRequested(object sender, BackRequestedEventArgs e)
		{
			IAsyncNavigationService service = SimpleIoc.Default.GetInstance<IAsyncNavigationService>();
			if (service != null && !e.Handled)
			{
				e.Handled = true;
				service.GoBackAsync();
			}
		}
		private async Task ShowAccountsPanel()
		{
			AccountsPanel.SetVisualOpacity(0);
			ViewModel.IsAccountsPanelVisible = true;
			await AccountsPanel.StartCompositionFadeScaleAnimationAsync(0, 1, 1.1f, 1, 150, null, 0, EasingFunctionNames.SineEaseInOut);
		}
		private async Task ShowWhatsNewPopupVisiblity()
		{
			WhatsNewPopup.SetVisualOpacity(0);
			WhatsNewPopup.Visibility = Visibility.Visible;
			await WhatsNewPopup.StartCompositionFadeScaleAnimationAsync(0, 1, 1.3f, 1, 160, null, 0, EasingFunctionNames.SineEaseInOut);
		}
		#endregion
	}
}
