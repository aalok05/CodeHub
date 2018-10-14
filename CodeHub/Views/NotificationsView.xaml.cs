using CodeHub.Services;
using CodeHub.ViewModels;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Octokit;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.UI.Xaml.Navigation;
using static CodeHub.Helpers.GlobalHelper;

namespace CodeHub.Views
{
    public sealed partial class NotificationsView : Windows.UI.Xaml.Controls.Page
    {
        public NotificationsViewmodel ViewModel;

        public NotificationsView()
        {
            InitializeComponent();

            ViewModel = new NotificationsViewmodel();
            DataContext = ViewModel;

            Messenger.Default.Register<User>(this, ViewModel.RecieveSignInMessage);
            Messenger.Default.Register<SignOutMessageType>(this, ViewModel.RecieveSignOutMessage);
            Messenger.Default.Register(this, (UpdateAllNotificationsCountMessageType n) =>
            {
                AppViewmodel.UnreadNotifications =new ObservableCollection<Notification>( AppViewmodel.UnreadNotifications.OrderBy(un => un.UpdatedAt));
                ViewModel.UpdateAllNotificationIndicator(n.Count);
                Bindings.Update();
            });
            Messenger.Default.Register(this, (UpdateUnreadNotificationsCountMessageType n) =>
            {
                ViewModel.UpdateUnreadNotificationIndicator(n.Count);
                Bindings.Update();
            });
            Messenger.Default.Register(this, (UpdateParticipatingNotificationsCountMessageType n) =>
            {
                ViewModel.UpdateParticipatingNotificationIndicator(n.Count);
                Bindings.Update();
            });
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            await ViewModel.Load();
        }


        public RelayCommand<Notification> _MarkasReadAllNotifCommand;
        public RelayCommand<Notification> MarkasReadAllNotifCommand
            => _MarkasReadAllNotifCommand
                    ?? (_MarkasReadAllNotifCommand = new RelayCommand<Notification>(
                                     async (Notification notification) =>
                                     {
                                         ViewModel.IsLoadingAll = true;
                                         if (notification.Unread)
                                         {
                                             await NotificationsService.MarkNotificationAsRead(notification.Id);

                                             var index = NotificationsViewmodel.AllNotifications.IndexOf(NotificationsViewmodel.AllNotifications.Where(p => p.Id == notification.Id).First());
                                             NotificationsViewmodel.AllNotifications[index] = await NotificationsService.GetNotificationById(notification.Id);
                                         }
                                         ViewModel.IsLoadingAll = false;
                                         Messenger.Default.Send(new UpdateAllNotificationsCountMessageType { Count = 0 });
                                     }));
        public RelayCommand<Notification> _MarkasReadUnreadNotifCommand;
        public RelayCommand<Notification> MarkasReadUnreadNotifCommand
            => _MarkasReadUnreadNotifCommand
                    ?? (_MarkasReadUnreadNotifCommand = new RelayCommand<Notification>(
                                     async (Notification notification) =>
                                     {
                                         ViewModel.IsLoadingUnread = true;
                                         if (notification.Unread)
                                         {
                                             await NotificationsService.MarkNotificationAsRead(notification.Id);

                                             var index = AppViewmodel.UnreadNotifications.IndexOf(AppViewmodel.UnreadNotifications.Where(p => p.Id == notification.Id).First());
                                             AppViewmodel.UnreadNotifications.RemoveAt(index);
                                         }
                                         ViewModel.IsLoadingUnread = false;

                                         Messenger.Default.Send(new UpdateUnreadNotificationsCountMessageType { Count = AppViewmodel.UnreadNotifications?.Count ?? 0 });
                                     }));
        public RelayCommand<Notification> _MarkasReadParticipatingNotifCommand;
        public RelayCommand<Notification> MarkasReadParticipatingNotifCommand
            => _MarkasReadParticipatingNotifCommand
                    ?? (_MarkasReadParticipatingNotifCommand = new RelayCommand<Notification>(
                                     async (Notification notification) =>
                                     {
                                         ViewModel.IsloadingParticipating = true;

                                         if (notification.Unread)
                                         {
                                             await NotificationsService.MarkNotificationAsRead(notification.Id);

                                             var index = NotificationsViewmodel
                                                            .ParticipatingNotifications
                                                            .IndexOf(
                                                                NotificationsViewmodel
                                                                    .ParticipatingNotifications
                                                                    .Where(p => p.Id == notification.Id)
                                                                    .First()
                                                            );
                                             NotificationsViewmodel.ParticipatingNotifications[index] = await NotificationsService
                                                                                                            .GetNotificationById(notification.Id);
                                         }
                                         ViewModel.IsloadingParticipating = false;
                                         Messenger.Default.Send(new UpdateParticipatingNotificationsCountMessageType { Count = NotificationsViewmodel.ParticipatingNotifications?.Count ?? 0 });
                                     }));

