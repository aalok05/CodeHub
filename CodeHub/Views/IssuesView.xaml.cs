using GalaSoft.MvvmLight.Messaging;
using CodeHub.Helpers;
using CodeHub.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace CodeHub.Views
{
    public sealed partial class IssuesView : Page
    {
        public IssuesViewmodel ViewModel { get; set; }
        public IssuesView()
        {
            this.InitializeComponent();
            ViewModel = new IssuesViewmodel();
           
            this.DataContext = ViewModel;

            NavigationCacheMode = NavigationCacheMode.Required;
        }
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            Messenger.Default.Send(new GlobalHelper.SetHeaderTextMessageType { PageName = "Issues" });

            if (e.NavigationMode != NavigationMode.Back)
            {
                await ViewModel.Load((long)e.Parameter);
            }
        }
     
    }
}
