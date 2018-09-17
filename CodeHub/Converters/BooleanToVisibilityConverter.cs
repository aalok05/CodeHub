using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace CodeHub.Converters
{
	class BooleanToVisibilityConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, string language) 
			=> (bool)value ? Visibility.Visible : Visibility.Collapsed;

		public object ConvertBack(object value, Type targetType, object parameter, string language) 
			=> throw new NotImplementedException();
	}
}
