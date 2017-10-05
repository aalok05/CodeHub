using CodeHub.Helpers;
using Octokit;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeHub.Services
{
    class CommitService
    {
        public async static Task<GitHubCommit> GetCommit(long repoId, string SHA)
        {
            try
            {
                return await GlobalHelper.GithubClient.Repository.Commit.Get(repoId, SHA);
            }
            catch
            {
                return null;
            }
        }
        public async static Task<ObservableCollection<CommitComment>> GetAllCommentsForCommit(long repoId, string SHA)
        {
            try
            {
                return new ObservableCollection<CommitComment>(await GlobalHelper.GithubClient.Repository.Comment.GetAllForCommit(repoId, SHA));
            }
            catch
            {
                return null;
            }
        }
    }
}
