using GalaSoft.MvvmLight;
using Windows.UI.Xaml.Media;

namespace CodeHub.Models
{
	public class SyntaxHighlightStyle : ObservableObject
	{
		public string Name { get; set; }
		public bool IsLineNumbersVisible { get; set; }
		public SolidColorBrush ColorOne { get; set; }
		public SolidColorBrush ColorTwo { get; set; }
		public SolidColorBrush ColorThree { get; set; }
		public SolidColorBrush ColorFour { get; set; }
		public SolidColorBrush BackgroundColor { get; set; }
	}
}
