using CodeHub.Helpers;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System.Windows.Input;

namespace CodeHub.ViewModels.Settings
{
    public class AboutSettingsViewModel
    {
        public string Logo { get; private set; }

        public string DisplayName { get; private set; }

        public string Publisher { get; private set; }

        public string Version { get; private set; }

        private ICommand _shoWWhatsNewCommand;
        public ICommand ShoWWhatsNewCommand
        {
            get
            {
                if (_shoWWhatsNewCommand == null)
                {
                    _shoWWhatsNewCommand = new RelayCommand(() => Messenger.Default.Send(new GlobalHelper.ShowWhatsNewPopupMessageType()));
                }

                return _shoWWhatsNewCommand;
            }
        }


        public AboutSettingsViewModel()
        {
            Logo = "/Assets/Images/appLogoPurple.png";
            DisplayName = Windows.ApplicationModel.Package.Current.DisplayName;
            Publisher = Windows.ApplicationModel.Package.Current.PublisherDisplayName;

            var ver = Windows.ApplicationModel.Package.Current.Id.Version;
            Version = $"{ver.Major}.{ver.Minor}.{ver.Build}.{ver.Revision}";
        }
    }
}
