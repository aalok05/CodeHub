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
        public static async Task<ObservableCollection<Repository>> SearchRepos(string query)
        {
            try
            {
                var client = await UserDataService.getAuthenticatedClient();
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

        /// <summary>
        /// Two calls are made to this method to emulate Incremental Loading. First call (second parameter = true) returns first 7 repositories, 
        /// Second call (second parameter = false) returns the rest
        ///</summary>
        /// <param name="range"></param>
        /// <param name="firstCall"></param>
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
        public static async Task<ObservableCollection<IssueComment>> GetAllCommentsForIssue(long repoId, int number)
        {
            try
            {
                var client = await UserDataService.getAuthenticatedClient();
                var comments = await client.Issue.Comment.GetAllForIssue(repoId, number);

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
    }
}

