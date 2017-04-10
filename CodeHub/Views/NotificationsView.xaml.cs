using CodeHub.Helpers;
using CodeHub.ViewModels;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Ioc;
using Octokit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
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
            Loading += NotificationsView_Loading;

            NavigationCacheMode = NavigationCacheMode.Required;
        }

        private void NotificationsView_Loading(FrameworkElement sender, object args)
        {
            //Listening for Sign In message
            Messenger.Default.Register<User>(this, ViewModel.RecieveSignInMessage);
            //listen for sign out message
            Messenger.Default.Register<GlobalHelper.SignOutMessageType>(this, ViewModel.RecieveSignOutMessage); 
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            Messenger.Default.Send(new GlobalHelper.SetHeaderTextMessageType { PageName = "Notifications" });
        }

        private void Repo_Click(object sender, RoutedEventArgs e)
        {
            SimpleIoc.Default.GetInstance<Services.IAsyncNavigationService>().NavigateAsync(typeof(RepoDetailView), "Repository", (sender as HyperlinkButton).Content);
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
                                              if(notification.Unread)
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
                                              await NotificationsService.MarkNotificationAsRead(notification.Id);

                                              var index = ViewModel.UnreadNotifications.IndexOf(ViewModel.UnreadNotifications.Where(p => p.Id == notification.Id).First());
                                              ViewModel.UnreadNotifications.RemoveAt(index);

                                              ViewModel.IsLoadingUnread = false;
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

                                              if(notification.Unread)
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
                                              await NotificationsService.SetThreadSubscription(notification.Id,false,false);
                                              if(notification.Unread)
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
                                              await NotificationsService.SetThreadSubscription(notification.Id, false, false);
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
                                              await NotificationsService.SetThreadSubscription(notification.Id, false, false);
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
    }
}
