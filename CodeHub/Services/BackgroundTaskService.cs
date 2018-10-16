using CodeHub.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Foundation.Collections;
using Windows.Storage;

namespace CodeHub.Services
{
    internal static class BackgroundTaskService
    {
        #region private members
        #region Fields
        private static ApplicationTrigger AppTrigger;
        private static ApplicationTriggerResult _allResult;
        private static ApplicationTriggerResult _participatingResult;
        private static ApplicationTriggerResult _unreadResult;
        #endregion

        #region methods
        private static BackgroundTaskBuilder BuildTask(BackgroundTaskBuilderModel model, string taskEntryPointName, bool isNetworkRequested = false, bool cancelOnConditionLoss = true, BackgroundTaskRegistrationGroup group = null)
        {
            // Specify the background task
            var builder = BuildTask(model, isNetworkRequested, cancelOnConditionLoss, group);
            builder.TaskEntryPoint = taskEntryPointName;
            return builder;
        }

        private static void GenerateDefault<T>(this ICollection<BackgroundTaskBuilder> tasks, ref T[] values)
        {
            if (tasks != null && tasks.Count > 0)
            {

                values = new T[tasks.Count];
                int i = 0;
                foreach (var t in tasks)
                {
                    if (values[i] is bool boolValue)
                    {
                        boolValue = false;
                    }
                    else if (values[i] is BackgroundTaskRegistrationGroup groupValue)
                    {
                        groupValue = null;
                    }
                    else
                    {
                        values[i] = default;
                    }
                    i++;
                }
            }
        }

        private static void GenerateDefault<T>(this ICollection<BackgroundTaskBuilderModel> tasks, ref T[] values)
        {
            if (tasks != null && tasks.Count > 0)
            {

                values = new T[tasks.Count];
                int i = 0;
                foreach (var t in tasks)
                {
                    if (values[i] is bool boolValue)
                    {
                        boolValue = false;
                    }
                    else if (values[i] is BackgroundTaskRegistrationGroup groupValue)
                    {
                        groupValue = null;
                    }
                    else
                    {
                        values[i] = default;
                    }
                    i++;
                }
            }
        }

        private static void ResetAppDataForAppTriggertask()
        {
            ApplicationData.Current.LocalSettings.Values.Remove("AppTrigger");
        }

        private static async Task<ApplicationTriggerResult> RunAppTrigger(ValueSet valueSet, bool resetAppData = true)
        {
            if (resetAppData)
            {
                ResetAppDataForAppTriggertask();
            }

            return await GetAppTrigger()?.RequestAsync(valueSet);
        }

        private static void Validate<T>(this ICollection<BackgroundTaskBuilder> tasks, ref T[] array)
        {
            if (tasks == null)
            {
                throw new ArgumentNullException(nameof(tasks));
            }

            if (tasks.Count > 0)
            {
                if (array == null)
                {
                    tasks.GenerateDefault(ref array);
                }
                if (tasks.Count != array.Length)
                {
                    throw new IndexOutOfRangeException($"Length of {nameof(tasks)} and {nameof(array)} must be same");
                }
            }
        }

        private static void Validate<T>(this ICollection<BackgroundTaskBuilderModel> tasks, ref T[] array)
        {
            if (tasks == null)
            {
                throw new ArgumentNullException(nameof(tasks));
            }

            if (tasks.Count > 0)
            {
                if (array == null)
                {
                    tasks.GenerateDefault(ref array);
                }
                if (tasks.Count != array.Length)
                {
                    throw new IndexOutOfRangeException($"Length of {nameof(tasks)} and {nameof(array)} must be same");
                }
            }
        }
        #endregion
        #endregion

        #region Internal Members
        #region Methods   
        internal static async Task<ApplicationTriggerResult> RunAppTriggerBackgroundTask(string action, string what, string location, string filter, string type, bool sendMessage = false, bool resetAppData = true)
        {
            var valueSet = new ValueSet
            {
                { "action", action},
                { "what", what },
                { "location",  location },
                { "filter", filter },
                { "type", type },
                { "sendMessage",  sendMessage }
            };

            return await RunAppTrigger(valueSet, resetAppData);
        }

        internal static void RegisterBackgroundTasks()
        {
            IBackgroundCondition internetAvailableCondition = new SystemCondition(SystemConditionType.InternetAvailable),
                                 userPresentCondition = new SystemCondition(SystemConditionType.UserPresent),
                                 sessionConnectedCondition = new SystemCondition(SystemConditionType.SessionConnected),
                                 backgroundCostNotHighCondition = new SystemCondition(SystemConditionType.BackgroundWorkCostNotHigh);

            var conditions = new[] {
                internetAvailableCondition,
                // userPresentCondition,
                //sessionConnectedCondition
            };

            var bgBuilderModel = new BackgroundTaskBuilderModel(
                                    "ToastNotificationAction",
                                    new ToastNotificationActionTrigger(),
                                    conditions
                                 );
            var toastActionTask = BuildTask(bgBuilderModel, true, true, null);
            toastActionTask.Register(true, false, true);

            bgBuilderModel = new BackgroundTaskBuilderModel(
                                "SyncNotifications",
                                new TimeTrigger(15, false),
                                conditions
                             );
            var syncTask = BuildTask(bgBuilderModel, true, true, null);
            syncTask.Register(true, false, true);
        }
        #endregion
        #endregion

