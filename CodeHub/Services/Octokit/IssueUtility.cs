using CodeHub.Helpers;
using Octokit;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace CodeHub.Services
{
	class IssueUtility
	{
		/// <summary>
		/// Gets an Issue
		/// </summary>
		/// <param name="repoId"></param>
		/// <param name="number"></param>
		/// <returns></returns>
		public static async Task<Issue> GetIssue(long repoId, int number)
		{
			try
			{
				return await GlobalHelper.GithubClient.Issue.Get(repoId, number);
			}
			catch
			{
				return null;
			}
		}

		/// <summary>
		/// Creates a new Issue for a repository
		/// </summary>
		/// <param name="repoId"></param>
		/// <param name="newIssue"></param>
		/// <returns></returns>
		public static async Task<Issue> CreateIssue(long repoId, NewIssue newIssue)
		{
			try
			{
				return await GlobalHelper.GithubClient.Issue.Create(repoId, newIssue);
			}
			catch
			{
				return null;
			}
		}

		/// <summary>
		/// Creates a new comment on a specified issue
		/// </summary>
		/// <param name="repoId"></param>
		/// <param name="number"></param>
		/// <param name="comment"></param>
		/// <returns></returns>
		public static async Task<IssueComment> CommentOnIssue(long repoId, int number, string comment)
		{
			try
			{
				return await GlobalHelper.GithubClient.Issue.Comment.Create(repoId, number, comment);
			}
			catch
			{
				return null;
			}
		}

		/// <summary>
		/// Gets all comments for a given issue
		/// </summary>
		/// <param name="repoId"></param>
		/// <param name="number"></param>
		/// <returns></returns>
		public static async Task<ObservableCollection<IssueComment>> GetAllCommentsForIssue(long repoId, int number)
		{
			try
			{
				var comments = await GlobalHelper.GithubClient.Issue.Comment.GetAllForIssue(repoId, number);
				return new ObservableCollection<IssueComment>(comments);
			}
			catch
			{
				return null;
			}

		}

		/// <summary>
		/// Creates a new Label
		/// </summary>
		/// <param name="repoId"></param>
		/// <param name="newLabel"></param>
		/// <returns></returns>
		public static async Task<Label> CreateLabel(long repoId, NewLabel newLabel)
		{
			try
			{
				return await GlobalHelper.GithubClient.Issue.Labels.Create(repoId, newLabel);
			}
			catch
			{
				return null;
			}
		}

		/// <summary>
		/// Gets all labels in a repository
		/// </summary>
		/// <param name="repoId"></param>
		/// <returns></returns>
		public static async Task<ObservableCollection<Label>> GetAllLabelsForRepository(long repoId)
		{
			try
			{
				var labels = await GlobalHelper.GithubClient.Issue.Labels.GetAllForRepository(repoId);
				return new ObservableCollection<Label>(labels);
			}
			catch
			{
				return null;
			}
		}

		/// <summary>
		/// Removes a label from issue
		/// </summary>
		/// <param name="repoId"></param>
		/// <param name="number"></param>
		/// <param name="labelName"></param>
		/// <returns></returns>
		public static async Task RemoveLabelFromIssue(long repoId, int number, string labelName)
		{
			await GlobalHelper.GithubClient.Issue.Labels.RemoveFromIssue(repoId, number, labelName);
		}

		/// <summary>
		/// Updates an issue
		/// </summary>
		/// <param name="repoId"></param>
		/// <param name="number"></param>
		/// <param name="issueUpdate"></param>
		/// <returns></returns>
		public static async Task<Issue> EditIssue(long repoId, int number, IssueUpdate issueUpdate)
		{
			try
			{
				return await GlobalHelper.GithubClient.Issue.Update(repoId, number, issueUpdate);
			}
			catch
			{
				return null;
			}
		}
	}
}
