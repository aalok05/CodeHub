using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using CodeHub.Helpers;
using CodeHub.ViewModels;
using Octokit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace CodeHub.Views
{
    public sealed partial class ProfileView : Windows.UI.Xaml.Controls.Page
    {
        public ProfileViewmodel ViewModel { get; set; }
        public ProfileView()
        {
            this.InitializeComponent();
            Loading += ProfileView_Loading;
            ViewModel = new ProfileViewmodel();
           
            this.DataContext = ViewModel;

            NavigationCacheMode = NavigationCacheMode.Enabled;
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            Messenger.Default.Send(new GlobalHelper.SetHeaderTextMessageType { PageName = "Profile" });

            ViewModel.User = (User)e.Parameter;
        }
        private async void ProfileView_Loading(FrameworkElement sender, object args)
        {
            Messenger.Default.Register<User>(this, ViewModel.RecieveSignInMessage); //Listening for Sign In message
            Messenger.Default.Register<GlobalHelper.SignOutMessageType>(this, ViewModel.RecieveSignOutMessage);  //Listening for Sign Out message
            await ViewModel.Load();
        }
    }
}
