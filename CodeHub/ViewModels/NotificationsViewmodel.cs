using CodeHub.Helpers;
using CodeHub.Services;
using CodeHub.Views;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using Octokit;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.UI.Xaml.Controls;
using static CodeHub.Helpers.GlobalHelper;
using Task = System.Threading.Tasks.Task;
using ToastNotificationManager = Windows.UI.Notifications.ToastNotificationManager;

namespace CodeHub.ViewModels
{
    public class NotificationsViewmodel : AppViewmodel
    {
        #region properties
        public static ObservableCollection<Notification> AllNotifications { get; set; }

        public static ObservableCollection<Notification> ParticipatingNotifications { get; set; }

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

        public NotificationsViewmodel()
        {
            UnreadNotifications = UnreadNotifications ?? new ObservableCollection<Notification>();
            AllNotifications = AllNotifications ?? new ObservableCollection<Notification>();
            ParticipatingNotifications = ParticipatingNotifications ?? new ObservableCollection<Notification>();
        }

        public async Task Load()
        {
            if (!IsInternet())
            {
                //Sending NoInternet message to all viewModels
                Messenger.Default.Send(new NoInternet().SendMessage());
            }
            else
            {
                IsLoadingUnread = true;
                await LoadUnreadNotifications();
                IsLoadingUnread = false;
                IsLoadingAll = true;
                await LoadAllNotifications();
                IsLoadingAll = false;
                IsloadingParticipating = true;
                await LoadParticipatingNotifications();
                IsloadingParticipating = false;
            }
        }

        public async void RefreshAll()
        {
            if (!IsInternet())
            {
                //Sending NoInternet message to all viewModels
                Messenger.Default.Send(new NoInternet().SendMessage());
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
            if (!IsInternet())
            {
                //Sending NoInternet message to all viewModels
                Messenger.Default.Send(new NoInternet().SendMessage());
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

            if (!IsInternet())
            {
                //Sending NoInternet message to all viewModels
                Messenger.Default.Send(new NoInternet().SendMessage());
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
            if (!IsInternet())
            {
                //Sending NoInternet message to all viewModels
                Messenger.Default.Send(new NoInternet().SendMessage());
            }
            else
            {
                IsLoadingAll = IsLoadingUnread = IsloadingParticipating = true;
                await NotificationsService.MarkAllNotificationsAsRead();
                IsLoadingAll = IsLoadingUnread = IsloadingParticipating = false;
                Messenger.Default.Send(new UpdateAllNotificationsCountMessageType
                {
                    Count = 0
                });
            }
        }

        public void RecieveSignOutMessage(SignOutMessageType empty)
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
            await BackgroundTaskService.LoadAllNotifications();
        }

        private async Task LoadUnreadNotifications()
        {
            await BackgroundTaskService.LoadUnreadNotifications();
        }

        private async Task LoadParticipatingNotifications()
        {
            await BackgroundTaskService.LoadParticipatingNotifications();
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
            var isIssue = notif.Subject.Type.ToLower().Equals("issue");
            Issue issue = null;
            PullRequest pr = null;
            if (isIssue)
            {
                if (int.TryParse(notif.Subject.Url.Split('/').Last().Split('?')[0], out int id))
                {
                    issue = await IssueUtility.GetIssue(notif.Repository.Id, id);
                    await SimpleIoc.Default.GetInstance<IAsyncNavigationService>().NavigateAsync(typeof(IssueDetailView), new System.Tuple<Repository, Issue>(notif.Repository, issue));
                }
            }
            else
            {

                if (int.TryParse(notif.Subject.Url.Split('/').Last().Split('?')[0], out int id))
                {
                    pr = await PullRequestUtility.GetPullRequest(notif.Repository.Id, id);
                    await SimpleIoc.Default.GetInstance<IAsyncNavigationService>().NavigateAsync(typeof(PullRequestDetailView), new System.Tuple<Repository, PullRequest>(notif.Repository, pr));
                }
            }
            if (notif.Unread)
            {
                await NotificationsService.MarkNotificationAsRead(notif.Id);
                var toast = await notif.BuildToast(ToastNotificationScenario.Reminder);
                ToastNotificationManager.History.Remove(toast.Tag, toast.Group);
            }
        }
    }
}
