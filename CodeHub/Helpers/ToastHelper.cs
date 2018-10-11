using CodeHub.Services;
using System;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.Foundation.Collections;
using Windows.Foundation.Metadata;
using Windows.UI.Notifications;

namespace CodeHub.Helpers
{

    public enum ToastNotificationScenario
    {
        Default = 0,
        Alarm = 1,
        Reminder = 2,
        IncomingCall = 3
    }

    public static class ToastHelper
    {
        public static ToastNotification PopToast(string title, string content)
            => PopToast(title, content, null, null);

        public static ToastNotification PopToast(string title, string content, string tag, string group)
        {
            var xml = $@"<toast activationType='foreground'>
                                            <visual>
                                                <binding template='ToastGeneric'>
                                                </binding>
                                            </visual>
                                        </toast>";

            var doc = new XmlDocument();
            doc.LoadXml(xml);

            var binding = doc.SelectSingleNode("//binding");

            var el = doc.CreateElement("text");
            el.InnerText = title;

            binding.AppendChild(el);

            el = doc.CreateElement("text");
            el.InnerText = content;
            binding.AppendChild(el);

            return PopCustomToast(doc, tag, group);
        }

        public static ToastNotification PopCustomToast(string xml)
            => PopCustomToast(xml, null, null);

        public static ToastNotification PopCustomToast(string xml, string tag, string group)
        {
            var doc = new XmlDocument();
            doc.LoadXml(xml);

            return PopCustomToast(doc, tag, group);
        }

        public static ToastNotification PopCustomToast(ToastNotification notification)
        {
            return PopCustomToast(notification, null, null);
        }
        public static ToastNotification PopCustomToast(ToastNotification toast, string tag, string group)
        {
            if (!StringHelper.IsNullOrEmptyOrWhiteSpace(tag))
            {
                toast.Tag = tag;
            }

            if (!StringHelper.IsNullOrEmptyOrWhiteSpace(group))
            {
                toast.Group = group;
            }
            ToastNotificationManager.CreateToastNotifier().Show(toast);
            return toast;
        }

        [DefaultOverload]
        public static ToastNotification PopCustomToast(XmlDocument doc)
        {
            return PopCustomToast(new ToastNotification(doc), null, null);
        }

        public static ToastNotification PopCustomToast(XmlDocument doc, string tag, string group)
        {
            var toast = new ToastNotification(doc);
            return PopCustomToast(toast, tag, group);
        }

        public static string ToString(ValueSet valueSet)
        {
            var builder = new StringBuilder();

            foreach (var pair in valueSet)
            {
                if (builder.Length != 0)
                {
                    builder.Append('\n');
                }

                builder.Append(pair.Key);
                builder.Append(": ");
                builder.Append(pair.Value);
            }

            return builder.ToString();
        }

        public static async Task<ToastNotification> PopCustomToast(Octokit.Notification notification, ToastNotificationScenario scenario = ToastNotificationScenario.Default)
        {
            var toast = await notification.BuildToast(scenario);
            return PopCustomToast(toast, toast.Tag, toast.Group);
        }

        public static async Task<Octokit.Notification> GetNotification(this ToastNotification toast)
        {
            if (toast == null)
            {
                throw new ArgumentNullException(nameof(toast));
            }

            if (!StringHelper.IsNullOrEmptyOrWhiteSpace(toast.Tag))
            {
                var notificationId = toast.Tag.Split('+')[0];
                if (notificationId.Length == 0)
                {
                    throw new ArgumentException("Invalid notificationId");
                }
                notificationId = notificationId.Substring(1, notificationId.Length - 1);

                return await NotificationsService.GetNotificationById(notificationId);
            }
            else
            {
                return null;
            }
        }
    }
}
