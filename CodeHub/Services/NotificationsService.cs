using CodeHub.Helpers;
using Octokit;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace CodeHub.Services
{
	class NotificationsService
	{
		/// <summary>
		/// Gets all Notifications for the current user
		/// </summary>
		/// <param name="all"></param>
		/// <param name="participating"></param>
		/// <returns></returns>
		public static async Task<ObservableCollection<Notification>> GetAllNotificationsForCurrentUser(bool all, bool participating)
		{
			try
			{
				var req = new NotificationsRequest { All = all, Participating = participating };
				var options = new ApiOptions { PageSize = 100, PageCount = 1 };
				return new ObservableCollection<Notification>(await GlobalHelper.GithubClient.Activity.Notifications.GetAllForCurrent(req, options));
			}
			catch
			{
				return null;
			}

		}

		/// <summary>
		///  Gets all Notifications for a repository
		/// </summary>
		/// <param name="repoId"></param>
		/// <returns></returns>
		public static async Task<ObservableCollection<Notification>> GetAllNotificationsForRepository(long repoId)
		{
			try
			{
				return new ObservableCollection<Notification>(await GlobalHelper.GithubClient.Activity.Notifications.GetAllForRepository(repoId));
			}
			catch
			{
				return null;
			}
		}

		/// <summary>
		/// Gets a notification
		/// </summary>
		/// <param name="notificationId"></param>
		/// <returns></returns>
		public static async Task<Notification> GetNotificationById(string notificationId)
		{
			try
			{
				if (int.TryParse(notificationId, out int id))
				{
					return await GlobalHelper.GithubClient.Activity.Notifications.Get(id);
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

		/// <summary>
		/// Marks all notifications as read
		/// </summary>
		/// <returns></returns>
		public static async Task MarkAllNotificationsAsRead() 
			=> await GlobalHelper.GithubClient.Activity.Notifications.MarkAsRead();

		/// <summary>
		/// Marks a notification as read
		/// </summary>
		/// <param name="notificationId"></param>
		/// <returns></returns>
		public static async Task MarkNotificationAsRead(string notificationId)
		{
			if (int.TryParse(notificationId, out int id))
			{
				await GlobalHelper.GithubClient.Activity.Notifications.MarkAsRead(id);
			}
		}

		/// <summary>
		/// Sets the user's subscription settings for a given thread
		/// </summary>
		/// <param name="notificationId"></param>
		/// <param name="subscribed"></param>
		/// <param name="ignored"></param>
		/// <returns></returns>
		public static async Task SetThreadSubscription(string notificationId, bool subscribed, bool ignored)
		{
			if (int.TryParse(notificationId, out int id))
			{
				await GlobalHelper.GithubClient.Activity.Notifications.SetThreadSubscription(id,
				    new NewThreadSubscription
				    {
					    Subscribed = subscribed,
					    Ignored = ignored
				    });
			}

		}

		/// <summary>
		/// Gets a ThreadSubscription
		/// </summary>
		/// <param name="notificationId"></param>
		/// <returns></returns>
		public static async Task<ThreadSubscription> GetSubscribtionThread(string notificationId)
		{
			if (int.TryParse(notificationId, out int id))
			{
				return await GlobalHelper.GithubClient.Activity.Notifications.GetThreadSubscription(id);
			}
			return null;
		}
	}
}
