using CodeHub.Helpers;
using System;
using UICompositionAnimations;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace CodeHub.Controls
{
    public sealed partial class IncrementalLoadButtonControl : UserControl
    {
        public IncrementalLoadButtonControl()
        {
            this.InitializeComponent();
            this.SetVisualOpacity(0);
            IsHitTestVisible = false;
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
        public double VerticalOffsetThreshold { get; set; } = 400;

        private bool _ButtonShown;

        /// <summary>
        /// Binds the IncrementalLoadButton to the ScrollViewer contained inside a given DependencyObject
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

        private void RelatedScrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            if (sender.To<ScrollViewer>().VerticalOffset >= VerticalOffsetThreshold &&
                !_ButtonShown)
            {
                _ButtonShown = true;
                this.StartXAMLTransformFadeSlideAnimation(null, 1, TranslationAxis.Y, 20, 0, 200, null, null, EasingFunctionNames.SineEaseOut,
                    () => this.IsHitTestVisible = true);
            }
            else if (sender.To<ScrollViewer>().VerticalOffset < VerticalOffsetThreshold &&
                _ButtonShown)
            {
                _ButtonShown = false;
                this.IsHitTestVisible = false;
                this.StartXAMLTransformFadeSlideAnimation(null, 0, TranslationAxis.Y, 0, 20, 200, null, null, EasingFunctionNames.SineEaseOut);
            }
        }
    }
}
