using Windows.ApplicationModel.Background;
using Windows.UI.Notifications;

namespace BackgroundTasks
{
	public sealed class ToastNotificationHistoryChangedTriggerTask : IBackgroundTask
	{
		public void Run(IBackgroundTaskInstance taskInstance)
		{
			if (!(taskInstance.TriggerDetails is ToastNotificationHistoryChangedTriggerDetail details))
			{
				return;
			}

			// We send back the change type, the UI listens to the progress and parses the change type
			taskInstance.Progress = (uint)details.ChangeType;
		}
	}
}
