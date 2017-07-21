using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;
using Octokit;
using CodeHub.Helpers;

namespace CodeHub.Converters
{
    class PullRequestDetailStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            PullRequest pr = (PullRequest)value;
            switch (pr.State)
            {
                case ItemState.Open:
                    return $"#{pr.Number} opened by {pr.User.Login} {GlobalHelper.ConvertDateToTimeAgoFormat(DateTime.Parse(pr.CreatedAt.ToString()))}";

                case ItemState.Closed:
                    return $"#{pr.Number} by {pr.User.Login} was merged {GlobalHelper.ConvertDateToTimeAgoFormat(DateTime.Parse(pr.CreatedAt.ToString()))}";

            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
