using Octokit;
using System;
using Windows.UI.Xaml.Data;


namespace CodeHub.Converters
{
	/// <summary>
	/// A simple converter that returns the formatted creation time for a repository
	/// </summary>
	public class RepositoryCreationTimeConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			var repo = value as Repository;
			return repo?.CreatedAt.ToString("dd MMM yyyy", null) ?? string.Empty;
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language) 
			=> throw new NotImplementedException();
	}
}
