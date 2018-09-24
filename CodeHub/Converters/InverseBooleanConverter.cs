using System;
using Windows.UI.Xaml.Data;

namespace CodeHub.Converters
{
	class InverseBooleanConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, string language) 
			=> !(bool)value;

		public object ConvertBack(object value, Type targetType, object parameter, string language) 
			=> !(bool)value;
	}
}
