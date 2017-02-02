using CodeHub.Helpers;
using Octokit;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
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

        #region Files info parsing

        /// <summary>
        /// Returns a wrapped repository content with all the additional info that can be retrieved from the associated HTML page
        /// </summary>
        /// <param name="contentTask">The task with the contents to load</param>
        /// <param name="htmlUrl">The URL to the repository page</param>
        /// <param name="client">The GitHubClient to manually retrieve the commits</param>
        /// <param name="repoId">The id of the current repository</param>
        /// <param name="branch">The name of the current branch to load</param>
        /// <param name="token">The cancellation token for the operation</param>
        [ItemCanBeNull]
        public static async Task<IEnumerable<RepositoryContentWithCommitInfo>> TryLoadLinkedCommitDataAsync(
            [NotNull] Task<IReadOnlyList<RepositoryContent>> contentTask, [NotNull] String htmlUrl,
            [NotNull] GitHubClient client, long repoId, [NotNull] String branch, CancellationToken token)
        {
            // Try to download the file info
            IReadOnlyList<RepositoryContent> contents = null;
            try
            {
                // Load the full HTML body
                WebView view = new WebView();
                TaskCompletionSource<String> tcs = new TaskCompletionSource<String>();
                view.NavigationCompleted += (s, e) =>
                {
                    view.InvokeScriptAsync("eval", new[] { "document.documentElement.outerHTML;" }).AsTask().ContinueWith(t =>
                    {
                        tcs.SetResult(t.Status == TaskStatus.RanToCompletion ? t.Result : null);
                    });
                };

                // Manually set the user agent to get the full desktop site
                String userAgent = "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; ARM; Trident/7.0; Touch; rv:11.0; WPDesktop) like Gecko";
                HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, new Uri(htmlUrl));
                httpRequestMessage.Headers.Append("User-Agent", userAgent);
                view.NavigateWithHttpRequestMessage(httpRequestMessage);

                // Run the web calls in parallel
                await Task.WhenAll(contentTask, tcs.Task);
                contents = contentTask.Result;
                String html = tcs.Task.Result;
                if (token.IsCancellationRequested)
                {
                    return contents?.OrderByDescending(entry => entry.Type).Select(content => new RepositoryContentWithCommitInfo(content));
                }

                // Load the HTML document
                HtmlDocument document = new HtmlDocument();
                document.LoadHtml(html);

                /* =================
                 * HTML error tags
                 * =================
                 * ...
                 * <include-fragment class="commit-tease commit-loader">
                 * ...
                 *   <div class="loader-error"/>
                 * </include-fragment>
                 * ... */

                // Check if the HTML loading was successful
                if (document.DocumentNode
                    ?.Descendants("include-fragment") // Get the <include-fragment/> nodes
                    ?.FirstOrDefault(node => node.Attributes?.AttributesWithName("class") // Get the nodes with a class attribute
                        ?.FirstOrDefault(att => att.Value?.Equals("commit-tease commit-loader") == true) // That attribute must have this name
                        != null) // There must be a node with these specs if the HTML loading failed
                    ?.Descendants("div") // Get the inner <div/> nodes
                    ?.FirstOrDefault(node => node.Attributes?.AttributesWithName("class")?.FirstOrDefault() // Check the class name
                        ?.Value?.Equals("loader-error") == true) != null) // Make sure there was in fact a loading error
                {
                    // Use the Oktokit APIs to get the info
                    IEnumerable<Task<IReadOnlyList<GitHubCommit>>> tasks = contents.Select(r => client.Repository.Commit.GetAll(repoId,
                        new CommitRequest { Path = r.Path, Sha = branch }, // Only get the commits that edited the current file
                        new ApiOptions { PageCount = 1, PageSize = 1 })); // Just get the latest commit for this file
                    IReadOnlyList<GitHubCommit>[] commits = await Task.WhenAll(tasks);

                    // Query the results
                    return contents.AsParallel().OrderByDescending(file => file.Type).Select((file, i) =>
                    {
                        GitHubCommit commit = commits[i].FirstOrDefault();
                        return commit != null
                            ? new RepositoryContentWithCommitInfo(file, commit, null, commit.Commit.Committer.Date.DateTime)
                            : new RepositoryContentWithCommitInfo(file);
                    });
                }

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
                 *       [...]?
                 *       <a title="COMMIT_MESSAGE">...</a>
                 *     </span>
                 *   </td>
                 *   <td class="age">
                 *     <span ...>
                 *       <time-ago datetime="EDIT_TIME">...</a>
                 *     </span>
                 *   </td> 
                 * ... */

                // Try to extract the commit info, in parallel
                int cores = Environment.ProcessorCount;
                List<RepositoryContentWithCommitInfo>[] partials =
                    (from i in Enumerable.Range(1, cores)
                     let list = new List<RepositoryContentWithCommitInfo>()
                     select list).ToArray();
                ParallelLoopResult result = Parallel.For(0, cores, new ParallelOptions { MaxDegreeOfParallelism = cores }, workerId =>
                {
                    int max = contents.Count * (workerId + 1) / cores;
                    for (int i = contents.Count * workerId / cores; i < max; i++)
                    {
                        // Find the right node
                        RepositoryContent element = contents[i];
                        HtmlNode target =
                            document.DocumentNode?.Descendants("a")
                                ?.FirstOrDefault(child => child.Attributes?.AttributesWithName("id")
                                ?.FirstOrDefault()?.Value?.EndsWith(element.Sha) == true);
                        // Parse the node contents
                        if (target != null)
                        {
                            // Get the commit and time nodes
                            HtmlNode
                                messageRoot = target.Ancestors("td")?.FirstOrDefault()?.Siblings()?.FirstOrDefault(node => node.Name.Equals("td")),
                                timeRoot = messageRoot?.Siblings()?.FirstOrDefault(node => node.Name.Equals("td"));
                            HtmlAttribute
                                messageTitle = messageRoot?.Descendants("a")
                                    ?.Select(node => node.Attributes?.AttributesWithName("title")?.FirstOrDefault())
                                    ?.FirstOrDefault(node => node != null),
                                timestamp = timeRoot?.Descendants("time-ago")?.FirstOrDefault()?.Attributes?.AttributesWithName("datetime")?.FirstOrDefault();

                            // Fix the message, if present
                            String message = messageTitle?.Value;
                            if (message != null)
                            {
                                message = WebUtility.HtmlDecode(message); // Remove HTML-encoded characters
                                message = Regex.Replace(message, @":[^:]+: ?| ?:[^:]+:", String.Empty); // Remove GitHub emojis
                            }

                            // Add the parsed contents
                            if (timestamp?.Value != null)
                            {
                                DateTime time;
                                if (DateTime.TryParse(timestamp.Value, out time))
                                {
                                    partials[workerId].Add(new RepositoryContentWithCommitInfo(element, null, message, time));
                                    continue;
                                }
                            }
                            partials[workerId].Add(new RepositoryContentWithCommitInfo(element, null, message));
                            continue;
                        }
                        partials[workerId].Add(new RepositoryContentWithCommitInfo(element));
                    }
                });
                if (!result.IsCompleted) throw new InvalidOperationException();
                return partials.SelectMany(list => list).OrderByDescending(entry => entry.Content.Type);
            }
            catch
            {
                // Just return the original content without additional info
                return contents?.OrderByDescending(entry => entry.Type).Select(content => new RepositoryContentWithCommitInfo(content));
            }
        }

        #endregion

        public static async Task<ObservableCollection<RepositoryContentWithCommitInfo>> GetRepositoryContent(Repository repo, string branch)
        {
            try
            {
                // Get the files list
                GitHubClient client = await UserDataService.getAuthenticatedClient();
                IEnumerable<RepositoryContentWithCommitInfo> results = await TryLoadLinkedCommitDataAsync(
                    client.Repository.Content.GetAllContentsByRef(repo.Owner.Login, repo.Name, branch), repo.HtmlUrl,
                    client, repo.Id, branch, CancellationToken.None);
                return new ObservableCollection<RepositoryContentWithCommitInfo>(results);
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
                String url = $"{repo.HtmlUrl}/tree/{branch}/{path}";
                IEnumerable<RepositoryContentWithCommitInfo> results = await TryLoadLinkedCommitDataAsync(
                    client.Repository.Content.GetAllContentsByRef(repo.Id, path, branch), url,
                    client, repo.Id, branch, CancellationToken.None);
                return new ObservableCollection<RepositoryContentWithCommitInfo>(results);
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

