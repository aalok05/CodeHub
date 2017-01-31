using CodeHub.Helpers;
using Octokit;
using System;
using System.Collections.Generic;
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
        /// <param name="htmlUrl">The URL to the repository page</param>
        /// <param name="contents">The contents to load</param>
        /// <param name="token">The cancellation token for the operation</param>
        public static async Task<IEnumerable<RepositoryContentWithCommitInfo>> TryLoadLinkedCommitDataAsync([NotNull] String htmlUrl, [NotNull] IEnumerable<RepositoryContent> contents , CancellationToken token)
        {
            // Try to download the file info
            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    // Get the HTML for the current file
                    String html = await httpClient.GetStringAsync(new Uri(htmlUrl)).AsTask(token).AsCancellableTask(token);
                    if (html == null) return contents.Select(content => new RepositoryContentWithCommitInfo(content));

                    // Load the HTML document
                    HtmlDocument document = new HtmlDocument();
                    document.LoadHtml(html);

                    /* ================
                     * HTML STRUCTURE
                     * ================ 
                     * ...
                     * <tr class="js-navigation-item">
                     *   ...
                     *   <td class="content">
                     *     <span ...>
                     *       <a href="CONTENT_URL">...</a>
                     *     </span>
                     *   </td>
                     *   <td class="message">
                     *     <span ...>
                     *       <a title="COMMIT_MESSAGE">...</a>
                     *     </span>
                     *   </td>
                     *   <td class="age">
                     *     <span ...>
                     *       <time-ago datetime="EDIT_TIME">...</a>
                     *     </span>
                     *   </td> 
                     * ... */

                    // Try to extract the commit info
                    List<RepositoryContentWithCommitInfo> results = new List<RepositoryContentWithCommitInfo>();
                    foreach (RepositoryContent content in contents)
                    {
                        // Find the right node
                        HtmlNode target = document.DocumentNode?.Descendants("a")
                            ?.FirstOrDefault(child => child.Attributes?.AttributesWithName("href")
                            ?.FirstOrDefault()?.Value?.Equals(content.HtmlUrl.AbsolutePath) == true);
                        if (target != null)
                        {
                            // Get the commit and time nodes
                            HtmlNode
                                messageRoot = target.Ancestors("td")?.FirstOrDefault()?.Siblings()?.FirstOrDefault(node => node.Name.Equals("td")),
                                timeRoot = messageRoot?.Siblings()?.FirstOrDefault(node => node.Name.Equals("td"));
                            HtmlAttribute
                                messageTitle = messageRoot?.Descendants("a")?.FirstOrDefault()?.Attributes?.AttributesWithName("title")?.FirstOrDefault(),
                                timestamp = timeRoot?.Descendants("time-ago")?.FirstOrDefault()?.Attributes?.AttributesWithName("datetime")?.FirstOrDefault();

                            // Add the parsed contents
                            bool added = false;
                            if (timestamp?.Value != null)
                            {
                                DateTime time;
                                if (DateTime.TryParse(timestamp.Value, out time))
                                {
                                    results.Add(new RepositoryContentWithCommitInfo(content, null, messageTitle?.Value, time));
                                    added = true;
                                }
                            }
                            if (!added) results.Add(new RepositoryContentWithCommitInfo(content, null, messageTitle?.Value));
                        }
                    }
                    return results;
                }
            }
            catch
            {
                // Just return the original content without additional info
                return contents.Select(content => new RepositoryContentWithCommitInfo(content));
            }
        }

        public static async Task<ObservableCollection<RepositoryContentWithCommitInfo>> GetRepositoryContent(Repository repo, string branch)
        {
            try
            {
                // Get the files list
                GitHubClient client = await UserDataService.getAuthenticatedClient();
                IReadOnlyList<RepositoryContent> content = await client.Repository.Content.GetAllContentsByRef(repo.Owner.Login, repo.Name, branch);
                IEnumerable<RepositoryContent> sorted = content.OrderByDescending(entry => entry.Type);

                // Load the info and prepare the results
                IEnumerable<RepositoryContentWithCommitInfo> result = await TryLoadLinkedCommitDataAsync(repo.HtmlUrl, sorted, CancellationToken.None);
                return new ObservableCollection<RepositoryContentWithCommitInfo>(result);
            }
            catch
            {
                return null;
            }
        }

        public static async Task<ObservableCollection<RepositoryContentWithCommitInfo>> GetRepositoryContentByPath(Repository repo, string path, string branch)
        {
            try
            {
                // Get the files list
                GitHubClient client = await UserDataService.getAuthenticatedClient();
                IReadOnlyList<RepositoryContent> content = await client.Repository.Content.GetAllContentsByRef(repo.Id, path, branch);
                IEnumerable<RepositoryContent> sorted = content.OrderByDescending(entry => entry.Type);

                // Load the info and prepare the results
                String url = $"{repo.HtmlUrl}/tree/{branch}/{path}";
                IEnumerable<RepositoryContentWithCommitInfo> result = await TryLoadLinkedCommitDataAsync(url, sorted, CancellationToken.None);
                return new ObservableCollection<RepositoryContentWithCommitInfo>(result);
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

