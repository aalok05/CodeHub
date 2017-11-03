using CodeHub.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;

namespace CodeHub.Services
{
    public class WhatsNewDisplayService
    {
        internal static bool IsNewVersion()
        {
            var currentVersion = PackageVersionToReadableString(Package.Current.Id.Version);

            var lastVersion = SettingsService.Get<string>(SettingsKeys.CurrentVersion);

            if (lastVersion == null)
            {
                SettingsService.Save<string>(SettingsKeys.CurrentVersion, currentVersion);
                return true;
            }
            else
            {
                if (currentVersion != lastVersion)
                {
                    SettingsService.Save<string>(SettingsKeys.CurrentVersion, currentVersion);

                    return true;
                }
                return false;
            }
        }

        private static string PackageVersionToReadableString(PackageVersion packageVersion)
        {
            return $"{packageVersion.Major}.{packageVersion.Minor}.{packageVersion.Build}.{packageVersion.Revision}";
        }
    }
}
