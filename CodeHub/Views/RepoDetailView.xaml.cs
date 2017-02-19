using System;
using Windows.UI.Xaml;
using GalaSoft.MvvmLight.Messaging;
using CodeHub.Helpers;
using CodeHub.ViewModels;
using Windows.UI.Xaml.Navigation;
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
        }
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            Messenger.Default.Send(new GlobalHelper.SetHeaderTextMessageType { PageName = "Repository" });

            await ViewModel.Load(e.Parameter);
            FindName("LanguageText");
            FindName("DescriptionText");
            FindName("calendarSymbol");
            FindName("createdText");
            FindName("createdDateText");
            FindName("editSymbol");
            FindName("editText");
            FindName("updatedDateText");
            FindName("issueSymbol");
            FindName("issueText");
            FindName("issueCount");
            FindName("sizeSymbol");
            FindName("sizeText");
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
                ReadmeLoadingRing.IsActive = false;


        }
        private async void WebView_NavigationCompleted(WebView sender, WebViewNavigationCompletedEventArgs args)
        {
            var webView = sender as WebView;
            await webView.InvokeScriptAsync("eval", new[]
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
                })()"
            });

            ReadmeWebView.Visibility = Visibility.Visible;
            ReadmeLoadingRing.IsActive = false;
        }
    }
}
