using System;
using Windows.Foundation;
using Windows.Graphics.Display;
using Windows.System;
using Windows.UI;
using Windows.UI.Xaml;
using GalaSoft.MvvmLight.Messaging;
using CodeHub.Helpers;
using CodeHub.ViewModels;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.Web.Http;
using CodeHub.Services;
using UICompositionAnimations;
using UICompositionAnimations.Enums;

namespace CodeHub.Views
{
    public sealed partial class RepoDetailView : Windows.UI.Xaml.Controls.Page
    {
        public RepoDetailViewmodel ViewModel;
        public RepoDetailView()
        {
            this.Loaded += (s, e) => TopScroller.InitializeScrollViewer(MainScrollViewer);
            this.Unloaded += (s, e) => TopScroller.Dispose();
            this.InitializeComponent();
            ViewModel = new RepoDetailViewmodel();
            this.DataContext = ViewModel;
        }
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            Messenger.Default.Send(new GlobalHelper.SetHeaderTextMessageType { PageName = "Repository" });

            await ViewModel.Load(e.Parameter);

            FindName("LanguageText");
            FindName("DescriptionText");
            FindName("calendarSymbol");
            FindName("createdDateText");
            FindName("editSymbol");
            FindName("updatedDateText");
            FindName("sizeSymbol");
            FindName("sizeCount");
            FindName("sizeUnitText");


            ReadmeWebView.Visibility = Visibility.Collapsed;
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
            {
                ReadmeLoadingRing.IsActive = false;
            }
        }
        private async void WebView_NavigationCompleted(WebView sender, WebViewNavigationCompletedEventArgs args)
        {
            /*  We are getting the readme div and setting it as the root of the webview.
             *  Also We are running a Javascript function that will make all links in the WebView open in an external browser
             *  instead of within the WebView itself.
             */

            String heightString = await ReadmeWebView.InvokeScriptAsync("eval", new[]
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

            if (String.IsNullOrEmpty(heightString)) return;
            double
                scale = DisplayInformation.GetForCurrentView().RawPixelsPerViewPixel,
                height = double.Parse(heightString) / (scale >= 2 ? scale - 1 : scale); // Approximate height (not so precise with high scaling)
            ReadmeWebView.Height = height;
            ReadmeGrid.Height = height;
            ReadmeWebView.SetVisualOpacity(0);
            ReadmeWebView.Visibility = Visibility.Visible;
            ReadmeWebView.StartCompositionFadeSlideAnimation(0, 1, TranslationAxis.Y, 20, 0, 200, null, null, EasingFunctionNames.CircleEaseOut);
            ReadmeLoadingRing.IsActive = false;
        }

        // Scrolls the page content back to the top
        private void TopScroller_OnTopScrollingRequested(object sender, EventArgs e)
        {
            MainScrollViewer.ChangeView(null, 0, null, false);
        }
    }
}
