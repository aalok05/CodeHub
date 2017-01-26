using Octokit;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media;
using CodeHub.Helpers;
using CodeHub.Models;
using JetBrains.Annotations;

namespace CodeHub.Services
{

    /* This class deals with the other users' data, not the Authenticated user.
     */
    class UserUtility
    {
        public static async Task<User> getUserInfo(string login)
        {
            try
            {
                var client = await UserDataService.getAuthenticatedClient();
                var user = await client.User.Get(login);
                return user;
            }
            catch
            {
                return null;
            }

        }

        /// <summary>
        /// Loads a classic and a blurred version of the user avatar
        /// </summary>
        /// <param name="user">The original user to wrap</param>
        /// <param name="blur">The amount of blur to use</param>
        /// <param name="token">The cancellation token for the operation</param>
        [ItemCanBeNull]
        public static async Task<Tuple<ImageSource, ImageSource>> GetDeveloperAvatarOptionsAsync([NotNull] User user, int blur, CancellationToken token)
        {
            IBuffer imageBuffer = await HTTPHelper.DownloadDataAsync(user.AvatarUrl, token);
            return imageBuffer == null ? null : await ImageHelper.GetImageAndBlurredCopyFromPixelDataAsync(imageBuffer, blur);
        }

        public static async Task<bool> FollowUser(string login)
        {
            try
            {
                var client = await UserDataService.getAuthenticatedClient();
                return(await client.User.Followers.Follow(login));
            }
            catch
            {
                return false;
            }
        }
        public static async Task UnFollowUser(string login)
        {
            var client = await UserDataService.getAuthenticatedClient();
            await client.User.Followers.Unfollow(login);
        }
        public static async Task<bool> checkFollow(string login)
        {
            var client = await UserDataService.getAuthenticatedClient();
            return await client.User.Followers.IsFollowingForCurrent(login);
        }
    }
}
