using CodeHub.Helpers;
using Octokit;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace CodeHub.Services
{
	class ActivityService
	{
		/// <summary>
		/// Gets all public events of a given user
		/// </summary>
		/// <param name="login"></param>
		/// <returns></returns>
		public static async Task<ObservableCollection<Activity>> GetUserPerformedActivity(string login)
		{
			try
			{
				var options = new ApiOptions
				{
					PageSize = 30,
					PageCount = 1
				};
				var result = await GlobalHelper.GithubClient.Activity.Events.GetAllUserPerformedPublic(login, options);

				return new ObservableCollection<Activity>(result);
			}
			catch
			{
				return null;
			}
		}
	}
}
