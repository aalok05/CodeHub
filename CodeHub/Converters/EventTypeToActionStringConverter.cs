using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;
using CodeHub.Models;

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

            switch (activity.Type)
            {
                case "IssueCommentEvent":
                    return "commented on issue #"+ ((IssueCommentPayload)activity.Payload).Issue.Number+" in";

                case "PullRequestReviewCommentEvent":
                    return "commented on PR #" + ((PullRequestCommentPayload)activity.Payload).PullRequest.Number;

                case "PullRequestEvent":
                    return ((PullRequestEventPayload)activity.Payload).Action+" PR #" + ((PullRequestEventPayload)activity.Payload).PullRequest.Number;

                case "CommitCommentEvent":
                    return "commented on commit in";
  
                case "PushEvent":
                    return "pushed "+((PushEventPayload)activity.Payload).Commits.Count+" commits to";

                case "IssuesEvent":
                    return ((IssueEventPayload)activity.Payload).Action+" issue #"+ ((IssueEventPayload)activity.Payload).Issue.Number+" in";

                case "CreateEvent":
                    return "created in";

                case "PullRequestReviewEvent":
                    return ((PullRequestEventPayload)activity.Payload).Action + " PR #" + ((PullRequestEventPayload)activity.Payload).PullRequest.Number;
                   
                case "DeleteEvent":
                    return "deleted in";

                case "ForkEvent":
                    return "forked";

                case "WatchEvent":
                    return "started watching";

                case "PublicEvent":
                    return "open sourced";

                case "ReleaseEvent":
                    return "published a release to";

                default: return "acted on";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
