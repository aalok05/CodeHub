using System;
using System.Text.RegularExpressions;
using Windows.UI.Xaml.Data;
using Octokit;

namespace CodeHub.Converters
{
    class RepositoryContentToFileIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            RepositoryContent content = value as RepositoryContent;
            if (content == null) return String.Empty;

            const int unknown = 0xE160, file = 0xE7C3, link = 0xE816, folder = 0xE8D5;
            switch (content.Type)
            {
                case ContentType.File:
                    return Regex.IsMatch(content.Name, @"[^.]+([.]\w+)")
                        ? System.Convert.ToChar(file).ToString()
                        : System.Convert.ToChar(unknown).ToString();
                case ContentType.Dir:
                    return System.Convert.ToChar(folder).ToString();
                default: return System.Convert.ToChar(link).ToString();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
