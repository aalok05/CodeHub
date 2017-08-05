using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking.Connectivity;
using Windows.System.Profile;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;

namespace CodeHub.Helpers
{
    public class GlobalHelper
    {
        /*
        *  Message types are used for Viewodel to ViewModel communication
        */
        #region Message Types

        public class AdsEnabledMessageType {}

        public class SignOutMessageType {}

        public class HasInternetMessageType {}

        public class HostWindowBlurMessageType {}

        public class ShowWhatsNewPopupMessageType {}

        public class LocalNotificationMessageType
        {
            public string Message { get; set; }
            public string Glyph { get; set; }
        }
        public class SetHeaderTextMessageType
        {
            public string PageName { get; set; }
        }

        public class UpdateUnreadNotificationMessageType
        {
            public bool IsUnread { get; set; }
        }
        #endregion

        /// <summary>
        /// Client for GitHub client
        /// </summary>
        public static GitHubClient GithubClient { get; set; }

        /// <summary>
        /// Maintains a stack of page titles
        /// </summary>
        public static Stack<string> NavigationStack { get; set; } =  new Stack<string>();

        /// <summary>
        /// Username of the Authenticated user 
        /// </summary>
        public static string UserLogin { get; set; }

        /// <summary>
        /// Indicates whether user has performed a new Star/Unstar action. Used to update starred repositories
        /// </summary>
        public static bool NewStarActivity { get; set; }

        /// <summary>
        /// List of names and owner names of Trending repositories
        /// </summary>
        public static List<Tuple<string, string>> TrendingTodayRepoNames { get; set; }

        /// <summary>
        /// List of names and owner names of Trending repositories
        /// </summary>
        public static List<Tuple<string, string>> TrendingWeekRepoNames { get; set; }

        /// <summary>
        /// List of names and owner names of Trending repositories
        /// </summary>
        public static List<Tuple<string, string>> TrendingMonthRepoNames { get; set; }

        /// <summary>
        /// Determines if internet connection is available to device
        /// </summary>
        /// <returns></returns>
        public static bool IsInternet()
        {
            var connectionProfile = NetworkInformation.GetInternetConnectionProfile();
            return (connectionProfile != null &&
                                        connectionProfile.GetNetworkConnectivityLevel() ==
                                        NetworkConnectivityLevel.InternetAccess);

        }

        /// <summary>
        /// Converts a Hex string to corressponding SolidColorBrush
        /// </summary>
        /// <param name="hex">rrggbbaa</param>
        /// <returns></returns>
        public static SolidColorBrush GetSolidColorBrush(string hex)
        {
            byte r = (byte)(Convert.ToUInt32(hex.Substring(0, 2), 16));
            byte g = (byte)(Convert.ToUInt32(hex.Substring(2, 2), 16));
            byte b = (byte)(Convert.ToUInt32(hex.Substring(4, 2), 16));
            byte a = (byte)(Convert.ToUInt32(hex.Substring(6, 2), 16));
            SolidColorBrush myBrush = new SolidColorBrush(Color.FromArgb(a, r, g, b));
            return myBrush;
        }

        public static Geometry GetGeomtery(string path)
        {
            var sym = "<Geometry xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\">"+path+"</Geometry>";
            return (Geometry)XamlReader.Load(sym);
        }

        /// <summary>
        /// Gets the OS build
        /// </summary>
        /// <returns></returns>
        public static ulong GetOSBuild()
        {
            string deviceFamilyVersion = AnalyticsInfo.VersionInfo.DeviceFamilyVersion;
            ulong version = ulong.Parse(deviceFamilyVersion);
            //ulong major = (version & 0xFFFF000000000000L) >> 48;
            //ulong minor = (version & 0x0000FFFF00000000L) >> 32;
            ulong build = (version & 0x00000000FFFF0000L) >> 16;
            //ulong revision = (version & 0x000000000000FFFFL);
            //var osVersion = $"{major}.{minor}.{build}.{revision}";
            return build;
        }

        /// <summary>
        /// Coverts a DateTime to 'time ago' format
        /// </summary>
        /// <returns></returns>
        public static string ConvertDateToTimeAgoFormat(DateTime dt)
        {
            var ts = new TimeSpan(DateTime.Now.Ticks - dt.Ticks);
            double delta = Math.Abs(ts.TotalSeconds);

            if (delta < 60)
            {
                return ts.Seconds == 1 ? "one second ago" : ts.Seconds + " seconds ago";
            }
            if (delta < 120)
            {
                return "a minute ago";
            }
            if (delta < 2700) // 45 * 60
            {
                return ts.Minutes + " minutes ago";
            }
            if (delta < 5400) // 90 * 60
            {
                return "an hour ago";
            }
            if (delta < 86400) // 24 * 60 * 60
            {
                return ts.Hours + " hours ago";
            }
            if (delta < 172800) // 48 * 60 * 60
            {
                return "a day ago";
            }
            if (delta < 2592000) // 30 * 24 * 60 * 60
            {
                return ts.Days + " days ago";
            }
            if (delta < 31104000) // 12 * 30 * 24 * 60 * 60
            {
                int months = System.Convert.ToInt32(Math.Floor((double)ts.Days / 30));
                return months <= 1 ? "one month ago" : months + " months ago";
            }
            int years = System.Convert.ToInt32(Math.Floor((double)ts.Days / 365));
            return years <= 1 ? "one year ago" : years + " years ago";
        }
    }

}