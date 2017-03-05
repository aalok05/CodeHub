using Octokit;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace CodeHub.Services
{
    class SearchUtility
    {
        /// <summary>
        /// Searches repositories 
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public static async Task<ObservableCollection<Repository>> SearchRepos(string query)
        {
            try
            {
                var client = await UserUtility.GetAuthenticatedClient();
                var request = new SearchRepositoriesRequest(query);
                var result = await client.Search.SearchRepo(request);
                return new ObservableCollection<Repository>(new List<Repository>(result.Items));
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
        public static async Task<ObservableCollection<SearchCode>> SearchCode(string query)
        {
            try
            {
                var client = await UserUtility.GetAuthenticatedClient();
                var request = new SearchCodeRequest(query);
                var result = await client.Search.SearchCode(request);
                return new ObservableCollection<SearchCode>(new List<SearchCode>(result.Items));
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
        public static async Task<ObservableCollection<User>> SearchUsers(string query)
        {
            try
            {
                var client = await UserUtility.GetAuthenticatedClient();
                var request = new SearchUsersRequest(query);
                var result = await client.Search.SearchUsers(request);
                return new ObservableCollection<User>(new List<User>(result.Items));
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
        public static async Task<ObservableCollection<Issue>> SearchIssues(string query)
        {
            try
            {
                var client = await UserUtility.GetAuthenticatedClient();
                var request = new SearchIssuesRequest(query);
                var result = await client.Search.SearchIssues(request);
                return new ObservableCollection<Issue>(new List<Issue>(result.Items));
            }
            catch
            {
                return null;
            }

        }
    }
}
