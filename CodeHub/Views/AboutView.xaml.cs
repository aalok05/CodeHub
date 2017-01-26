using CodeHub.ViewModels;
using GalaSoft.MvvmLight.Ioc;
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
    public sealed partial class AboutView : Page
    {
        public AboutViewmodel ViewModel;
        public AboutView()
        {
            this.InitializeComponent();

            ViewModel = new AboutViewmodel();
            this.DataContext = ViewModel;

            Loaded += AboutView_Loaded;
        }

        private void AboutView_Loaded(object sender, RoutedEventArgs e)
        {
            if (Window.Current.Bounds.Width < 720)
            {
                ViewModel.CurrentState = "Mobile";
            }
            else
            {
                ViewModel.CurrentState = "Desktop";
            }
        }

        private void OnCurrentStateChanged(object sender, VisualStateChangedEventArgs e)
        {
            ViewModel.CurrentState = e.NewState.Name;

            if(ViewModel.CurrentState == "Desktop")
            {
                SimpleIoc.Default.GetInstance<Services.INavigationService>().GoBack();
            }
        }
    }
}
