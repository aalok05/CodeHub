using System;
using System.Linq;
using Windows.ApplicationModel.Background;

namespace CodeHub.Helpers
{
    public static class BackgroundTaskHelper
    {
        public static BackgroundTaskBuilder BuildBackgroundTask(string name, IBackgroundTrigger trigger, params IBackgroundCondition[] conditions)
        {
            BackgroundTaskBuilder builder = null;

            var taskExists = BackgroundTaskRegistration.AllTasks.Any(i => i.Value.Name == name);
            TimeTrigger timeTrigger = null;
            MaintenanceTrigger maintenanceTrigger = null;

            if (taskExists)
            {
                if (trigger is TimeTrigger tTrigger)
                {
                    timeTrigger = tTrigger;
                }

                if (trigger is MaintenanceTrigger mTrigger)
                {
                    maintenanceTrigger = mTrigger;
                }

                if (timeTrigger == null && maintenanceTrigger == null)
                {
                    var taskDic = BackgroundTaskRegistration.AllTasks.SingleOrDefault(i => i.Value.Name == name);
                    taskDic.Value.Unregister(true);
                }
            }

            // Specify the background task
            builder = new BackgroundTaskBuilder()
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
