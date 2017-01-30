using CodeHub.Helpers;
using Octokit;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using static CodeHub.ViewModels.HomeViewmodel;

namespace CodeHub.Services
{
    class RepositoryUtility
    {
        /// <summary>
        /// Two calls are made to this method to emulate Incremental Loading. First call (second parameter = true) returns first 7 repositories, 
        /// Second call (second parameter = false) returns the rest
        ///</summary>
        /// <param name="range">Today, weekly or monthly</param>
        /// <param name="firstCall">Indicates if this is the first call in incremental calls or not</param>
        /// <returns>Trending Repositories in a Time range</returns>
        public static async Task<ObservableCollection<Repository>> GetTrendingRepos(TimeRange range, bool firstCall)
        {

            try
            {
                ObservableCollection<Repository> repos = new ObservableCollection<Repository>();
                var trendingReposNames = await HtmlParseService.ExtractTrendingRepos(range);

                var client = await UserDataService.getAuthenticatedClient();
               
                if (firstCall)
                {
                    for (int i = 0; i < 7; i++)
                    {
                        repos.Add(await client.Repository.Get(trendingReposNames[i].Item1, trendingReposNames[i].Item2));
                    }
                }
                else
                {
                    for (int i = 7; i < trendingReposNames.Count; i++)
                    {
                        repos.Add(await client.Repository.Get(trendingReposNames[i].Item1, trendingReposNames[i].Item2));
                    }
                }

                return repos;
            }
            catch
            {
                return null;
            }

        }
        public static async Task<ObservableCollection<string>> GetAllBranches(Repository repo)
        {
            try
            {
                var client = await UserDataService.getAuthenticatedClient();
                var branches = await client.Repository.Branch.GetAll(repo.Owner.Login, repo.Name);

                ObservableCollection<string> branchList = new ObservableCollection<string>();
                foreach (Branch i in branches)
                {
                    branchList.Add(i.Name);
                }
                return branchList;
            }
            catch
            {
                return null;
            }

        }
        public static async Task<ObservableCollection<RepositoryContent>> GetRepositoryContent(Repository repo, string branch)
        {
            try
            {
                var client = await UserDataService.getAuthenticatedClient();
                var content = await client.Repository.Content.GetAllContentsByRef(repo.Owner.Login, repo.Name, branch);


                ObservableCollection<RepositoryContent> contentList = new ObservableCollection<RepositoryContent>();
                foreach (RepositoryContent c in content)
                {
                    contentList.Add(c);
                }

                return contentList;
            }
            catch
            {
                return null;
            }

        }
        public static async Task<ObservableCollection<RepositoryContent>> GetRepositoryContentByPath(long repoId, string path, string branch)
        {
            try
            {
                var client = await UserDataService.getAuthenticatedClient();
                var content = await client.Repository.Content.GetAllContentsByRef(repoId, path, branch);

                ObservableCollection<RepositoryContent> contentList = new ObservableCollection<RepositoryContent>();
                foreach (RepositoryContent c in content)
                {
                    contentList.Add(c);
                }

                return contentList;
            }
            catch
            {
                return null;
            }

        }
        public static async Task<ObservableCollection<Issue>> GetAllIssuesForRepo(long repoId, RepositoryIssueRequest filter)
        {
            try
            {
                var client = await UserDataService.getAuthenticatedClient();
                var issues = await client.Issue.GetAllForRepository(repoId, filter);
                ObservableCollection<Issue> issueList = new ObservableCollection<Issue>();
                foreach (Issue c in issues)
                {
                    issueList.Add(c);
                }

                return issueList;
            }
            catch
            {
                return null;
            }

        }
        public static async Task<ObservableCollection<Issue>> GetAllIssuesForRepoByUser(long repoId)
        {
            try
            {
                var client = await UserDataService.getAuthenticatedClient();
                var issues = await client.Issue.GetAllForRepository(repoId, new RepositoryIssueRequest
                {
                    State = ItemStateFilter.All,
                    Creator = GlobalHelper.UserLogin

                });
                ObservableCollection<Issue> issueList = new ObservableCollection<Issue>();
                foreach (Issue c in issues)
                {
                    issueList.Add(c);
                }

                return issueList;
            }
            catch
            {
                return null;
            }

        }
        public static async Task<ObservableCollection<Repository>> GetRepositoriesForUser(string login)
        {
            try
            {
                var client = await UserDataService.getAuthenticatedClient();
                var result = await client.Repository.GetAllForUser(login);
                ObservableCollection<Repository> repos = new ObservableCollection<Repository>();
                foreach (Repository r in result)
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
        public static async Task<ObservableCollection<IssueComment>> GetAllCommentsForIssue(string owner, string name, int number)
        {
            try
            {
                var client = await UserDataService.getAuthenticatedClient();
                var comments = await client.Issue.Comment.GetAllForIssue(owner, name, number);

                ObservableCollection<IssueComment> commentList = new ObservableCollection<IssueComment>();
                foreach (IssueComment c in comments)
                {
                    commentList.Add(c);
                }

                return commentList;
            }
            catch
            {
                return null;
            }

        }
        public static async Task<string> GetDefaultBranch(long repoId)
        {
            try
            {
                var client = await UserDataService.getAuthenticatedClient();
                var repo = await client.Repository.Get(repoId);
                return repo.DefaultBranch;
            }
            catch
            {
                return null;
            }
        }
        public static async Task<bool> StarRepository(Repository repo)
        {
            try
            {
                var client = await UserDataService.getAuthenticatedClient();
                return await client.Activity.Starring.StarRepo(repo.Owner.Login, repo.Name);
            }
            catch
            {
                return false;
            }
        }
        public static async Task<bool> UnstarRepository(Repository repo)
        {
            try
            {
                var client = await UserDataService.getAuthenticatedClient();
                return await client.Activity.Starring.RemoveStarFromRepo(repo.Owner.Login, repo.Name);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Checks if a repository is starred by the authorized user
        /// </summary>
        /// <param name="repo"></param>
        /// <returns></returns>
        public static async Task<bool> CheckStarred(Repository repo)
        {
            try
            {
                var client = await UserDataService.getAuthenticatedClient();
                return await client.Activity.Starring.CheckStarred(repo.Owner.Login, repo.Name);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a single commit for a given file path
        /// </summary>
        /// <param name="repoId">Repository Id</param>
        /// <param name="path">file path</param>
        /// <returns></returns>
        public static async Task<ObservableCollection<GitHubCommit>> GetCommitsForFile(long repoId, string path)
        {
            try
            {
                var client = await UserDataService.getAuthenticatedClient();
                CommitRequest request = new CommitRequest{ Path = path };
                var list = await client.Repository.Commit.GetAll(repoId, request);
                ObservableCollection<GitHubCommit> commitList = new ObservableCollection<GitHubCommit>();
                foreach (GitHubCommit c in list)
                {
                    commitList.Add(c);
                }
                return commitList;
            }
            catch(Exception e)
            {
                return null;
            }
        }
    }
}

