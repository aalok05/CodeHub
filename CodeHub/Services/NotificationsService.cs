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

        public static async Task<ObservableCollection<Notification>> GetAllNotificationsForCurrentUser(bool all, bool participating)
        {
            try
            {
                var client = await UserUtility.GetAuthenticatedClient();
                NotificationsRequest req = new NotificationsRequest{ All = all, Participating = participating};
                ApiOptions options = new ApiOptions { PageSize = 100 };
                return new ObservableCollection<Notification>(await client.Activity.Notifications.GetAllForCurrent(req,options));
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

        public static async Task<Notification> GetNotificationById(string notificationId)
        {
            try
            {
                var client = await UserUtility.GetAuthenticatedClient();

                if(int.TryParse(notificationId, out int id))
                {
                    return await client.Activity.Notifications.Get(id);
                }
                else
                {
                    return null;
                }
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

        public static async Task MarkNotificationAsRead(string notificationId)
        {
            var client = await UserUtility.GetAuthenticatedClient();

            if (int.TryParse(notificationId, out int id))
            {
                await client.Activity.Notifications.MarkAsRead(id);
            }
        }

        public static async Task UnsubscribeFromThread(string notificationId)
        {
            var client = await UserUtility.GetAuthenticatedClient();

            if (int.TryParse(notificationId, out int id))
            {
                await client.Activity.Notifications.DeleteThreadSubscription(id);
            }

        }

    }
}
