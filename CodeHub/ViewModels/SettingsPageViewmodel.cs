using GalaSoft.MvvmLight.Command;
using System;
using CodeHub.Services;
using Windows.UI.Xaml;
using CodeHub.Helpers;

namespace CodeHub.ViewModels
{
    public class SettingsPageViewModel : AppViewmodel
    {
        public string Logo => "/Assets/Images/appLogoPurple.png";

        public string DisplayName => Windows.ApplicationModel.Package.Current.DisplayName;

        public string Publisher => Windows.ApplicationModel.Package.Current.PublisherDisplayName;

        public string Version
        {
            get
            {
                var v = Windows.ApplicationModel.Package.Current.Id.Version;
                return $"{v.Major}.{v.Minor}.{v.Build}.{v.Revision}";
            }
        }
    }
}
