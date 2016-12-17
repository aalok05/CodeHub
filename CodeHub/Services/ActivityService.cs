using System.Threading.Tasks;
using Octokit;
using System.Collections.ObjectModel;
using CodeHub.Helpers;

namespace CodeHub.Services
{
    class ActivityService
    {
        /// <summary>
        /// Gets public user events 
        /// </summary>
        /// <returns></returns>
        public static async Task<ObservableCollection<Activity>> getEvents()
        {
            try
            {
                ObservableCollection<Activity> events = new ObservableCollection<Activity>();
                var client = await UserDataService.getAuthenticatedClient();

                // returning first 50 items
                var firstOneFifty = new ApiOptions
                {
                    PageSize = 50,
                    PageCount = 1
                };
                var result = await client.Activity.Events.GetAllUserReceivedPublic(GlobalHelper.UserLogin, firstOneFifty);
             
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
    }
}
