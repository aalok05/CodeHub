using Octokit;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeHub.Services
{
    class NotificationsService
    {

        public static async Task<ObservableCollection<Notification>> GetAllNotificationsForCurrentUser()
        {
            try
            {
                var client = await UserUtility.GetAuthenticatedClient();
                return new ObservableCollection<Notification>(await client.Activity.Notifications.GetAllForCurrent());
            }
            catch
            {
                return null;
            }

        }

        public static async Task<ObservableCollection<Notification>> GetAllNotificationsForRepository(long repoId)
        {
            try
            {
                var client = await UserUtility.GetAuthenticatedClient();
                return new ObservableCollection<Notification>(await client.Activity.Notifications.GetAllForRepository(repoId));
            }
            catch
            {
                return null;
            }
        }

        public static async Task<Notification> GetNotificationById(int notificationId)
        {
            try
            {
                var client = await UserUtility.GetAuthenticatedClient();
                return await client.Activity.Notifications.Get(notificationId);
            }
            catch
            {
                return null;
            }

        }

        public static async Task MarkAllNotificationsAsRead()
        {
            var client = await UserUtility.GetAuthenticatedClient();
            await client.Activity.Notifications.MarkAsRead();
        }

        public static async Task MarkNotificationAsRead(int notificationId)
        {
            var client = await UserUtility.GetAuthenticatedClient();
            await client.Activity.Notifications.MarkAsRead(notificationId);
        }

    }
}
