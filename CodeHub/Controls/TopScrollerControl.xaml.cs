using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using CodeHub.Helpers;
using UICompositionAnimations;

namespace CodeHub.Controls
{
    public sealed partial class TopScrollerControl : UserControl, IDisposable
    {
        /// <summary>
        /// Gets the duration of the fade in/out animation for the control
        /// </summary>
        private static readonly int AnimationDuration = 200;

        public TopScrollerControl()
        {
            this.InitializeComponent();
            this.SetVisualOpacity(0);
            this.IsHitTestVisible = false;
        }

        public void Dispose()
        {
            if (_RelatedScrollViewer != null)
            {
                _RelatedScrollViewer.ViewChanged -= RelatedScrollViewer_ViewChanged;
                _RelatedScrollViewer = null;
            }
        }

        // The ScrollViewer in use
        private ScrollViewer _RelatedScrollViewer;

        /// <summary>
        /// Gets or sets the minimum vertical offset before the control is shown
        /// </summary>
        public double VerticalOffsetThreshold { get; set; } = 200;

        /// <summary>
        /// Binds the AutoHideCanvas to the ScrollViewer contained inside a given DependencyObject
        /// </summary>
        /// <param name="parentObject">The object that contains the ScrollViewer</param>
        public void InitializeScrollViewer(DependencyObject parentObject)
        {
            if (this.Visibility == Visibility.Collapsed) return;
            ScrollViewer scroller = parentObject.FindChild<ScrollViewer>();
            if (scroller == null)
            {
                throw new ArgumentException("The DependencyObject doesn't contain a ScrollViewer");
            }
            if (_RelatedScrollViewer != null)
            {
                _RelatedScrollViewer.ViewChanged -= RelatedScrollViewer_ViewChanged;
            }
            _RelatedScrollViewer = scroller;
            _RelatedScrollViewer.ViewChanged += RelatedScrollViewer_ViewChanged;
        }

        private DateTime? _LastAnimationStartTime;

        private bool _ButtonShown;

        private void RelatedScrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            if (sender.To<ScrollViewer>().VerticalOffset >= VerticalOffsetThreshold &&
                !_ButtonShown &&
                (_LastAnimationStartTime == null || DateTime.Now.Subtract(_LastAnimationStartTime.Value).TotalMilliseconds > AnimationDuration))
            {
                _LastAnimationStartTime = DateTime.Now;
                _ButtonShown = true;
                this.StartXAMLTransformFadeSlideAnimation(null, 1, TranslationAxis.Y, 20, 0, 200, null, null, EasingFunctionNames.SineEaseOut,
                    () => this.IsHitTestVisible = true);
            }
            else if (sender.To<ScrollViewer>().VerticalOffset < VerticalOffsetThreshold &&
                _ButtonShown &&
                (_LastAnimationStartTime == null || DateTime.Now.Subtract(_LastAnimationStartTime.Value).TotalMilliseconds > AnimationDuration))
            {
                _LastAnimationStartTime = DateTime.Now;
                _ButtonShown = false;
                this.IsHitTestVisible = false;
                this.StartXAMLTransformFadeSlideAnimation(null, 0, TranslationAxis.Y, 0, 20, 200, null, null, EasingFunctionNames.SineEaseOut);
            }
        }

        /// <summary>
        /// Raised whenever the user taps on the control to request a scrolling to the top
        /// </summary>
        public event EventHandler TopScrollingRequested;

        private void TopScrollerHandleControl_OnTapped(object sender, TappedRoutedEventArgs e)
        {
            if (!_ButtonShown) return;
            _LastAnimationStartTime = DateTime.Now;
            _ButtonShown = false;
            this.IsHitTestVisible = false;
            this.StartXAMLTransformFadeSlideAnimation(null, 0, TranslationAxis.Y, 0, 20, 200, null, null, EasingFunctionNames.SineEaseOut);
            TopScrollingRequested?.Invoke(this, EventArgs.Empty);
        }
    }
}
