using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace CodeHub.Converters
{
    class EventTypeToCommentStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            Activity activity = value as Activity;

            switch (activity.Type)
            {
                case "IssueCommentEvent":
                    return ((IssueCommentPayload)activity.Payload).Comment.Body;

                case "PullRequestReviewCommentEvent":
                    return ((PullRequestCommentPayload)activity.Payload).Comment.Body;

                case "PushEvent":
                    return ((PushEventPayload)activity.Payload).Ref;

                default: return string.Empty;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
