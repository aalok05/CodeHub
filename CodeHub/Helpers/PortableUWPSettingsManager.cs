using System;
using Windows.Foundation.Collections;
using Windows.Storage;
using BigWatson.PCL.Helpers;

namespace CodeHub.Helpers
{
    /// <summary>
    /// A setting imlementation for UWP apps
    /// </summary>
    public sealed class PortableUWPSettingsManager : ISettingsManager
    {
        /// <summary>
        /// Gets the settings container to use
        /// </summary>
        private readonly IPropertySet Container = ApplicationData.Current.LocalSettings.CreateContainer("CodeHubApp", ApplicationDataCreateDisposition.Always).Values;

        /// <summary>
        /// Checks whether or not there is a saved setting with the given key
        /// </summary>
        /// <param name="key">The key to look for</param>
        public bool ContainsKey(String key) => Container.ContainsKey(key);

        /// <summary>
        /// Adds or updates a setting value
        /// </summary>
        /// <typeparam name="T">The type of the setting to store</typeparam>
        /// <param name="key">The key of the setting</param>
        /// <param name="value">The value of the setting to store</param>
        public void AddOrUpdateValue<T>(String key, T value)
        {
            if (Container.ContainsKey(key)) Container[key] = value;
            else Container.Add(key, value);
        }

        /// <summary>
        /// Tries to get a setting value, returns the default value if it's not present
        /// </summary>
        /// <typeparam name="T">The type of the setting to get</typeparam>
        /// <param name="key">The key to use to retrieve the setting</param>
        public T GetValueOrDefault<T>(String key)
        {
            if (Container.ContainsKey(key)) return (T)Container[key];
            return default(T);
        }

        /// <summary>
        /// Clears all the existing settings
        /// </summary>
        public void Clear() => Container.Clear();
    }
}
