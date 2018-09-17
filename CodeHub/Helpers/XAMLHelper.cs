using JetBrains.Annotations;
using System;
using Windows.UI.Xaml;

namespace CodeHub.Helpers
{
	/// <summary>
	/// A static class used to manage XAML resources
	/// </summary>
	public static class XAMLHelper
	{
		/// <summary>
		/// Assigns the given value to a XAML resource
		/// </summary>
		/// <typeparam name="T">The Type of the resource</typeparam>
		/// <param name="resourceName">The name of the resource</param>
		/// <param name="value">The new value to use</param>
		public static void AssignValueToXAMLResource<T>([NotNull] string resourceName, T value)
		{
			// Parameter check
			if (resourceName.Length == 0)
			{
				throw new ArgumentException("The resource name is not valid");
			}

			// Safe cast to be sure the target resource has the Type of the new value
			if (Application.Current.Resources[resourceName]?.GetType() != typeof(T))
			{
				throw new InvalidOperationException("The target resource has a different type");
			}

			// Finally assign the new value to the resource
			Application.Current.Resources[resourceName] = value;
		}

		/// <summary>
		/// Retrieves the value of a given generic XAML resource
		/// </summary>
		/// <param name="resourceName">The name of the resource</param>
		public static object GetGenericResourceValue([NotNull] string resourceName) 
			=> Application.Current.Resources[resourceName];

		/// <summary>
		/// Retrieves the value of a given XAML resource
		/// </summary>
		/// <typeparam name="T">The Type of the resource to get</typeparam>
		/// <param name="resourceName">The name of the resource</param>
		public static T GetResourceValue<T>([NotNull] string resourceName) 
			=> Application.Current.Resources[resourceName].To<T>();
	}
}