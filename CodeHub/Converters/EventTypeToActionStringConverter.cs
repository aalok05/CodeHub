using Octokit;
using System;
using Windows.UI.Xaml.Data;

namespace CodeHub.Converters
{
    class EventTypeToActionStringConverter : IValueConverter
    {
        /// <summary>
        /// Converts Event type to Action string. 
        /// Action string indicates in a verbose manner, what action was done by the actor in an event
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="language"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            Activity activity = value as Activity;

            var languageLoader = new Windows.ApplicationModel.Resources.ResourceLoader();
            
            switch (activity.Type)
            {
                case "IssueCommentEvent":
                    return string.Format(languageLoader.GetString("activity_CommentedIssue"), ((IssueCommentPayload)activity.Payload).Issue.Number);

                case "PullRequestReviewCommentEvent":
                    return string.Format(languageLoader.GetString("activity_CommentedPR"), ((PullRequestCommentPayload)activity.Payload).PullRequest.Number);

                case "PullRequestEvent":
                case "PullRequestReviewEvent":
                    return string.Format(languageLoader.GetString("activity_ActivityWithPR"), ActionConverter(((PullRequestEventPayload)activity.Payload).Action), ((PullRequestEventPayload)activity.Payload).PullRequest.Number);

                case "CommitCommentEvent":
                    return languageLoader.GetString("activity_CommentedCommit");
  
                case "PushEvent":
                    return string.Format(languageLoader.GetString("activity_PushedCommits"), ((PushEventPayload)activity.Payload).Commits.Count);

                case "IssuesEvent":
                    return string.Format(languageLoader.GetString("activity_ActivityWithIssues"), ActionConverter(((IssueEventPayload)activity.Payload).Action), ((IssueEventPayload)activity.Payload).Issue.Number);

                case "CreateEvent":
                    return languageLoader.GetString("activity_CreatedBranch");
                   
                case "DeleteEvent":
                    return languageLoader.GetString("activity_DeletedBranch");

                case "ForkEvent":
                    return languageLoader.GetString("activity_ForkedRepository");

                case "WatchEvent":
                    return languageLoader.GetString("activity_StarredRepository");

                case "PublicEvent":
                    return languageLoader.GetString("activity_PublishedRepository");

                case "ReleaseEvent":
                    return languageLoader.GetString("activity_PublishedRelease");

                default:
                    return languageLoader.GetString("activity_DefaultAction");
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Convert "opened", "reopened", "closed" word to a localized phrase
        /// </summary>
        /// <param name="action">opened OR reopened OR closed</param>
        /// <returns>Localized phrase</returns>
        public string ActionConverter(string action)
        {
            var languageLoader = new Windows.ApplicationModel.Resources.ResourceLoader();

            switch (action)
            {
                case "opened":
                    return languageLoader.GetString("activity_ActionOpened");

                case "reopened":
                    return languageLoader.GetString("activity_ActionReOpened");

                case "closed":
                    return languageLoader.GetString("activity_ActionClosed");

                default:
                    return action;
            }
        }
    }
}
