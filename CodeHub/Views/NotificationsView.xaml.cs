using CodeHub.Helpers;
using CodeHub.ViewModels;
using GalaSoft.MvvmLight.Messaging;
using Octokit;
using System.Linq;
using Windows.UI.Xaml.Navigation;
using GalaSoft.MvvmLight.Command;
using CodeHub.Services;

namespace CodeHub.Views
{
    public sealed partial class NotificationsView : Windows.UI.Xaml.Controls.Page
    {
        public NotificationsViewmodel ViewModel;

        public NotificationsView()
        {
            this.InitializeComponent();

            ViewModel = new NotificationsViewmodel();
            this.DataContext = ViewModel;

            Messenger.Default.Register<User>(this, ViewModel.RecieveSignInMessage);
            Messenger.Default.Register<GlobalHelper.SignOutMessageType>(this, ViewModel.RecieveSignOutMessage);
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            await ViewModel.Load();
        }


        public RelayCommand<Notification> _MarkasReadAllNotifCommand;
        public RelayCommand<Notification> MarkasReadAllNotifCommand
        {
            get
            {
                return _MarkasReadAllNotifCommand
                    ?? (_MarkasReadAllNotifCommand = new RelayCommand<Notification>(
                                          async (Notification notification) =>
                                          {
                                              ViewModel.IsLoadingAll = true;
                                              if (notification.Unread)
                                              {
                                                  await NotificationsService.MarkNotificationAsRead(notification.Id);

                                                  var index = ViewModel.AllNotifications.IndexOf(ViewModel.AllNotifications.Where(p => p.Id == notification.Id).First());
                                                  ViewModel.AllNotifications[index] = await NotificationsService.GetNotificationById(notification.Id);
                                              }
                                              ViewModel.IsLoadingAll = false;
                                          }));
            }
        }
        public RelayCommand<Notification> _MarkasReadUnreadNotifCommand;
        public RelayCommand<Notification> MarkasReadUnreadNotifCommand
        {
            get
            {
                return _MarkasReadUnreadNotifCommand
                    ?? (_MarkasReadUnreadNotifCommand = new RelayCommand<Notification>(
                                          async (Notification notification) =>
                                          {
                                              ViewModel.IsLoadingUnread = true;
                                              if (notification.Unread)
                                              {
                                                  await NotificationsService.MarkNotificationAsRead(notification.Id);

                                                  var index = ViewModel.UnreadNotifications.IndexOf(ViewModel.UnreadNotifications.Where(p => p.Id == notification.Id).First());
                                                  ViewModel.UnreadNotifications.RemoveAt(index);
                                              }
                                              ViewModel.IsLoadingUnread = false;

                                              if (ViewModel.UnreadNotifications.Count == 0)
                                                  Messenger.Default.Send(new GlobalHelper.UpdateUnreadNotificationMessageType { IsUnread = false });
                                          }));
            }
        }
        public RelayCommand<Notification> _MarkasReadParticipatingNotifCommand;
        public RelayCommand<Notification> MarkasReadParticipatingNotifCommand
        {
            get
            {
                return _MarkasReadParticipatingNotifCommand
                    ?? (_MarkasReadParticipatingNotifCommand = new RelayCommand<Notification>(
                                          async (Notification notification) =>
                                          {
                                              ViewModel.IsloadingParticipating = true;

                                              if (notification.Unread)
                                              {
                                                  await NotificationsService.MarkNotificationAsRead(notification.Id);

                                                  var index = ViewModel.ParticipatingNotifications.IndexOf(ViewModel.ParticipatingNotifications.Where(p => p.Id == notification.Id).First());
                                                  ViewModel.ParticipatingNotifications[index] = await NotificationsService.GetNotificationById(notification.Id);
                                              }
                                              ViewModel.IsloadingParticipating = false;
                                          }));
            }
        }

        public RelayCommand<Notification> _UnsubscribeAllNotifCommand;
        public RelayCommand<Notification> UnsubscribeAllNotifCommand
        {
            get
            {
                return _UnsubscribeAllNotifCommand
                    ?? (_UnsubscribeAllNotifCommand = new RelayCommand<Notification>(
                                          async (Notification notification) =>
                                          {
                                              ViewModel.IsLoadingAll = true;
                                              await NotificationsService.SetThreadSubscription(notification.Id, false, true);
                                              if (notification.Unread)
                                              {
                                                  await NotificationsService.MarkNotificationAsRead(notification.Id);

                                                  var index = ViewModel.AllNotifications.IndexOf(ViewModel.AllNotifications.Where(p => p.Id == notification.Id).First());
                                                  ViewModel.AllNotifications[index] = await NotificationsService.GetNotificationById(notification.Id);
                                              }

                                              ViewModel.IsLoadingAll = false;
                                          }));
            }
        }
        public RelayCommand<Notification> _UnsubscribeUnreadNotifCommand;
        public RelayCommand<Notification> UnsubscribeUnreadNotifCommand
        {
            get
            {
                return _UnsubscribeUnreadNotifCommand
                    ?? (_UnsubscribeUnreadNotifCommand = new RelayCommand<Notification>(
                                          async (Notification notification) =>
                                          {
                                              ViewModel.IsLoadingUnread = true;
                                              await NotificationsService.SetThreadSubscription(notification.Id, false, true);
                                              if (notification.Unread)
                                              {
                                                  await NotificationsService.MarkNotificationAsRead(notification.Id);

                                                  var index = ViewModel.UnreadNotifications.IndexOf(ViewModel.UnreadNotifications.Where(p => p.Id == notification.Id).First());
                                                  ViewModel.UnreadNotifications.RemoveAt(index);
                                              }
                                              ViewModel.IsLoadingUnread = false;
                                          }));
            }
        }
        public RelayCommand<Notification> _UnsubscribeParticipatingNotifCommand;
        public RelayCommand<Notification> UnsubscribeParticipatingNotifCommand
        {
            get
            {
                return _UnsubscribeParticipatingNotifCommand
                    ?? (_UnsubscribeParticipatingNotifCommand = new RelayCommand<Notification>(
                                          async (Notification notification) =>
                                          {
                                              ViewModel.IsloadingParticipating = true;
                                              await NotificationsService.SetThreadSubscription(notification.Id, false, true);
                                              if (notification.Unread)
                                              {
                                                  await NotificationsService.MarkNotificationAsRead(notification.Id);

                                                  var index = ViewModel.ParticipatingNotifications.IndexOf(ViewModel.ParticipatingNotifications.Where(p => p.Id == notification.Id).First());
                                                  ViewModel.ParticipatingNotifications[index] = await NotificationsService.GetNotificationById(notification.Id);
                                              }
                                              ViewModel.IsloadingParticipating = false;
                                          }));
            }
        }


