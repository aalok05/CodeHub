using Octokit;
using System;
using System.Text.RegularExpressions;
using Windows.UI.Xaml.Data;

namespace CodeHub.Converters
{
	class RepositoryContentToFileIconConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			if (!(value is RepositoryContent content))
			{
				return string.Empty;
			}

			const int unknown = 0xE160, file = 0xE7C3, link = 0xE816, folder = 0xE8D5;

			if (content.Type.TryParse(out ContentType fileType))
			{
				switch (fileType)
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

			return System.Convert.ToChar(link).ToString();
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language) 
			=> throw new NotImplementedException();
	}
}
