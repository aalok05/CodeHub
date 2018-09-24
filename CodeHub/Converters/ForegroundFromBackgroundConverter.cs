using CodeHub.Helpers;
using System;
using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace CodeHub.Converters
{
	class ForegroundFromBackgroundConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			SolidColorBrush background = GlobalHelper.GetSolidColorBrush((value as string) + "FF");
			return new SolidColorBrush(PerceivedBrightness(background) > 130 ? Colors.Black : Colors.White);
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language) 
			=> throw new NotImplementedException();

		private int PerceivedBrightness(SolidColorBrush c) 
			=> (int)Math.Sqrt(
					c.Color.R * c.Color.R * .299 +
					c.Color.G * c.Color.G * .587 +
					c.Color.B * c.Color.B * .114);
	}
}