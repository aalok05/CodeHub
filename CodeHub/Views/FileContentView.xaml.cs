using CodeHub.Helpers;
using CodeHub.ViewModels;
using GalaSoft.MvvmLight.Messaging;
using Octokit;
using System;
using System.Collections.Generic;
using Windows.System;
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

            await ViewModel.Load(tuple);
        }
    }

}