        #region Public Members
        #region Methods
        public static BackgroundTaskBuilder BuildTask(BackgroundTaskBuilderModel model, bool isNetworkRequested = false, bool cancelOnConditionLoss = true, BackgroundTaskRegistrationGroup group = null)
        {
            // Specify the background task
            var builder = new BackgroundTaskBuilder()
            {
                Name = model.Name
            };
            if (model.Trigger != null)
            {
                builder.SetTrigger(model.Trigger);
            }

            builder.IsNetworkRequested = isNetworkRequested;
            builder.CancelOnConditionLoss = cancelOnConditionLoss;
            if (group != null)
            {
                builder.TaskGroup = group;
            }

            var conditions = model.GetConditions();
            if (conditions.Count > 0)
            {
                foreach (var condition in conditions)
                {
                    builder.AddCondition(condition);
                }
            }

            return builder;
        }

        public static BackgroundTaskBuilder BuildTask(BackgroundTaskBuilderModel model, Type entryPointType, bool isNetworkRequested = false, bool cancelOnConditionLoss = true, BackgroundTaskRegistrationGroup group = null)
        {
            return BuildTask(model, entryPointType.FullName, isNetworkRequested, cancelOnConditionLoss, group);
        }

        public static BackgroundTaskBuilder BuildTask<T>(BackgroundTaskBuilderModel model, bool isNetworkRequested = false, bool cancelOnConditionLoss = true, BackgroundTaskRegistrationGroup group = null)
            where T : IBackgroundTask
        {
            return BuildTask(model, typeof(T).FullName, isNetworkRequested, cancelOnConditionLoss, group);
        }

        public static IEnumerable<BackgroundTaskBuilder> BuildTasks(ICollection<BackgroundTaskBuilderModel> backgroundTasks, bool[] isNetworkRequested = null, bool[] cancelOnConditionLoss = null, BackgroundTaskRegistrationGroup[] groups = null)
        {
            if (backgroundTasks == null)
            {
                throw new ArgumentNullException(nameof(backgroundTasks));
            }

            if (backgroundTasks.Count > 0)
            {
                backgroundTasks.Validate(ref isNetworkRequested);
                backgroundTasks.Validate(ref cancelOnConditionLoss);
                backgroundTasks.Validate(ref groups);
                int i = 0;
                foreach (var task in backgroundTasks)
                {
                    yield return BuildTask(task, isNetworkRequested[i], cancelOnConditionLoss[i], groups[i]);
                    i++;
                }
            }
        }

        public static IEnumerable<BackgroundTaskBuilder> BuildTasks(ICollection<BackgroundTaskBuilderModel> backgroundTasks, Type[] entrypointTypes, bool[] isNetworkRequested = null, bool[] cancelOnConditionLoss = null, BackgroundTaskRegistrationGroup[] groups = null)
        {
            if (backgroundTasks == null)
            {
                throw new ArgumentNullException(nameof(backgroundTasks));
            }

            if (backgroundTasks.Count > 0)
            {
                backgroundTasks.Validate(ref entrypointTypes);
                backgroundTasks.Validate(ref isNetworkRequested);
                backgroundTasks.Validate(ref cancelOnConditionLoss);
                backgroundTasks.Validate(ref groups);
                int i = 0;
                foreach (var task in backgroundTasks)
                {
                    yield return BuildTask(task, entrypointTypes[i], isNetworkRequested[i], cancelOnConditionLoss[i], groups[i]);
                    i++;
                }
            }
        }

        public static IEnumerable<BackgroundTaskBuilder> BuildTasks<T>(ICollection<BackgroundTaskBuilderModel> backgroundTasks, T[] entrypoints, bool[] isNetworkRequested = null, bool[] cancelOnConditionLoss = null, BackgroundTaskRegistrationGroup[] groups = null)
            where T : IBackgroundTask
        {
            if (backgroundTasks == null)
            {
                throw new ArgumentNullException(nameof(backgroundTasks));
            }

            if (backgroundTasks.Count > 0)
            {
                backgroundTasks.Validate(ref entrypoints);
                backgroundTasks.Validate(ref isNetworkRequested);
                backgroundTasks.Validate(ref cancelOnConditionLoss);
                backgroundTasks.Validate(ref groups);
                int i = 0;
                foreach (var task in backgroundTasks)
                {
                    yield return BuildTask(task, entrypoints[i].GetType(), isNetworkRequested[i], cancelOnConditionLoss[i], groups[i]);
                    i++;
                }
            }
        }

