using System;
using System.Threading.Tasks;

namespace CodeHub.Services
{
    /// <summary>
    /// An interface for an asynchronous navigations system (due to the navigation animations)
    /// </summary>
    public interface IAsyncNavigationService
    {
        /// <summary>
        /// Navigates to the target page
        /// </summary>
        /// <param name="pageType">The type of the target page</param>
        /// <param name="pageTitle">The page title</param>
        Task<bool> NavigateAsync(Type pageType, String pageTitle);

        /// <summary>
        /// Navigates to the target page with a given parameter
        /// </summary>
        /// <param name="pageType">The type of the target page</param>
        /// <param name="pageTitle">The page title</param>
        /// <param name="parameter">The navigation parameter</param>
        Task<bool> NavigateAsync(Type pageType, String pageTitle, object parameter);

        /// <summary>
        /// Navigates to the target page without displaying any animations
        /// </summary>
        /// <param name="pageType">The type of the target page</param>
        /// <param name="pageTitle">The page title</param>
        void NavigateWithoutAnimations(Type pageType, String pageTitle);

        /// <summary>
        /// Navigates to the target page with a given parameter without displaying any animations
        /// </summary>
        /// <param name="pageType">The type of the target page</param>
        /// <param name="pageTitle">The page title</param>
        void NavigateWithoutAnimations(Type pageType, String pageTitle, object parameter);

        /// <summary>
        /// Gets the current page type
        /// </summary>
        Type CurrentSourcePageType { get; }

        /// <summary>
        /// Tries to navigate back
        /// </summary>
        Task<bool> GoBackAsync();

        /// <summary>
        /// Checks if it is possible to perform a back navigation
        /// </summary>
        Task<bool> CanGoBackAsync();
    }
}
