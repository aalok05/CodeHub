using System;
using System.Threading.Tasks;
using Windows.Foundation;
//using StatusBar = Windows.UI.ViewManagement.StatusBar;
using ApiInformation = Windows.Foundation.Metadata.ApiInformation;

namespace CodeHub.Helpers
{
    /// <summary>
    /// A static class that manages the status bar on mobile devices
    /// </summary>
    //public static class SystemTrayManager
    //{
    //    // Gets the full namespace for the class
    //    private const String StatusBarString = "Windows.UI.ViewManagement.StatusBar";

    //    private static StatusBar GetCurrentStatusBarAsync()
    //    {
    //        return ApiInformation.IsTypePresent(StatusBarString) ? StatusBar.GetForCurrentView() : null;
    //    }

    //    /// <summary>
    //    /// Gets whether or not the system tray is available on the current device
    //    /// </summary>
    //    public static bool IsAPIAvailable => ApiInformation.IsTypePresent(StatusBarString);

    //    /// <summary>
    //    /// Tries to display the status bar
    //    /// </summary>
    //    /// <returns>The occluded height if the operation succedes</returns>
    //    public static async Task<double> TryShowAsync()
    //    {
    //        StatusBar statusBar = GetCurrentStatusBarAsync();
    //        if (statusBar == null) return 0;
    //        statusBar.BackgroundColor = null;
    //        await statusBar.ShowAsync();
    //        return statusBar.OccludedRect.Height;
    //    }

    //    /// <summary>
    //    /// Tries to hide the status bar, if present
    //    /// </summary>
    //    public static IAsyncAction HideAsync() => GetCurrentStatusBarAsync()?.HideAsync();

    //    /// <summary>
    //    /// Gets the occluded height of the status bar, if displayed
    //    /// </summary>
    //    public static double OccludedHeight => GetCurrentStatusBarAsync()?.OccludedRect.Height ?? 0;
    //}
}