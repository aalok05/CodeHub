using System;
using Windows.UI.Xaml.Data;

namespace CodeHub.Converters
{
    class TimeAgoConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            DateTime? date = value as DateTime?;
            if (date == null) return String.Empty;
            TimeSpan delta = DateTime.Now.Subtract(date.Value);
            if (delta.TotalDays >= 730) return $"{(int)(delta.TotalDays / 365)} years ago";
            if (delta.TotalDays >= 365) return "a year ago";
            if (delta.Days >= 60) return $"{delta.Days / 30} months ago";
            if (delta.Days > 30) return "a month ago";
            if (delta.Days >= 14) return $"{delta.Days / 7} weeks ago";
            if (delta.Days > 7) return "a week ago";
            if (delta.Days > 1) return $"{delta.Days} days ago";
            if (delta.Days == 1) return "a day ago";
            if (delta.Hours > 1) return $"{delta.Hours} hours ago";
            if (delta.Hours == 1) return "an hour ago";
            if (delta.Minutes > 1) return $"{delta.Minutes} minutes ago";
            return "less than a minute ago";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
