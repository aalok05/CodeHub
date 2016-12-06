using CodeHub.ViewModels;
using Octokit;
using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;


namespace CodeHub.Views
{
    public sealed partial class FileContentView : Windows.UI.Xaml.Controls.Page
    {
        public FileContentViewModel ViewModel;
        public FileContentView()
        {
            this.InitializeComponent();
            ViewModel = new FileContentViewModel();

            this.DataContext = ViewModel;
        }
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            await ViewModel.Load(e.Parameter as Tuple<Repository, string, string>);
        }

        private async void WebView_NavigationCompleted(WebView sender, WebViewNavigationCompletedEventArgs args)
        {
            /*
             * We are running a Javascript function that will make all links in the WebView open in an external browser
             * instead of within the WebView itself
             */
            var webView = sender as WebView;
            await webView.InvokeScriptAsync("eval", new[]
            {
                @"(function()
                {
                    var hyperlinks = document.getElementsByTagName('a');
                    for(var i = 0; i < hyperlinks.length; i++)
                    {
                         hyperlinks[i].setAttribute('target', '_blank');
                    }
                })()"
            });
        }
    }

}
