using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking.Connectivity;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace CodeHub.Helpers
{
    public class GlobalHelper
    {
        /*
        *  Message types are used for Viewodel to ViewModel communication
        */
        #region Message Types
        public class SignOutMessageType
        {

        }
        public class NoInternetMessageType
        {

        }
        public class HasInternetMessageType
        {

        }
        public class SetHeaderTextMessageType
        {
           public string PageName { get; set; }
        }
        public class FollowActivityMessageType
        {

        }
        #endregion

        /*The Username (login) of the Authenticated user is available throughout the app to make calls for User's data*/
        public static string UserLogin { get; set; }

        /// <summary>
        /// Indicates whether user has performed a new Star/Unstar action. Used to update starred repositories
        /// </summary>
        public static bool NewStarActivity { get; set; }

        /// <summary>
        /// Indicates whether user has performed a new Follow/UnFollow action. Used to update followers
        /// </summary>
        public static bool NewFollowActivity { get; set; }
        public static bool IsInternet()
        {
            var connectionProfile = NetworkInformation.GetInternetConnectionProfile();
            return (connectionProfile != null &&
                                        connectionProfile.GetNetworkConnectivityLevel() ==
                                        NetworkConnectivityLevel.InternetAccess);
               
        }
        public static SolidColorBrush GetSolidColorBrush(string hex)
            {
                byte r = (byte)(Convert.ToUInt32(hex.Substring(0, 2), 16));
                byte g = (byte)(Convert.ToUInt32(hex.Substring(2, 2), 16));
                byte b = (byte)(Convert.ToUInt32(hex.Substring(4, 2), 16));
                byte a = (byte)(Convert.ToUInt32(hex.Substring(6, 2), 16));
                SolidColorBrush myBrush = new SolidColorBrush(Color.FromArgb(a, r, g, b));
                return myBrush;
            }
        
    }

}