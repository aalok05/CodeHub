using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using UICompositionAnimations;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace CodeHub.Controls
{
    /// <summary>
    /// A custom Frame that animates its content during the navigation process
    /// </summary>
    public class CustomFrame : Frame
    {
        #region Constants, parameters and events

        /// <summary>
        /// The duration of each navigation animation, in milliseconds
        /// </summary>
        private const int ContentAnimationDuration = 200;

        /// <summary>
        /// The offset of the X/Y axis, when the CustomFrame is using a slide animation to navigate
        /// </summary>
        private const int TargetContentAxisOffset = 30;

        /// <summary>
        /// Raised when the CustomFrame contains at least one page, and when it gets empty
        /// </summary>
        public event EventHandler<bool> EmptyContentStateChanged;

        /// <summary>
        /// Returns the current content as a FrameworkElement
        /// </summary>
        private FrameworkElement FrameworkContent => (FrameworkElement)Content;

        #endregion

        #region Animations

        /// <summary>
        /// Animates the content out when the CustomFrame is navigating forward (fade out current page)
        /// </summary>
        private Task GetContentForwardOutStoryboard()
        {
            return FrameworkContent.StartCompositionFadeScaleAnimationAsync(1, 0, 1, 1.1f, ContentAnimationDuration, null, 0, EasingFunctionNames.SineEaseInOut);
        }

        /// <summary>
        /// Animates the content in when the CustomFrame is navigating forward (fade in target page)
        /// </summary>
        private Task GetContentForwardInStoryboard()
        {
            return FrameworkContent.StartCompositionFadeScaleAnimationAsync(0, 1, 0.9f, 1, ContentAnimationDuration, null, 0, EasingFunctionNames.SineEaseInOut);
        }

        /// <summary>
        /// Animates the content in when the CustomFrame is going back (fade in previous page)
        /// </summary>
        private Task GetContentBackOutStoryboard()
        {
            return FrameworkContent.StartCompositionFadeScaleAnimationAsync(1, 0, 1, 0.9f, ContentAnimationDuration, null, 0, EasingFunctionNames.SineEaseInOut);
        }

        /// <summary>
        /// Animates the content out when the CustomFrame is going back (fade out current page)
        /// </summary>
        private Task GetContentBackInStoryboard()
        {
            return FrameworkContent.StartCompositionFadeScaleAnimationAsync(0, 1, 1.1f, 1, ContentAnimationDuration, null, 0, EasingFunctionNames.SineEaseInOut);
        }

        #endregion

        #region Navigation methods

        /// <summary>
        /// Navigates to the target page type without playing any animations
        /// </summary>
        /// <param name="pageType">The type of the target page</param>
        /// <param name="parameter">The optional parameter</param>
        public bool NavigateWithoutAnimations(Type pageType, [Optional] object parameter)
        {
            bool eventPending = IsEmpty;
            bool result = base.Navigate(pageType, parameter);
            if (eventPending) EmptyContentStateChanged?.Invoke(this, false);
            return result;
        }

        /// <summary>
        /// Navigates to the target page type
        /// </summary>
        /// <param name="pageType">The type of the target page</param>
        /// <param name="parameter">The optional parameter</param>
        public new async Task<bool> Navigate(Type pageType, [Optional] object parameter)
        {
            // Avoid accidental inputs while the Frame is navigating
            IsHitTestVisible = false;

            // Create the TaskCompletionSource to await the end of the async method
            TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();

            // Prepare the action that will call the base navigation method and finish the animations
            Action<bool> finishNavigation = async empty =>
            {
                // Set the CustomFrame up before the navigation
                Opacity = 0;
                bool navResult = base.Navigate(pageType, parameter);
                if (empty) BackStack.Clear();
                FrameworkContent.SetVisualOpacity(0);
                Opacity = 1;

                // Setup the right animation
                await GetContentForwardInStoryboard();
                IsHitTestVisible = true;
                tcs.SetResult(navResult);
            };

            // If the current content is not null, animate the fade out for the current page
            if (Content != null)
            {
                await GetContentForwardOutStoryboard();
                finishNavigation(false);
            }
            else
            {
                EmptyContentStateChanged?.Invoke(this, false);
                finishNavigation(true);
            }

            // Wait for all the animations to finish and then return the navigation result
            return await tcs.Task;
        }

        /// <summary>
        /// Navigates back to the previous page
        /// </summary>
        public new async Task GoBack()
        {
            // Avoid accidental inputs while the Frame is going back
            IsHitTestVisible = false;

            // Setup the right animation
            await GetContentBackOutStoryboard();

            // Set the CustomFrame up before going back
            Opacity = 0;
            base.GoBack();
            FrameworkContent.SetVisualOpacity(0);
            Opacity = 1;

            // Prepare the final animation
            await GetContentBackInStoryboard();
            IsHitTestVisible = true;
        }

        /// <summary>
        /// Clears the Frame content and its navigation stack without any animation
        /// </summary>
        public void ResetFrame()
        {
            BackStack.Clear();
            if (Content != null)
            {
                try
                {
                    // This call causes the current page to execute the OnNavigatedFrom method
                    GetNavigationState();
                }
                catch
                {
                    // Doesn't matter
                }
            }
            Content = null;
            EmptyContentStateChanged?.Invoke(this, true);
        }

        /// <summary>
        /// Clears the Frame content and its navigation stack and plays the fade out animations necessary
        /// </summary>
        /// <param name="skipAnimations">Indicates whether or not to play the fade out animations</param>
        public async Task ClearContent(bool skipAnimations = false)
        {
            // Ignore the call if the content is already empty
            if (Content == null) return;

            // Avoid accidental inputs while the Frame is resetting
            IsHitTestVisible = false;

            // Start and wait the animation
            if (!skipAnimations) await GetContentBackOutStoryboard();

            // Clear the content and the navigation stack
            ResetFrame();
            IsHitTestVisible = true;
        }

        #endregion

        #region Extensions

        /// <summary>
        /// Returns true if the CustomFrame is actually empty
        /// </summary>
        public bool IsEmpty => Content == null;

        /// <summary>
        /// Gets the Type of the Page inside the CustomFrame. Returns null if the CustomFrame is empty.
        /// </summary>
        public Type ContentType => Content?.GetType();

        public object LastNavigationParameter => BackStackDepth == 0 ? null : BackStack.Last().Parameter;

        public void IsContentVisible(bool value) => FrameworkContent.SetVisualOpacity(value? 1: 0);

        #endregion
    }
}