        public RelayCommand<Notification> _UnsubscribeAllNotifCommand;
        public RelayCommand<Notification> UnsubscribeAllNotifCommand
            => _UnsubscribeAllNotifCommand
                    ?? (_UnsubscribeAllNotifCommand = new RelayCommand<Notification>(
                                     async (Notification notification) =>
                                     {
                                         ViewModel.IsLoadingAll = true;
                                         await NotificationsService.SetThreadSubscription(notification.Id, false, true);
                                         if (notification.Unread)
                                         {
                                             await NotificationsService.MarkNotificationAsRead(notification.Id);

                                             var index = NotificationsViewmodel.AllNotifications.IndexOf(NotificationsViewmodel.AllNotifications.Where(p => (p as Notification).Id == notification.Id).First());
                                             NotificationsViewmodel.AllNotifications[index] = await NotificationsService.GetNotificationById(notification.Id);
                                         }

                                         ViewModel.IsLoadingAll = false;
                                     }));
        public RelayCommand<Notification> _UnsubscribeUnreadNotifCommand;
        public RelayCommand<Notification> UnsubscribeUnreadNotifCommand
            => _UnsubscribeUnreadNotifCommand
                    ?? (_UnsubscribeUnreadNotifCommand = new RelayCommand<Notification>(
                                     async (Notification notification) =>
                                     {
                                         ViewModel.IsLoadingUnread = true;
                                         await NotificationsService.SetThreadSubscription(notification.Id, false, true);
                                         if (notification.Unread)
                                         {
                                             await NotificationsService.MarkNotificationAsRead(notification.Id);

                                             var index = AppViewmodel.UnreadNotifications.IndexOf(AppViewmodel.UnreadNotifications.Where(p => (p as Notification).Id == notification.Id).First());
                                             AppViewmodel.UnreadNotifications.RemoveAt(index);
                                         }
                                         ViewModel.IsLoadingUnread = false;
                                     }));
        public RelayCommand<Notification> _UnsubscribeParticipatingNotifCommand;
        public RelayCommand<Notification> UnsubscribeParticipatingNotifCommand
            => _UnsubscribeParticipatingNotifCommand
                    ?? (_UnsubscribeParticipatingNotifCommand = new RelayCommand<Notification>(
                                     async (Notification notification) =>
                                     {
                                         ViewModel.IsloadingParticipating = true;
                                         await NotificationsService.SetThreadSubscription(notification.Id, false, true);
                                         if (notification.Unread)
                                         {
                                             await NotificationsService.MarkNotificationAsRead(notification.Id);

                                             var index = NotificationsViewmodel.ParticipatingNotifications.IndexOf(NotificationsViewmodel.ParticipatingNotifications.Where(p => (p as Notification).Id == notification.Id).First());
                                             NotificationsViewmodel.ParticipatingNotifications[index] = await NotificationsService.GetNotificationById(notification.Id);
                                         }
                                         ViewModel.IsloadingParticipating = false;
                                     }));


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
