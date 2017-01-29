using System;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using GalaSoft.MvvmLight.Messaging;
using CodeHub.Helpers;
using CodeHub.ViewModels;
using Octokit;
using Windows.UI.Xaml.Navigation;
using UICompositionAnimations;
using Application = Windows.UI.Xaml.Application;

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
        }
    }
}
