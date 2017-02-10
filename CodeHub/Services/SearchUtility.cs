using Octokit;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace CodeHub.Services
{
    class SearchUtility
    {
        public static async Task<ObservableCollection<Repository>> SearchRepos(string query)
        {
            try
            {
                var client = await UserUtility.GetAuthenticatedClient();
                var request = new SearchRepositoriesRequest(query);
                var result = await client.Search.SearchRepo(request);
                ObservableCollection<Repository> repos = new ObservableCollection<Repository>();
                foreach (Repository r in result.Items)
                {
                    repos.Add(r);
                }
                return repos;
            }
            catch
            {
                return null;
            }

        }
        public static async Task<ObservableCollection<SearchCode>> SearchCode(string query)
        {
            try
            {
                var client = await UserUtility.GetAuthenticatedClient();
                var request = new SearchCodeRequest(query);
                var result = await client.Search.SearchCode(request);
                ObservableCollection<SearchCode> codes = new ObservableCollection<SearchCode>();
                foreach (SearchCode r in result.Items)
                {
                    codes.Add(r);
                }
                return codes;
            }
            catch
            {
                return null;
            }

        }
        public static async Task<ObservableCollection<User>> SearchUsers(string query)
        {
            try
            {
                var client = await UserUtility.GetAuthenticatedClient();
                var request = new SearchUsersRequest(query);
                var result = await client.Search.SearchUsers(request);
                ObservableCollection<User> users = new ObservableCollection<User>();
                foreach (User r in result.Items)
                {
                    users.Add(r);
                }
                return users;
            }
            catch
            {
                return null;
            }

        }
        public static async Task<ObservableCollection<Issue>> SearchIssues(string query)
        {
            try
            {
                var client = await UserUtility.GetAuthenticatedClient();
                var request = new SearchIssuesRequest(query);
                var result = await client.Search.SearchIssues(request);
                ObservableCollection<Issue> issues = new ObservableCollection<Issue>();
                foreach (Issue r in result.Items)
                {
                    issues.Add(r);
                }
                return issues;
            }
            catch
            {
                return null;
            }

        }
    }
}
