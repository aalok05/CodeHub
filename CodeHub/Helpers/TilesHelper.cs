using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

namespace CodeHub.Helpers
{
    public enum TilesBindingHintPresentation
    {
        Contacts,
        People,
        Photos
    }
    public enum TilesTextStyles
    {
        Base,
        BaseSubtle,
        Body,
        BodySubtle,
        Caption,
        CaptionSubtle,
        Header,
        HeaderNumeral,
        HeaderSubtle,
        SubHeader,
        SubHeaderNumeral,
        SubHeaderSubtle,
        Subtitle,
        SubtitleSubtle,
        Title,
        TitleSubtle
    }
    public enum TilesVisualBrandings
    {
        Default,
        None,
        Logo,
        Name,
        NameAndLogo,
    }

    public static class TilesHelper
    {
        public static void UpdateTile(TileNotification tile)
        {
            UpdateTile(tile, null);
        }

        public static void UpdateTile(TileNotification tile, string tag)
        {
            if (!string.IsNullOrEmpty(tag))
            {
                tile.Tag = tag;
            }

            TileUpdateManager.CreateTileUpdaterForApplication().Update(tile);
        }

        public static void UpdateTile(string tileXml)
        {
            UpdateTile(tileXml, null);
        }

        public static void UpdateTile(string tileXml, string tag)
        {
            var doc = new XmlDocument();
            doc.LoadXml(tileXml);
            UpdateTile(new TileNotification(doc), tag);
        }

        public static void UpdateTile(XmlDocument badgeXml)
        {
            UpdateTile(new TileNotification(badgeXml));
        }

        public static void UpdateTile(XmlDocument badgeXml, string tag)
        {
            UpdateTile(new TileNotification(badgeXml), tag);
        }

        public static async Task UpdateTile(Octokit.Notification notification)
        {
            var tile = await notification.BuildTiles(TilesTextStyles.Base, TilesTextStyles.SubtitleSubtle,
                 TilesTextStyles.Body, TilesVisualBrandings.NameAndLogo);
            UpdateTile(tile, tile.Tag);
        }
    }
}
