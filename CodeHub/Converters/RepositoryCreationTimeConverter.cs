using System;
using Windows.UI.Xaml.Data;
using Octokit;

namespace CodeHub.Converters
{
    /// <summary>
    /// A simple converter that returns the formatted creation time for a repository
    /// </summary>
    public class RepositoryCreationTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, String language)
        {
            Repository repo = value as Repository;
            return repo?.CreatedAt.ToString("dd MMM yyyy", null) ?? String.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, String language)
        {
            throw new NotImplementedException();
        }
    }
}
