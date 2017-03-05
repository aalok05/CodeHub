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

                    /* TODO: UI changes made here
                    FavoriteIcon.Foreground = brush;
                    FavoriteBlock.Foreground = brush;
                    BranchPath.Fill = brush;
                    BranchBlock.Foreground = brush;
                    BranchPath.Fill = brush;
                    BranchBlock.Foreground = brush; */
                }
                else if (Application.Current.RequestedTheme == ApplicationTheme.Dark && b.Brightness >= 180)
                {
                    double opacity = 1.0 - b.Brightness * 0.5 / 255;
                    BackgroundImage.StartCompositionFadeAnimation(null, (float)opacity, 200, null, EasingFunctionNames.Linear);
                }
            });
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
                //LanguageColorProgressRing.Visibility = Visibility.Collapsed;
                ReadmeLoadingRing.IsActive = false;

            }
        }
        private async void WebView_NavigationCompleted(WebView sender, WebViewNavigationCompletedEventArgs args)
        {
            /*  We are getting the readme div and setting it as the root of the webview.
             *  Also We are running a Javascript function that will make all links in the WebView open in an external browser
             *  instead of within the WebView itself.
             */
            String html = await ReadmeWebView.InvokeScriptAsync("eval", new[] { "document.documentElement.outerHTML;" });
            ViewModel.TryParseRepositoryLanguageColor(html);
            //LanguageColorProgressRing.Visibility = Visibility.Collapsed;
            //if (ViewModel.LanguageColor == null) ColorEllipse.Visibility = Visibility.Collapsed;
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
                })()"
            });

            if (heightString == null) return;
            double
                scale = DisplayInformation.GetForCurrentView().RawPixelsPerViewPixel,
                height = double.Parse(heightString) / (scale >= 2 ? scale - 1 : scale); // Approximate height (not so precise with high scaling)
            ReadmeWebView.Height = height;
            //ReadmeGrid.Height = height;
            ReadmeWebView.SetVisualOpacity(0);

            ReadmeWebView.Visibility = Visibility.Visible;
            ReadmeLoadingRing.IsActive = false;
        }

        private async void UIElement_OnTapped(object sender, TappedRoutedEventArgs e)
        {
            Point p = e.GetPosition(ReadmeWebView);
            int
                x = Convert.ToInt32(p.X),
                y = Convert.ToInt32(p.Y);
            String url = await ReadmeWebView.InvokeScriptAsync("eval", new[]
            {
                $@"(function()
                {{
                    var target = document.elementFromPoint({x}, {y});
                    if (target != null)
                    {{
                        return target.getAttribute('href', 2);
                    }}
                    return null;
                }})()"
            });
            if (!String.IsNullOrEmpty(url))
            {
                Launcher.LaunchUriAsync(new Uri(url)).AsTask().Forget();
            }
        }

        // Scrolls the page content back to the top
        private void TopScroller_OnTopScrollingRequested(object sender, EventArgs e)
        {
            MainScrollViewer.ChangeView(null, 0, null, false);
        }
    }
}
