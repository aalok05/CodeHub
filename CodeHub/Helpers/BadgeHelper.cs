using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

namespace CodeHub.Helpers
{
    public static class BadgeHelper
    {
        private static BadgeNotification BuildBadge(int number)
        {
            // Get the blank badge XML payload for a badge number
            var badgeXml = BadgeUpdateManager.GetTemplateContent(BadgeTemplateType.BadgeNumber);

            // Set the value of the badge in the XML to our number
            var badgeElement = badgeXml.SelectSingleNode("/badge") as XmlElement;
            badgeElement.SetAttribute("value", number.ToString());
            return new BadgeNotification(badgeXml);
        }

        public static void UpdateBadge(int number)
        {
            BadgeUpdateManager
                .CreateBadgeUpdaterForApplication()
                .Update(BuildBadge(number));
        }
    }
}
