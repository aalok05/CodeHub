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
    class IssueDetailStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            Issue issue = (Issue)value;
            switch (issue.State)
            {
                case ItemState.Open:
                    return $"#{issue.Number} opened by {issue.User.Login} {GlobalHelper.ConvertDateToTimeAgoFormat(DateTime.Parse(issue.CreatedAt.ToString()))}";

                case ItemState.Closed:
                    return $"#{issue.Number} by {issue.User.Login} was closed {GlobalHelper.ConvertDateToTimeAgoFormat(DateTime.Parse(issue.CreatedAt.ToString()))}";

            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
