using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace CodeHub.Controls
{
	public class HtmlToRtfConverter
	{
		// Getter and Setter
		public static string GetHtmlString(DependencyObject obj) 
			=> (string)obj.GetValue(HtmlStringProperty);

		public static void SetHtmlString(DependencyObject obj, string value) 
			=> obj.SetValue(HtmlStringProperty, value);

		public static readonly DependencyProperty HtmlStringProperty =
		    DependencyProperty.RegisterAttached("HtmlString", typeof(string), typeof(HtmlToRtfConverter), new PropertyMetadata("", OnHtmlChanged));

		private static void OnHtmlChanged(DependencyObject sender, DependencyPropertyChangedEventArgs eventArgs)
		{
			if (sender is WebView wv && eventArgs.NewValue != null)
			{
				wv.NavigateToString((string)eventArgs.NewValue);
			}
		}
	}
}
