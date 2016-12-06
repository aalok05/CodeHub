using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace CodeHub.Services
{
    public class NavigationService : INavigationService
    {
        public NavigationService(Frame mainFrame)
        {
            _mainFrame = mainFrame;
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
