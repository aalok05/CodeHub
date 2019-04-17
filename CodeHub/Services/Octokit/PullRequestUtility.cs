using CodeHub.Helpers;
using Octokit;
using System.Collections.ObjectModel;
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
				return await GlobalHelper.GithubClient.PullRequest.Get(repoId, number);
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
				var commits = await GlobalHelper.GithubClient.PullRequest.Commits(repoId, number);
				return new ObservableCollection<PullRequestCommit>(commits);
			}
			catch
			{
				return null;
			}
		}
	}
}
