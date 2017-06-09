using Octokit;
using System;
using System.Collections.Generic;
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
    }
}
