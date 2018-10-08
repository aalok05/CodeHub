using System;
using System.Linq;
using Windows.ApplicationModel.Background;

namespace CodeHub.Helpers
{
    public static class BackgroundTaskHelper
    {

        public static void UnregisterAllBackgroundTasks()
        {
            foreach (var task in BackgroundTaskRegistration.AllTasks)
            {
                if (task.Value.Name == "SyncNotifications" || task.Value.Name == "SyncNotifications" || task.Value.Name == "ToastNotificationBackgroundTask")
                {
                    task.Value.Unregister(true);
                    break;
                }
            }
        }
        public static BackgroundTaskBuilder BuildBackgroundTask(string name, IBackgroundTrigger trigger, params IBackgroundCondition[] conditions)
        {
            // Specify the background task
            var builder = new BackgroundTaskBuilder()
            {
                Name = name
            };

            // Set the trigger for Listener
            builder.SetTrigger(trigger);

            if (conditions != null && conditions.Length > 0)
            {
                foreach (var condition in conditions)
                {
                    builder.AddCondition(condition);
                }
            }

            return builder;
        }

        public static BackgroundTaskBuilder BuildBackgroundTask(string name, Type entryPointType, IBackgroundTrigger trigger, params IBackgroundCondition[] conditions)
        {
            var builder = BuildBackgroundTask(name, trigger, conditions);
            builder.TaskEntryPoint = entryPointType.FullName;
            return builder;
        }

        public static BackgroundTaskBuilder BuildBackgroundTask<T>(string name, IBackgroundTrigger trigger, params IBackgroundCondition[] conditions)
            where T : IBackgroundTask
        {
            var builder = BuildBackgroundTask(name, trigger, conditions);
            builder.TaskEntryPoint = typeof(T).FullName;
            return builder;
        }
        public static BackgroundTaskBuilder BuildBackgroundTask(string name, string entryPointName, IBackgroundTrigger trigger, params IBackgroundCondition[] conditions)
        {
            var builder = BuildBackgroundTask(name, trigger, conditions);
            builder.TaskEntryPoint = entryPointName;
            return builder;
        }
    }
}
