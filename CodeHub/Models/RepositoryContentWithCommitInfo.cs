using System;
using JetBrains.Annotations;
using Octokit;

namespace CodeHub.Models
{
    /// <summary>
    /// A class that wraps a repository content and its linked commit
    /// </summary>
    public sealed class RepositoryContentWithCommitInfo
    {
        /// <summary>
        /// Gets the repository content for this instance
        /// </summary>
        [NotNull]
        public RepositoryContent Content { get; }

        /// <summary>
        /// Gets the linked commit, if available
        /// </summary>
        [CanBeNull]
        public GitHubCommit Commit { get; }

        /// <summary>
        /// Gets the last edit time for this instance, if available
        /// </summary>
        [CanBeNull]
        public DateTime? LastEditTime { get; }

        public RepositoryContentWithCommitInfo([NotNull] RepositoryContent content, [CanBeNull] GitHubCommit commit, [CanBeNull] DateTime? editTime)
        {
            Content = content;
            Commit = commit;
            LastEditTime = editTime;
        }
    }
}
