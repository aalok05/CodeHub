using Octokit;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeHub.Services
{
    class IssueUtility
    {
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
                var client = await UserUtility.GetAuthenticatedClient();
                return await client.Issue.Create(repoId, newIssue);
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
                var client = await UserUtility.GetAuthenticatedClient();
                return await client.Issue.Comment.Create(repoId, number, comment);
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
                var client = await UserUtility.GetAuthenticatedClient();
                var comments = await client.Issue.Comment.GetAllForIssue(repoId, number);
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
                var client = await UserUtility.GetAuthenticatedClient();
                return await client.Issue.Labels.Create(repoId, newLabel);
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
                var client = await UserUtility.GetAuthenticatedClient();
                var labels = await client.Issue.Labels.GetAllForRepository(repoId);
                return new ObservableCollection<Label>(labels);
            }
            catch
            {
                return null;
            }
        }
    }
}
