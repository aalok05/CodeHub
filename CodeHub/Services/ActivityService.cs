using System.Threading.Tasks;
using Octokit;
using System.Collections.ObjectModel;
using CodeHub.Helpers;

namespace CodeHub.Services
{
    class ActivityService
    {
        /// <summary>
        /// Gets public activities of authenticated user
        /// </summary>
        /// <returns></returns>
        public static async Task<ObservableCollection<Activity>> GetUserActivity()
        {
            try
            {
                ObservableCollection<Activity> events = new ObservableCollection<Activity>();
                var client = await UserDataService.getAuthenticatedClient();

                var options = new ApiOptions
                {
                    PageSize = 50,
                    PageCount = 1
                };
                var result = await client.Activity.Events.GetAllUserReceivedPublic(GlobalHelper.UserLogin, options);
             
                foreach ( var i in result)
                {
                    events.Add(i);
                }
                return events;
                
            }
            catch
            {
                return null;
            }
        }
        public static async Task<ObservableCollection<Activity>> GetUserPerformedActivity(string login)
        {
            try
            {
                ObservableCollection<Activity> events = new ObservableCollection<Activity>();
                var client = await UserDataService.getAuthenticatedClient();

                var options = new ApiOptions
                {
                    PageSize = 10,
                    PageCount = 1
                };
                var result = await client.Activity.Events.GetAllUserPerformedPublic(login, options);

                foreach (var i in result)
                {
                    events.Add(i);
                }
                return events;
            }
            catch
            {
                return null;
            }
        }
    }
}