        public static ref ApplicationTrigger GetAppTrigger()
        {
            if (AppTrigger == null)
            {
                AppTrigger = new ApplicationTrigger();
            }

            return ref AppTrigger;
        }

        public static async Task LoadAllNotifications(bool reset = true, BackgroundTaskDeferral deferral = null)
        {
            reset = reset || (_allResult == ApplicationTriggerResult.Allowed
                     && _allResult == ApplicationTriggerResult.CurrentlyRunning);
            if (_allResult != ApplicationTriggerResult.CurrentlyRunning)
            {
                _allResult = await RunAppTriggerBackgroundTask("sync", "notifications", "online", "all", "toast", true, reset);
            }

            deferral?.Complete();
        }

        public static async Task LoadParticipatingNotifications(bool reset = true, BackgroundTaskDeferral deferral = null)
        {
            reset = reset || (_participatingResult == ApplicationTriggerResult.Allowed
                     && _participatingResult == ApplicationTriggerResult.CurrentlyRunning);
            if (_participatingResult != ApplicationTriggerResult.CurrentlyRunning)
            {
                _participatingResult = await RunAppTriggerBackgroundTask("sync", "notifications", "online", "participating", "toast", true, reset);
            }

            deferral?.Complete();
        }

        public static async Task LoadUnreadNotifications(bool reset = true, bool showToasts = true, BackgroundTaskDeferral deferral = null)
        {
            reset = reset || (_unreadResult == ApplicationTriggerResult.Allowed
                     && _unreadResult == ApplicationTriggerResult.CurrentlyRunning);
            if (_unreadResult != ApplicationTriggerResult.CurrentlyRunning)
            {
                _unreadResult = await RunAppTriggerBackgroundTask("sync", "notifications", "online", "unread", "toast", true, reset);
            }

            deferral?.Complete();
        }

        public static IBackgroundTaskRegistration Register(this BackgroundTaskBuilder builder, bool unregister = true, bool all = true, bool cancelTask = true)
        {
            if (unregister)
            {
                builder.Unregister(all, cancelTask);
            }

            return builder.Register();
        }

        public static IBackgroundTaskRegistration Register(this BackgroundTaskBuilder builder, ref ApplicationTrigger trigger, bool unregister = true, bool all = true, bool cancelTask = true)
        {
            if (unregister)
            {
                builder.Unregister(all, cancelTask);
            }

            builder.SetTrigger(trigger);
            return builder.Register();
        }

        public static IEnumerable<IBackgroundTaskRegistration> Register(this ICollection<BackgroundTaskBuilder> builders, bool unregister = true, bool all = true, bool cancelTask = true)
        {
            foreach (var builder in builders)
            {
                yield return builder.Register(unregister, all, cancelTask);
            }
        }

        public static IEnumerable<IBackgroundTaskRegistration> Register(this ICollection<BackgroundTaskBuilder> builders, ApplicationTrigger[] triggers, bool unregister = true, bool all = true, bool cancelTask = true)
        {
            builders.Validate(ref triggers);
            int i = 0;
            foreach (var builder in builders)
            {
                builder.SetTrigger(triggers[i]);
                yield return builder.Register(unregister, all, cancelTask);
                i++;
            }
        }

        public static void RegisterAppTriggerBackgroundTasks()
        {
            IBackgroundCondition internetAvailableCondition = new SystemCondition(SystemConditionType.InternetAvailable),
                                 userPresentCondition = new SystemCondition(SystemConditionType.UserPresent),
                                 sessionConnectedCondition = new SystemCondition(SystemConditionType.SessionConnected),
                                 backgroundCostNotHighCondition = new SystemCondition(SystemConditionType.BackgroundWorkCostNotHigh);

            var conditions = new[]
            {
                    internetAvailableCondition,
                    //userPresentCondition,
                    //sessionConnectedCondition
                };

            var bgBuilderModel = new BackgroundTaskBuilderModel(
                                 "AppTrigger",
                                 conditions
                              );

            var builder = BuildTask(bgBuilderModel, true, true, null);

            builder.Register(GetAppTrigger(), all: false);
        }

        public static void Unregister(this BackgroundTaskBuilder taskBuilder, bool all = true, bool cancelTask = true)
        {
            Unregister(taskBuilder.Name, all, cancelTask);
        }

        public static void Unregister(string name, bool all = true, bool cancelTask = true)
        {
            foreach (var task in BackgroundTaskRegistration.AllTasks)
            {
                if (task.Value.Name == name)
                {
                    task.Value.Unregister(cancelTask);
                    if (!all)
                    {
                        break;
                    }
                }
            }
        }
        #endregion
        #endregion
    }
}
