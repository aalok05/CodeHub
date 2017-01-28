using CodeHub.Helpers;
using CodeHub.ViewModels;
using GalaSoft.MvvmLight.Messaging;
using Octokit;
using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;


namespace CodeHub.Views
{
    public sealed partial class FileContentView : Windows.UI.Xaml.Controls.Page
    {
        public FileContentViewmodel ViewModel;
        public FileContentView()
        {
            this.InitializeComponent();
            ViewModel = new FileContentViewmodel();

            this.DataContext = ViewModel;
        }
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            //This page recieves repository ,path and branch
            var tuple = e.Parameter as Tuple<Repository, string, string>;

            Messenger.Default.Send(new GlobalHelper.SetHeaderTextMessageType { PageName = tuple.Item1.FullName });

            await ViewModel.Load(tuple);
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
