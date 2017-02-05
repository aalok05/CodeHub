using System;
using Windows.Graphics.Display;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using GalaSoft.MvvmLight.Messaging;
using CodeHub.Helpers;
using CodeHub.ViewModels;
using Octokit;
using Windows.UI.Xaml.Navigation;
using UICompositionAnimations;
using Application = Windows.UI.Xaml.Application;
using Windows.UI.Xaml.Controls;
using Windows.Web.Http;
using CodeHub.Services;

namespace CodeHub.Views
{
    public sealed partial class RepoDetailView : Windows.UI.Xaml.Controls.Page
    {
        public RepoDetailViewmodel ViewModel;
        public RepoDetailView()
        {
            this.InitializeComponent();
            ViewModel = new RepoDetailViewmodel();
            this.DataContext = ViewModel;

            // Adjust the UI to make sure the text is readable
            Messenger.Default.Register<GlobalHelper.SetBlurredAvatarUIBrightnessMessageType>(this, b =>
            {
                if (Application.Current.RequestedTheme == ApplicationTheme.Light && b.Brightness <= 80)
                {
                    byte delta = (byte)(128 - b.Brightness + 24);
                    Color color = Color.FromArgb(byte.MaxValue, delta, delta, delta);
                    SolidColorBrush brush = new SolidColorBrush(color);
                    RepoName.Foreground = brush;
                    ProfileLinkBlock.Foreground = brush;
                    FavoriteIcon.Foreground = brush;
                    FavoriteBlock.Foreground = brush;
                    BranchPath.Fill = brush;
                    BranchBlock.Foreground = brush;
                    BranchPath.Fill = brush;
                    BranchBlock.Foreground = brush;

                }
                else if (Application.Current.RequestedTheme == ApplicationTheme.Dark && b.Brightness >= 180)
                {
                    double opacity = 1.0 - b.Brightness * 0.5 / 255;
                    BackgroundImage.StartCompositionFadeAnimation(null, (float)opacity, 200, null, EasingFunctionNames.Linear);
                }
            });
        }
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            Messenger.Default.Send(new GlobalHelper.SetHeaderTextMessageType { PageName = "Repository" });

            await ViewModel.Load(e.Parameter as Repository);

            if (SettingsService.Get<bool>(SettingsKeys.ShowReadme))
            {
                ReadmeLoadingRing.IsActive = true;
                // Manually set the user agent to get the full desktop site
                String userAgent = "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; ARM; Trident/7.0; Touch; rv:11.0; WPDesktop) like Gecko";
                HttpRequestMessage httpRequestMessage = new HttpRequestMessage(HttpMethod.Post, new Uri(ViewModel.Repository.HtmlUrl));
                httpRequestMessage.Headers.Append("User-Agent", userAgent);
                ReadmeWebView.NavigateWithHttpRequestMessage(httpRequestMessage);
            }
            else
                ReadmeLoadingRing.IsActive = false;

            // ReadmeWebview will be hidden untill JS script is executed.
            ReadmeWebView.Visibility = Visibility.Collapsed;
        }
        private async void WebView_NavigationCompleted(WebView sender, WebViewNavigationCompletedEventArgs args)
        {
            /*  We are getting the readme div and setting it as the root of the webview.
             *  Also We are running a Javascript function that will make all links in the WebView open in an external browser
             *  instead of within the WebView itself.
             */
            var webView = sender as WebView;
            String html = await webView.InvokeScriptAsync("eval", new[] { "document.documentElement.outerHTML;" });
            ViewModel.TryParseRepositoryLanguageColor(html);
            String heightString = await webView.InvokeScriptAsync("eval", new[]
            {
                @"(function()
                {
                    var node = document.getElementById('readme');
                    if (node == null) return null;
                    node.style.marginBottom = '0px';
                    var body = document.getElementsByTagName('body')[0];
                    while (body.firstChild) { body.removeChild(body.firstChild); }
                    body.appendChild(node);
                    var hyperlinks = document.getElementsByTagName('a');
                    for(var i = 0; i < hyperlinks.length; i++)
                    {
                        hyperlinks[i].setAttribute('target', '_blank');
                    }
                    return body.scrollHeight.toString();
                })()"
            });
            if (heightString == null) return;
            webView.Height = double.Parse(heightString) / DisplayInformation.GetForCurrentView().RawPixelsPerViewPixel;
            webView.SetVisualOpacity(0);
            webView.Visibility = Visibility.Visible;
            webView.StartCompositionFadeSlideAnimation(0, 1, TranslationAxis.Y, 20, 0, 200, null, null, EasingFunctionNames.CircleEaseOut);
            ReadmeLoadingRing.IsActive = false;
        }
    }
}
