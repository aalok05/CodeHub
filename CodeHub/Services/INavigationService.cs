using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Navigation;

namespace CodeHub.Services
{
    public interface INavigationService
    {
        void Navigate(Type type);
        void Navigate(Type type, object parameter);
        void NavigateWithoutAnimations(Type type);
        Type CurrentSourcePageType { get; set; }
        void GoBack();
        bool CanGoBack();
    }

}
