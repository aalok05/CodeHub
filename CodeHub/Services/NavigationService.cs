using CodeHub.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using GalaSoft.MvvmLight;

namespace CodeHub.Services
{
    public class NavigationService : INavigationService
    {
        public Type CurrentSourcePageType { get; set; }
        public NavigationService(CustomFrame mainFrame)
        {
            _mainFrame = mainFrame;
            _mainFrame.Navigated += OnMainFrameNavigated;
        }

        private void OnMainFrameNavigated(object sender, NavigationEventArgs navigationEventArgs)
        {
            CurrentSourcePageType = _mainFrame.CurrentSourcePageType;
            if (_mainFrame.CanGoBack)
            {
                // Show UI in title bar if opted-in and in-app backstack is not empty.
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                    AppViewBackButtonVisibility.Visible;
            }
            else
            {
                // Remove the UI from the title bar if in-app back stack is empty.
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                    AppViewBackButtonVisibility.Collapsed;
            }
        }

        private CustomFrame _mainFrame;
        public async void Navigate(Type type)
        {
           if(_mainFrame.CurrentSourcePageType!= type)
                await _mainFrame.Navigate(type);
        }
        public async void Navigate(Type type, object parameter)
        {
           await _mainFrame.Navigate(type, parameter);
        }
        public void NavigateWithoutAnimations(Type type)
        {
            if (_mainFrame.CurrentSourcePageType != type)
                _mainFrame.NavigateWithoutAnimations(type);
        }
        public bool CanGoBack()
        {
            return _mainFrame.CanGoBack;
        }
        public async void GoBack()
        {
            if (_mainFrame.CanGoBack)
            {
               await _mainFrame.GoBack();
            }
        }

    }
}