        //private void MarkasReadUnreadNotif_Invoked(SwipeItem sender, SwipeItemInvokedEventArgs args)
        //{
        //    ViewModel.IsLoadingUnread = true;
        //    if (notification.Unread)
        //    {
        //        await NotificationsService.MarkNotificationAsRead(notification.Id);

        //        var index = ViewModel.UnreadNotifications.IndexOf(ViewModel.UnreadNotifications.Where(p => p.Id == notification.Id).First());
        //        ViewModel.UnreadNotifications.RemoveAt(index);
        //    }
        //    ViewModel.IsLoadingUnread = false;

        //    if (ViewModel.UnreadNotifications.Count == 0)
        //        Messenger.Default.Send(new GlobalHelper.UpdateUnreadNotificationMessageType { IsUnread = false });
        //}
        //private void MarkasReadParticipatingNotif_Invoked(SwipeItem sender, SwipeItemInvokedEventArgs args)
        //{
        //    ViewModel.IsloadingParticipating = true;

        //    if (notification.Unread)
        //    {
        //        await NotificationsService.MarkNotificationAsRead(notification.Id);

        //        var index = ViewModel.ParticipatingNotifications.IndexOf(ViewModel.ParticipatingNotifications.Where(p => p.Id == notification.Id).First());
        //        ViewModel.ParticipatingNotifications[index] = await NotificationsService.GetNotificationById(notification.Id);
        //    }
        //    ViewModel.IsloadingParticipating = false;
        //}
        //private void MarkasReadAllNotif_Invoked(SwipeItem sender, SwipeItemInvokedEventArgs args)
        //{
        //    ViewModel.IsLoadingAll = true;
        //    if (notification.Unread)
        //    {
        //        await NotificationsService.MarkNotificationAsRead(notification.Id);

        //        var index = ViewModel.AllNotifications.IndexOf(ViewModel.AllNotifications.Where(p => p.Id == notification.Id).First());
        //        ViewModel.AllNotifications[index] = await NotificationsService.GetNotificationById(notification.Id);
        //    }
        //    ViewModel.IsLoadingAll = false;
        //}
        //private void UnsubscribeUnreadNotif_Invoked(SwipeItem sender, SwipeItemInvokedEventArgs args)
        //{
        //    ViewModel.IsLoadingUnread = true;
        //    await NotificationsService.SetThreadSubscription(notification.Id, false, true);
        //    if (notification.Unread)
        //    {
        //        await NotificationsService.MarkNotificationAsRead(notification.Id);

        //        var index = ViewModel.UnreadNotifications.IndexOf(ViewModel.UnreadNotifications.Where(p => p.Id == notification.Id).First());
        //        ViewModel.UnreadNotifications.RemoveAt(index);
        //    }
        //    ViewModel.IsLoadingUnread = false;
        //}
        //private void UnsubscribeParticipatingNotif_Invoked(SwipeItem sender, SwipeItemInvokedEventArgs args)
        //{
        //    ViewModel.IsloadingParticipating = true;
        //    await NotificationsService.SetThreadSubscription(notification.Id, false, true);
        //    if (notification.Unread)
        //    {
        //        await NotificationsService.MarkNotificationAsRead(notification.Id);

        //        var index = ViewModel.ParticipatingNotifications.IndexOf(ViewModel.ParticipatingNotifications.Where(p => p.Id == notification.Id).First());
        //        ViewModel.ParticipatingNotifications[index] = await NotificationsService.GetNotificationById(notification.Id);
        //    }
        //    ViewModel.IsloadingParticipating = false;
        //}
        //private void UnsubscribeAllNotif_Invoked(SwipeItem sender, SwipeItemInvokedEventArgs args)
        //{
        //    ViewModel.IsLoadingAll = true;
        //    await NotificationsService.SetThreadSubscription(notification.Id, false, true);
        //    if (notification.Unread)
        //    {
        //        await NotificationsService.MarkNotificationAsRead(notification.Id);

        //        var index = ViewModel.AllNotifications.IndexOf(ViewModel.AllNotifications.Where(p => p.Id == notification.Id).First());
        //        ViewModel.AllNotifications[index] = await NotificationsService.GetNotificationById(notification.Id);
        //    }

        //    ViewModel.IsLoadingAll = false;
        //}

    }
}
