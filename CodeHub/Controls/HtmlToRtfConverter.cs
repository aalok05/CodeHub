using System;
using Windows.Data.Xml.Dom;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;

namespace CodeHub.Controls
{
    public class HtmlToRtfConverter
    {
        // Getter and Setter
        public static string GetHtmlString(DependencyObject obj) { return (string)obj.GetValue(HtmlStringProperty); }
        public static void SetHtmlString(DependencyObject obj, string value)
        { obj.SetValue(HtmlStringProperty, value); }

        public static readonly DependencyProperty HtmlStringProperty =
            DependencyProperty.RegisterAttached("HtmlString", typeof(string), typeof(HtmlToRtfConverter), new PropertyMetadata("", OnHtmlChanged));

        private static void OnHtmlChanged(DependencyObject sender, DependencyPropertyChangedEventArgs eventArgs)
        {
            WebView wv = sender as WebView;
            if (wv != null && eventArgs.NewValue != null)
            {
                wv.NavigateToString((string)eventArgs.NewValue);
            }
        }
    }
}
