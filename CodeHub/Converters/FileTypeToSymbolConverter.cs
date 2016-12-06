using Octokit;
using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace CodeHub.Converters
{
    class FileTypeToSymbolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (((RepositoryContent)value).Type == ContentType.Dir)
                return Symbol.Folder;
            else
                return Symbol.Page2;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
