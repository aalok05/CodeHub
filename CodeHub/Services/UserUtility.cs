using Octokit;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
