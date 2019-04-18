using System;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace EdgeAuth
{
    /// <summary>
    /// Starts the authentication operation. You can call the methods of this class multiple times in a single application or across multiple applications at the same time.
    /// Replaces the UWP equivalent and uses the Edge browser for authentication with apps like GitHub which displays a warning running on Internet Explorer.
    /// </summary>
    public sealed class WebAuthenticationBroker
    {
        private static Uri redirectUri;
        private static ContentDialog dialog;
        private static string code = string.Empty;
        private static uint errorCode = 0;

        public static Task<WebAuthenticationResult> AuthenticateAsync(WebAuthenticationOptions options, Uri requestUri)
        {
            return AuthenticateAsync(options, requestUri, Windows.Security.Authentication.Web.WebAuthenticationBroker.GetCurrentApplicationCallbackUri());
        }

        public static async Task<WebAuthenticationResult> AuthenticateAsync(WebAuthenticationOptions options, Uri requestUri, Uri callbackUri)
        {
            if (options != WebAuthenticationOptions.None)
                throw new ArgumentException("WebAuthenticationBroker currently only supports WebAuthenticationOptions.None", "options");

            redirectUri = callbackUri;
            dialog = new ContentDialog();

            var grid = new Grid();
            grid.RowDefinitions.Add(new RowDefinition() { Height = Windows.UI.Xaml.GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition() { Height = new Windows.UI.Xaml.GridLength(1, Windows.UI.Xaml.GridUnitType.Star) });

            var label = new TextBlock();
            label.Text = "Connecting to GitHub";
            label.HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Left;
            label.Margin = new Windows.UI.Xaml.Thickness(0);
            grid.Children.Add(label);

            var closeButton = new Button();
            closeButton.Content = "";
            closeButton.FontFamily = new FontFamily("Segoe UI Symbol");
            closeButton.BorderBrush = new SolidColorBrush(Windows.UI.Color.FromArgb(0, 0, 0, 0));
            closeButton.Background = new SolidColorBrush(Windows.UI.Color.FromArgb(0, 0, 0, 0));
            closeButton.Margin = new Windows.UI.Xaml.Thickness(0);
            closeButton.HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Right;
            closeButton.Click += (s,e) => { dialog.Hide(); };
            grid.Children.Add(closeButton);
            
            var webView = new WebView(WebViewExecutionMode.SameThread) { Source = requestUri };
            webView.AllowFocusOnInteraction = true;
            webView.SetValue(Grid.RowProperty, 1);
            webView.NavigationStarting += WebView_NavigationStarting;
            webView.NavigationFailed += WebView_NavigationFailed;
            webView.MinWidth = 480;
            webView.MinHeight = 600;
            grid.Children.Add(webView);
            
            dialog.Content = grid;
            dialog.GotFocus += (s, e) => { webView.Focus(Windows.UI.Xaml.FocusState.Programmatic); };
            var res = await dialog.ShowAsync();
            return new WebAuthenticationResult(code, errorCode, errorCode > 0 ? WebAuthenticationStatus.ErrorHttp : string.IsNullOrEmpty(code) ? WebAuthenticationStatus.UserCancel : WebAuthenticationStatus.Success);
        }

        private static void WebView_NavigationFailed(object sender, WebViewNavigationFailedEventArgs e)
        {
            errorCode = (uint)e.WebErrorStatus;
            dialog.Hide();
        }

        private static void WebView_NavigationStarting(WebView sender, WebViewNavigationStartingEventArgs args)
        {
            if (args.Uri.ToString().StartsWith(redirectUri.ToString()))
            {
                var querySegs = args.Uri.Query.Substring(1).Split('&');
                foreach (string seg in querySegs)
                {
                    if (seg.StartsWith("code="))
                    {
                        code = args.Uri.ToString();
                        break;
                    }
                }

                args.Cancel = true;
                dialog.Hide();
            }
        }
    }
}
