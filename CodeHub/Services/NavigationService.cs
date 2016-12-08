using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace CodeHub.Services
{
    public class NavigationService : INavigationService
    {
        public NavigationService(Frame mainFrame)
        {
            _mainFrame = mainFrame;
            _mainFrame.Navigated += OnMainFrameNavigated;
        }

        private void OnMainFrameNavigated(object sender, NavigationEventArgs navigationEventArgs)
        {
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

        private Frame _mainFrame;
        public void Navigate(Type type)
        {
            _mainFrame.Navigate(type);
        }
        public void Navigate(Type type, object parameter)
        {
            _mainFrame.Navigate(type, parameter);
        }
        public bool CanGoBack()
        {
            return _mainFrame.CanGoBack;
        }
        public void GoBack()
        {
            if (_mainFrame.CanGoBack)
            {
                _mainFrame.GoBack();
            }
        }

    }
}
