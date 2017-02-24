using Octokit;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace CodeHub.Services
{

    class UserUtility
    {
        /// <summary>
        /// Gets info of a given user
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        public static async Task<User> GetUserInfo(string login)
        {
            try
            {
                var client = await UserUtility.GetAuthenticatedClient();
                var user = await client.User.Get(login);
                return user;
            }
            catch
            {
                return null;
            }

        }

        /// <summary>
        /// Follows a given user
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        public static async Task<bool> FollowUser(string login)
        {
            try
            {
                var client = await UserUtility.GetAuthenticatedClient();
                return (await client.User.Followers.Follow(login));
            }
            catch
            {
                return false;
            }
        }
        /// <summary>
        /// Unfollows a given user
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        public static async Task UnFollowUser(string login)
        {
            var client = await UserUtility.GetAuthenticatedClient();
            await client.User.Followers.Unfollow(login);
        }

        /// <summary>
        /// Checks if the current user follows a given user
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        public static async Task<bool> CheckFollow(string login)
        {
            var client = await UserUtility.GetAuthenticatedClient();
            return await client.User.Followers.IsFollowingForCurrent(login);
        }

        /// <summary>
        /// Gets the authenticated GithubClient
        /// </summary>
        /// <returns></returns>
        public static async Task<GitHubClient> GetAuthenticatedClient()
        {
            try
            {
                var token = await AuthService.GetToken();
                GitHubClient client = new GitHubClient(new ProductHeaderValue("CodeHub"));
                if (token != null)
                {
                    client.Credentials = new Credentials(token);
                }
                return client;
            }
            catch
            {
                GitHubClient client = new GitHubClient(new ProductHeaderValue("CodeHub"));
                return client;
            }

        }

        /// <summary>
        /// Gets the current user info
        /// </summary>
        /// <returns></returns>
        public static async Task<User> GetCurrentUserInfo()
        {
            try
            {
                var client = await GetAuthenticatedClient();
                var user = await client.User.Current();
                return user;
            }
            catch
            {
                return null;
            }

        }

        /// <summary>
        /// Gets Email of current user
        /// </summary>
        /// <returns></returns>
        public static async Task<string> GetUserEmail()
        {
            /* If User's email is not publicly visible, the 'User' object returns null in email filed 
            *  Hence we need a separate method in such case.
            */
            try
            {
                var client = await GetAuthenticatedClient();
                var result = await client.User.Email.GetAll();
                var s = result[0].Email.ToString();
                return s;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Gets all repositories of current user
        /// </summary>
        /// <returns></returns>
        public static async Task<ObservableCollection<Repository>> GetUserRepositories()
        {
            try
            {
                var client = await GetAuthenticatedClient();
                var repos = await client.Repository.GetAllForCurrent();
                return new ObservableCollection<Repository>(new List<Repository>(repos));
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Gets all repositories starred by current user
        /// </summary>
        /// <returns></returns>
        public static async Task<ObservableCollection<Repository>> GetStarredRepositories()
        {
            try
            {
                var client = await GetAuthenticatedClient();
                var repo = await client.Activity.Starring.GetAllForCurrent();
                return new ObservableCollection<Repository>(new List<Repository>(repos));
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Gets all followers of a given user
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        public static async Task<ObservableCollection<User>> GetAllFollowers(string login)
        {
            try
            {
                ObservableCollection<User> followers = new ObservableCollection<User>();
                var client = await GetAuthenticatedClient();

                // return first 100 items
                var firstOneHundred = new ApiOptions
                {
                    PageSize = 100,
                    PageCount = 1
                };
                var result = await client.User.Followers.GetAll(login, firstOneHundred);
                foreach (User r in result)
                {
                    followers.Add(r);
                    if (followers.Count > 99)
                        break;
                }
                return followers;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Gets all users a given user is following
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        public static async Task<ObservableCollection<User>> GetAllFollowing(string login)
        {
            try
            {
                ObservableCollection<User> following = new ObservableCollection<User>();
                var client = await GetAuthenticatedClient();

                // return first 100 items
                var firstOneHundred = new ApiOptions
                {
                    PageSize = 100,
                    PageCount = 1
                };
                var result = await client.User.Followers.GetAllFollowing(login, firstOneHundred);
                foreach (User r in result)
                {
                    following.Add(r);
                }
                return following;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Gets all organizations for current user
        /// </summary>
        public static async Task<ObservableCollection<Organization>> GetAllOrganizations()
        {
            try
            {
                GitHubClient client = await GetAuthenticatedClient();

                var list = await client.Organization.GetAllForCurrent();

                return new ObservableCollection<Organization>(new List<Organization>(list));
            }
            catch
            {
                return null;
            }
        }
    }
}
