using System;
using GalaSoft.MvvmLight.Messaging;
using CodeHub.Helpers;
using CodeHub.ViewModels;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Controls;
using CodeHub.Services;
using Windows.ApplicationModel.DataTransfer;
using Windows.Graphics.Display;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml;
using Octokit;
using UICompositionAnimations;
using UICompositionAnimations.Enums;

namespace CodeHub.Views
{
    public sealed partial class RepoDetailView : Windows.UI.Xaml.Controls.Page
    {
        public RepoDetailViewmodel ViewModel;
        public RepoDetailView()
        {
            //Loaded += (s, e) => TopScroller.InitializeScrollViewer(ReadmeScrollViewer);
            //Unloaded += (s, e) => TopScroller.Dispose();
            InitializeComponent();
            ViewModel = new RepoDetailViewmodel();
            DataContext = ViewModel;

            DataTransferManager dataTransferManager = DataTransferManager.GetForCurrentView();
            dataTransferManager.DataRequested += DataTransferManager_DataRequested;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            Messenger.Default.Send(new GlobalHelper.SetHeaderTextMessageType { PageName = "Repository" });
            ReleaseBodyTextPanel.Visibility = Visibility.Collapsed;

            await ViewModel.Load(e.Parameter);

            ReadmeLoadingRing.IsActive = true;               
            ReadmeWebView.NavigateToString("<html><head> <link rel =\"stylesheet\" href =\"ms-appx-web:///Assets/css/github-markdown.css\" type =\"text/css\" media =\"screen\" /> </head> <body> " + await RepositoryUtility.GetReadmeHTMLForRepository(ViewModel.Repository.Id) + " </body></html> ");
        }

        private async void WebView_NavigationCompleted(WebView sender, WebViewNavigationCompletedEventArgs args)
        {
            /*  Also We are running a Javascript function that will make all links in the WebView open in an external browser
             *  instead of within the WebView itself.
             */
            await ReadmeWebView.InvokeScriptAsync("eval", new[]
            {
                @"(function()
                {
                    var body = document.getElementsByTagName('body')[0];
                    var hyperlinks = document.getElementsByTagName('a');
                    for(var i = 0; i < hyperlinks.length; i++)
                    {
                        hyperlinks[i].setAttribute('target', '_blank');
                    }
                })()"
            });
            ReadmeLoadingRing.IsActive = false;
        }

        //private void TopScroller_OnTopScrollingRequested(object sender, EventArgs e)
        //{
        //    ReadmeScrollViewer.ChangeView(null, 0, null, false);
        //}

        private void DataTransferManager_DataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            if(DataTransferManager.IsSupported())
            {
                if (!string.IsNullOrEmpty(ViewModel.Repository.HtmlUrl))
                {
                    args.Request.Data.SetText(ViewModel.Repository.HtmlUrl);
                    args.Request.Data.Properties.Title = Windows.ApplicationModel.Package.Current.DisplayName;
                }
                else
                {
                    args.Request.FailWithDisplayText("Nothing to share");
                }
            }

        }

        private void ShareButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            DataTransferManager.ShowShareUI();
        }

        private async void ReleasesList_ItemClick(object sender, ItemClickEventArgs e)
        {
            ReleaseBodyText.Text = (e.ClickedItem as Release).Body;
            ReleaseBodyTextPanel.Visibility = Visibility.Visible;
            await ReleaseBodyTextPanel.StartCompositionFadeScaleAnimationAsync(0, 1, 1.1f, 1, 150, null, 0, EasingFunctionNames.SineEaseInOut);
        }

        private async void CloseReleaseTextPanel_Tapped(object sender, TappedRoutedEventArgs e)
        {
            await ReleaseBodyTextPanel.StartCompositionFadeScaleAnimationAsync(1, 0, 1, 1.1f, 150, null, 0, EasingFunctionNames.SineEaseInOut);
            ReleaseBodyTextPanel.Visibility = Visibility.Collapsed;
        }
    }
}
