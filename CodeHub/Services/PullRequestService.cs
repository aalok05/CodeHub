using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeHub.Services
{
    class PullRequestService
    {
        /// <summary>
        /// Creates a new comment on a specified Pull Request
        /// </summary>
        /// <param name="repoId"></param>
        /// <param name="number"></param>
        /// <param name="comment"></param>
        /// <returns></returns>
        public static async Task<PullRequestReviewComment> CommentOnPullRequest(long repoId, int number, PullRequestReviewCommentCreate comment)
        {
            try
            {
                var client = await UserUtility.GetAuthenticatedClient();
                return await client.PullRequest.ReviewComment.Create(repoId, number, comment);
            }
            catch
            {
                return null;
            }
        }
    }
}
