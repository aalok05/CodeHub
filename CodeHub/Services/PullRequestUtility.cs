using Octokit;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeHub.Services
{
    class PullRequestUtility
    {
        /// <summary>
        /// Gets a Pull Request by number
        /// </summary>
        /// <param name="repoId"></param>
        /// <param name="number"></param>
        /// <returns></returns>
        public static async Task<PullRequest> GetPullRequest(long repoId, int number)
        {
            try
            {
                var client = await UserUtility.GetAuthenticatedClient();
                return await client.PullRequest.Get(repoId, number);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Gets all commits for a specified pull request
        /// </summary>
        /// <param name="repoId"></param>
        /// <param name="number"></param>
        /// <returns></returns>
        public static async Task<ObservableCollection<PullRequestCommit>> GetAllCommitsForPullRequest(long repoId, int number)
        {
            try
            {
                var client = await UserUtility.GetAuthenticatedClient();
                var commits = await client.PullRequest.Commits(repoId, number);
                return new ObservableCollection<PullRequestCommit>(commits);
            }
            catch
            {
                return null;
            }
        }
    }
}
