using CodeHub.Helpers;
using CodeHub.Services;
using CodeHub.Views;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using Octokit;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;


namespace CodeHub.ViewModels
{
	public class NotificationsViewmodel : AppViewmodel
	{
		#region properties
		public ObservableCollection<Notification> _AllNotifications;
		public ObservableCollection<Notification> AllNotifications
		{
			get => _AllNotifications;
			set => Set(() => AllNotifications, ref _AllNotifications, value);
		}
		public ObservableCollection<Notification> _UnreadNotifications;
		public ObservableCollection<Notification> UnreadNotifications
		{
			get => _UnreadNotifications;
			set => Set(() => UnreadNotifications, ref _UnreadNotifications, value);
		}
		public ObservableCollection<Notification> _ParticipatingNotifications;
		public ObservableCollection<Notification> ParticipatingNotifications
		{
			get => _ParticipatingNotifications;
			set => Set(() => ParticipatingNotifications, ref _ParticipatingNotifications, value);
		}

		public bool _ZeroAllCount;
		public bool ZeroAllCount
		{
			get => _ZeroAllCount;
			set => Set(() => ZeroAllCount, ref _ZeroAllCount, value);
		}
		public bool _ZeroUnreadCount;
		public bool ZeroUnreadCount
		{
			get => _ZeroUnreadCount;
			set => Set(() => ZeroUnreadCount, ref _ZeroUnreadCount, value);
		}
		public bool _ZeroParticipatingCount;
		public bool ZeroParticipatingCount
		{
			get => _ZeroParticipatingCount;
			set => Set(() => ZeroParticipatingCount, ref _ZeroParticipatingCount, value);
		}

		public bool _isloadingAll;
		public bool IsLoadingAll
		{
			get => _isloadingAll;
			set => Set(() => IsLoadingAll, ref _isloadingAll, value);
		}

		public bool _isloadingUnread;
		public bool IsLoadingUnread
		{
			get => _isloadingUnread;
			set => Set(() => IsLoadingUnread, ref _isloadingUnread, value);
		}

		public bool _isloadingParticipating;
		public bool IsloadingParticipating
		{
			get => _isloadingParticipating;
			set => Set(() => IsloadingParticipating, ref _isloadingParticipating, value);
		}
		#endregion

		public async Task Load()
		{
			if (GlobalHelper.IsInternet())
			{
				IsLoadingUnread = true;
				await LoadUnreadNotifications();
			}
		}

		public async void RefreshAll()
		{

			if (!GlobalHelper.IsInternet())
			{
				//Sending NoInternet message to all viewModels
				Messenger.Default.Send(new GlobalHelper.NoInternet().SendMessage());
			}
			else
			{

				IsLoadingAll = true;
				await LoadAllNotifications();
				IsLoadingAll = false;
			}
		}
		public async void RefreshUnread()
		{

			if (!GlobalHelper.IsInternet())
			{
				//Sending NoInternet message to all viewModels
				Messenger.Default.Send(new GlobalHelper.NoInternet().SendMessage());
			}
			else
			{

				IsLoadingUnread = true;
				await LoadUnreadNotifications();
				IsLoadingUnread = false;
			}
		}
		public async void RefreshParticipating()
		{

			if (!GlobalHelper.IsInternet())
			{
				//Sending NoInternet message to all viewModels
				Messenger.Default.Send(new GlobalHelper.NoInternet().SendMessage());
			}
			else
			{

				IsloadingParticipating = true;
				await LoadParticipatingNotifications();
				IsloadingParticipating = false;
			}
		}

		public async void MarkAllNotificationsAsRead()
		{
			if (!GlobalHelper.IsInternet())
			{
				//Sending NoInternet message to all viewModels
				Messenger.Default.Send(new GlobalHelper.NoInternet().SendMessage());
			}
			else
			{
				IsLoadingAll = IsLoadingUnread = IsloadingParticipating = true;
				await NotificationsService.MarkAllNotificationsAsRead();
				IsLoadingAll = IsLoadingUnread = IsloadingParticipating = false;
				await LoadUnreadNotifications();
			}
		}
		public void RecieveSignOutMessage(GlobalHelper.SignOutMessageType empty)
		{
			IsLoggedin = false;
			User = null;
			AllNotifications = UnreadNotifications = ParticipatingNotifications = null;
		}
		public async void RecieveSignInMessage(User user)
		{
			if (user != null)
			{
				IsLoggedin = true;
				User = user;
				await Load();
			}
		}

		private async Task LoadAllNotifications()
		{
			AllNotifications = await NotificationsService.GetAllNotificationsForCurrentUser(true, false);
			IsLoadingAll = false;
			if (AllNotifications != null)
			{
				ZeroAllCount = (AllNotifications.Count == 0) ? true : false;
			}
		}
		private async Task LoadUnreadNotifications()
		{
			UnreadNotifications = await NotificationsService.GetAllNotificationsForCurrentUser(false, false);
			IsLoadingUnread = false;
			if (UnreadNotifications != null)
			{
				if (UnreadNotifications.Count == 0)
				{
					ZeroUnreadCount = true;
					Messenger.Default.Send(new GlobalHelper.UpdateUnreadNotificationMessageType { IsUnread = false });
				}
				else
				{
					ZeroUnreadCount = false;
					Messenger.Default.Send(new GlobalHelper.UpdateUnreadNotificationMessageType { IsUnread = true });
				}
			}
		}
		private async Task LoadParticipatingNotifications()
		{
			ParticipatingNotifications = await NotificationsService.GetAllNotificationsForCurrentUser(false, true);
			IsloadingParticipating = false;
			if (ParticipatingNotifications != null)
			{
				ZeroParticipatingCount = (ParticipatingNotifications.Count == 0) ? true : false;
			}
		}

		public async void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			var p = sender as Pivot;
			if (p.SelectedIndex == 0)
			{
				IsLoadingUnread = true;
				await LoadUnreadNotifications();
				IsLoadingUnread = false;
			}
			else if (p.SelectedIndex == 1)
			{
				IsloadingParticipating = true;
				await LoadParticipatingNotifications();
				IsloadingParticipating = false;
			}
			else if (p.SelectedIndex == 2)
			{
				IsLoadingAll = true;
				await LoadAllNotifications();
				IsLoadingAll = false;
			}
		}

		public async void NotificationsListView_ItemClick(object sender, ItemClickEventArgs e)
		{
			var notif = e.ClickedItem as Notification;
			await SimpleIoc.Default.GetInstance<IAsyncNavigationService>().NavigateAsync(typeof(RepoDetailView), notif.Repository);
			if (notif.Unread)
			{
				await NotificationsService.MarkNotificationAsRead(notif.Id);
			}
		}
	}
}
