using CodeHub.Helpers;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeHub.Services;
using System.Collections.ObjectModel;
using Octokit;
using Windows.UI.Xaml.Controls;
using GalaSoft.MvvmLight.Ioc;
using CodeHub.Views;

namespace CodeHub.ViewModels
{
    public class NotificationsViewmodel : AppViewmodel
    {
        #region properties
        public ObservableCollection<Notification> _AllNotifications;
        public ObservableCollection<Notification> AllNotifications
        {
            get
            {
                return _AllNotifications;
            }
            set
            {
                Set(() => AllNotifications, ref _AllNotifications, value);
            }
        }
        public ObservableCollection<Notification> _UnreadNotifications;
        public ObservableCollection<Notification> UnreadNotifications
        {
            get
            {
                return _UnreadNotifications;
            }
            set
            {
                Set(() => UnreadNotifications, ref _UnreadNotifications, value);
            }
        }
        public ObservableCollection<Notification> _ParticipatingNotifications;
        public ObservableCollection<Notification> ParticipatingNotifications
        {
            get
            {
                return _ParticipatingNotifications;
            }
            set
            {
                Set(() => ParticipatingNotifications, ref _ParticipatingNotifications, value);
            }
        }

        public bool _ZeroAllCount;
        public bool ZeroAllCount
        {
            get
            {
                return _ZeroAllCount;
            }
            set
            {
                Set(() => ZeroAllCount, ref _ZeroAllCount, value);
            }
        }
        public bool _ZeroUnreadCount;
        public bool ZeroUnreadCount
        {
            get
            {
                return _ZeroUnreadCount;
            }
            set
            {
                Set(() => ZeroUnreadCount, ref _ZeroUnreadCount, value);
            }
        }
        public bool _ZeroParticipatingCount;
        public bool ZeroParticipatingCount
        {
            get
            {
                return _ZeroParticipatingCount;
            }
            set
            {
                Set(() => ZeroParticipatingCount, ref _ZeroParticipatingCount, value);
            }
        }

        public bool _isloadingAll;
        public bool IsLoadingAll
        {
            get
            {
                return _isloadingAll;
            }
            set
            {
                Set(() => IsLoadingAll, ref _isloadingAll, value);

            }
        }

        public bool _isloadingUnread;
        public bool IsLoadingUnread
        {
            get
            {
                return _isloadingUnread;
            }
            set
            {
                Set(() => IsLoadingUnread, ref _isloadingUnread, value);

            }
        }

        public bool _isloadingParticipating;
        public bool IsloadingParticipating
        {
            get
            {
                return _isloadingParticipating;
            }
            set
            {
                Set(() => IsloadingParticipating, ref _isloadingParticipating, value);

            }
        }
        #endregion

        public RelayCommand _loadCommand;
        public RelayCommand LoadCommand
        {
            get
            {
                return _loadCommand
                    ?? (_loadCommand = new RelayCommand(
                                          async () =>
                                          {

                                              if (!GlobalHelper.IsInternet())
                                              {
                                                  //Sending NoInternet message to all viewModels
                                                  Messenger.Default.Send(new GlobalHelper.LocalNotificationMessageType { Message = "No Internet", Glyph = "\uE704" });
                                              }
                                              else
                                              {
                                                  IsLoadingUnread = true;
                                                  await LoadUnreadNotifications();
                                              }
                                          }));
            }
        }

        public async void RefreshAll()
        {

            if (!GlobalHelper.IsInternet())
            {
                //Sending NoInternet message to all viewModels
                Messenger.Default.Send(new GlobalHelper.LocalNotificationMessageType { Message = "No Internet", Glyph = "\uE704" });
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
                Messenger.Default.Send(new GlobalHelper.LocalNotificationMessageType { Message = "No Internet", Glyph = "\uE704" });
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
                Messenger.Default.Send(new GlobalHelper.LocalNotificationMessageType { Message = "No Internet", Glyph = "\uE704" });
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
                Messenger.Default.Send(new GlobalHelper.LocalNotificationMessageType { Message = "No Internet", Glyph = "\uE704" });
            }
            else
            {
                IsLoadingAll = IsLoadingUnread = IsloadingParticipating = true;
                await NotificationsService.MarkAllNotificationsAsRead();                
                IsLoadingAll = IsLoadingUnread = IsloadingParticipating = false;
                await LoadUnreadNotifications();

                Messenger.Default.Send(new GlobalHelper.CheckNotificationMessageType());
            }
        }
        public void RecieveSignOutMessage(GlobalHelper.SignOutMessageType empty)
        {
            isLoggedin = false;
            User = null;
            AllNotifications = UnreadNotifications = ParticipatingNotifications = null;
        }
        public void RecieveSignInMessage(User user)
        {
            if (user != null)
            {
                isLoggedin = true;
                User = user;
                LoadCommand.Execute(null);
            }
        }

        private async Task LoadAllNotifications()
        {
            AllNotifications = await NotificationsService.GetAllNotificationsForCurrentUser(true,false);
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
                ZeroUnreadCount = (UnreadNotifications.Count == 0) ? true : false;
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
            Pivot p = sender as Pivot;
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
            else if(p.SelectedIndex == 2)
            {
                IsLoadingAll = true;
                await LoadAllNotifications();
                IsLoadingAll = false;
            }
        }

        public async void NotificationsListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            Notification notif = e.ClickedItem as Notification;
            SimpleIoc.Default.GetInstance<IAsyncNavigationService>().NavigateAsync(typeof(RepoDetailView), "Repository", notif.Repository.FullName);
            if (notif.Unread)
            {
                await NotificationsService.MarkNotificationAsRead(notif.Id);
            }
        }
    }
}
