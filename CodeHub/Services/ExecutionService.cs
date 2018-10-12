using Microsoft.Toolkit.Uwp.Helpers;
using System;
using Windows.ApplicationModel.Background;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.ExtendedExecution;
using Windows.Foundation;
using Windows.UI.Core;

namespace CodeHub.Services
{
    static class ExecutionService
    {
        public static async void RunActionAsExtentedAction(this ExtendedExecutionSession session, Action action, TypedEventHandler<object, ExtendedExecutionRevokedEventArgs> revoked, BackgroundTaskDeferral deferral = null)
        {
            if (session is null)
            {
                throw new NullReferenceException($"'{nameof(session)} can not be null'");
            }
            var result = await session?.RequestExtensionAsync();
            if (result == ExtendedExecutionResult.Allowed)
            {
                action();
            }
            session.Revoked -= revoked;
            session.Dispose();
            session = null;

            if (deferral != null)
            {
                deferral.Complete();
            }
        }

        public static async void RunActionInCoreWindow(DispatchedHandler handler, BackgroundTaskDeferral deferral = null)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, handler);

            if (deferral != null)
            {
                deferral.Complete();
            }
        }

        public static async void RunActionInUiThread(Action action, BackgroundTaskDeferral deferral = null)
        {
            await DispatcherHelper.ExecuteOnUIThreadAsync(action, CoreDispatcherPriority.Normal);

            if (deferral != null)
            {
                deferral.Complete();
            }
        }
    }
}
