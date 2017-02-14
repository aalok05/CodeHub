using System.Threading.Tasks;
using Octokit;
using System.Collections.ObjectModel;
using CodeHub.Helpers;

namespace CodeHub.Services
{
    class ActivityService
    {
        /// <summary>
        /// Gets all public events of current user
        /// </summary>
        /// <returns></returns>
        public static async Task<ObservableCollection<Activity>> GetUserActivity()
        {
            try
            {
                var client = await UserUtility.GetAuthenticatedClient();

                var options = new ApiOptions
                {
                    PageSize = 50,
                    PageCount = 1
                };
                var result = await client.Activity.Events.GetAllUserReceivedPublic(GlobalHelper.UserLogin, options);

                return new ObservableCollection<Activity>(result);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Gets all public events of a given user
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        public static async Task<ObservableCollection<Activity>> GetUserPerformedActivity(string login)
        {
            try
            {
                var client = await UserUtility.GetAuthenticatedClient();

                var options = new ApiOptions
                {
                    PageSize = 10,
                    PageCount = 1
                };
                var result = await client.Activity.Events.GetAllUserPerformedPublic(login, options);

                return new ObservableCollection<Activity>(result);
            }
            catch
            {
                return null;
            }
        }
    }
}
