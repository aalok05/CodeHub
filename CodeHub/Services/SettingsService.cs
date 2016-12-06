using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeHub.Services
{
    class SettingsService
    {
        public static void SaveSetting(string key, string value)
        {
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            localSettings.Values[key] = value;
        }
        public static string GetSetting(string key)
        {
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            var value = localSettings.Values[key];

            if (value != null)
            {
                return value.ToString();
            }
            else
            {
                return "";
            }
        }
    }
}
