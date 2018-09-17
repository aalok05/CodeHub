using System;
using Windows.UI.Xaml.Data;

namespace CodeHub.Converters
{
	class ObjectToStringConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, string language) 
			=> value != null ? value.ToString() : string.Empty;

		public object ConvertBack(object value, Type targetType, object parameter, string language) 
			=> throw new NotImplementedException();
	}
}
