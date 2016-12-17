using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            if( (((string)value) == "IssueCommentEvent") || ((string)value) == "PullRequestReviewCommentEvent" || ((string)value) == "CommitCommentEvent")
            {
                return "commented on";
            }
            else if(((string)value) == "PushEvent")
            {
                return "pushed to";
            }
            else if (((string)value) == "CreateEvent")
            {
                return "created";
            }
            else if (((string)value) == "DeleteEvent")
            {
                return "deleted";
            }
            else if (((string)value) == "ForkEvent" || ((string)value) == "ForkApplyEvent")
            {
                return "forked";
            }
            else if (((string)value) == "WatchEvent")
            {
                return "started watching";
            }
            else if (((string)value) == "PublicEvent")
            {
                return "open sourced";
            }
            else if (((string)value) == "FollowEvent")
            {
                return "followed";
            }
            return "acted on"; 
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
