using CodeHub.Helpers;
using Octokit;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.Web.Http;
using CodeHub.Models;
using HtmlAgilityPack;
using JetBrains.Annotations;
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
        
        /// <summary>
        /// Returns a wrapped repository content with all the additional info that can be retrieved from the associated HTML page
        /// </summary>
        /// <param name="client">The logged GitHub client</param>
        /// <param name="id">The repository id for the current file</param>
        /// <param name="content">The source content</param>
        /// <param name="token">The cancellation token for the operation</param>
        [ItemNotNull]
        public static async Task<RepositoryContentWithCommitInfo> TryLoadLinkedCommitDataAsync([NotNull] GitHubClient client, long id, [NotNull] RepositoryContent content, CancellationToken token)
        {
            // Try to download the file info
            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    // Get the HTML for the current file
                    String html = await httpClient.GetStringAsync(content.HtmlUrl).AsTask(token).AsCancellableTask(token);
                    if (html == null) return new RepositoryContentWithCommitInfo(content, null, null);

                    // Load the HTML document
                    HtmlDocument document = new HtmlDocument();
                    document.LoadHtml(html);

                    // Try to extract the commit SHA1
                    String shaText =
                        document.DocumentNode?.Descendants("a")
                        ?.FirstOrDefault(node => node.Attributes?.AttributesWithName("class")
                        ?.FirstOrDefault()?.Value?.Equals("commit-tease-sha") == true)?.InnerText;
                    if (shaText == null) return new RepositoryContentWithCommitInfo(content, null, null);
                    String sha = Regex.Match(shaText, "[a-z0-9]{7,7}")?.Value;
                    if (sha == null) return new RepositoryContentWithCommitInfo(content, null, null);

                    // Try to get the latest commit for the current file from the extracted SHA1
                    GitHubCommit commit = await client.Repository.Commit.Get(id, sha).AsCancellableTask(token);
                    if (commit == null) return new RepositoryContentWithCommitInfo(content, null, null);

                    // Try to extract the last edit time for this document
                    String timestr = document.DocumentNode?.Descendants("relative-time")
                        ?.FirstOrDefault()?.Attributes?.AttributesWithName("datetime")?.FirstOrDefault()?.Value;
                    if (timestr == null) return new RepositoryContentWithCommitInfo(content, commit, null);
                    DateTime edit;
                    if (DateTime.TryParse(timestr, out edit))
                    {
                        return new RepositoryContentWithCommitInfo(content, commit, edit);
                    }
                    return new RepositoryContentWithCommitInfo(content, commit, null);
                }
            }
            catch
            {
                // Just return the original content without additional info
                return new RepositoryContentWithCommitInfo(content, null, null);
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
                    if (c.Type == ContentType.File)
                    {
                        
                    }
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
                    if (c.Type == ContentType.File)
                    {
                        using (var http = new HttpClient())
                        {

                            var a = await http.GetStringAsync(c.HtmlUrl);

                            XmlDocument d = new XmlDocument();

                            HtmlAgilityPack.HtmlDocument ht = new HtmlDocument();
                            ht.LoadHtml(a);
                            var sha2 = ht.DocumentNode?.Descendants("a")?.FirstOrDefault(node =>
                                        node.Attributes?.AttributesWithName("class")?.FirstOrDefault()?.Value?.Equals("commit-tease-sha") == true)?
                                .InnerText;
                            if (sha2 != null)
                            {
                                var match = Regex.Match(sha2, "[a-z0-9]{7,7}")?.Value;
                                var test = await client.Repository.Commit.Get(repoId, match);
                                //var t2 = await client.Repository.Comment.GetAllForCommit(repoId, )
                                var comment = await client.Repository.Comment.GetAllForCommit(repoId, match);
                            }

                            var timestr = ht.DocumentNode?.Descendants("relative-time")?.FirstOrDefault()?.Attributes?
                                .AttributesWithName("datetime")?.FirstOrDefault()?.Value;
                            if (timestr != null)
                            {
                                DateTime edit;
                                DateTime.TryParse(timestr, out edit);
                            }
                        }

                        
                    }
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
        /// Gets all commits for a given file path
        /// </summary>
        /// <param name="repoId">Repository Id</param>
        /// <param name="path">file path</param>
        /// <returns></returns>
        public static async Task<ObservableCollection<GitHubCommit>> GetAllCommitsForFile(long repoId, string path)
        {
            try
            {
                GitHubClient client = await UserDataService.getAuthenticatedClient();
                CommitRequest request = new CommitRequest{ Path = path };

                var list = await client.Repository.Commit.GetAll(repoId, request);

                ObservableCollection<GitHubCommit> commitList = new ObservableCollection<GitHubCommit>();

                foreach (GitHubCommit c in list)
                {
                    commitList.Add(c);
                }
                return commitList;
            }
            catch
            {
                return null;
            }
        }

    }
}

