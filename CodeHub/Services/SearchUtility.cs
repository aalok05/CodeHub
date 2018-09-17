using CodeHub.Helpers;
using Octokit;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace CodeHub.Services
{
	class SearchUtility
	{
		/// <summary>
		/// Searches repositories 
		/// </summary>
		/// <param name="query"></param>
		/// <returns></returns>
		public static async Task<ObservableCollection<Repository>> SearchRepos(string query, Language? language = null)
		{
			try
			{
				var request = new SearchRepositoriesRequest(query);
				if (language != null)
				{
					request.Language = language;
				}
				var result = await GlobalHelper.GithubClient.Search.SearchRepo(request);
				return new ObservableCollection<Repository>(result.Items);
			}
			catch
			{
				return null;
			}

		}

		/// <summary>
		/// Searches code
		/// </summary>
		/// <param name="query"></param>
		/// <returns></returns>
		public static async Task<ObservableCollection<SearchCode>> SearchCode(string query, Language? language = null)
		{
			try
			{
				var request = new SearchCodeRequest(query);
				if (language != null)
				{
					request.Language = language;
				}
				var result = await GlobalHelper.GithubClient.Search.SearchCode(request);
				return new ObservableCollection<SearchCode>(result.Items);
			}
			catch
			{
				return null;
			}

		}

		/// <summary>
		/// Searches users
		/// </summary>
		/// <param name="query"></param>
		/// <returns></returns>
		public static async Task<ObservableCollection<User>> SearchUsers(string query, Language? language = null)
		{
			try
			{
				var request = new SearchUsersRequest(query);
				if (language != null)
				{
					request.Language = language;
				}
				var result = await GlobalHelper.GithubClient.Search.SearchUsers(request);
				return new ObservableCollection<User>(result.Items);
			}
			catch
			{
				return null;
			}

		}

		/// <summary>
		/// Searches issues
		/// </summary>
		/// <param name="query"></param>
		/// <returns></returns>
		public static async Task<ObservableCollection<Issue>> SearchIssues(string query, Language? language = null)
		{
			try
			{
				var request = new SearchIssuesRequest(query);
				if (language != null)
				{
					request.Language = language;
				}
				var result = await GlobalHelper.GithubClient.Search.SearchIssues(request);
				return new ObservableCollection<Issue>(result.Items);
			}
			catch
			{
				return null;
			}

		}

		//public static async Task SearchInRepo(string query)
		//{
		//    try
		//    {
		//        try
		//        {
		//            var request = new SearchIssuesRequest(query);

		//            var result = await GlobalHelper.GithubClient.Search.
		//            return new ObservableCollection<Issue>(result.Items);
		//        }
		//        catch
		//        {
		//            return null;
		//        }
		//    }
		//    catch
		//    {
		//        return null;
		//    }
		//}
	}
}
