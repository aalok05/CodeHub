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

        /// <summary>
        /// Gets all comments for a given PR
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="name"></param>
        /// <param name="number"></param>
        /// <returns></returns>
        public static async Task<ObservableCollection<PullRequestReviewComment>> GetAllCommentsForPullRequest(long repoId, int number)
        {
            try
            {
                var client = await UserUtility.GetAuthenticatedClient();
                var comments = await client.PullRequest.ReviewComment.GetAll(repoId, number);
                return new ObservableCollection<PullRequestReviewComment>(comments);
            }
            catch
            {
                return null;
            }

        }
    }
}
