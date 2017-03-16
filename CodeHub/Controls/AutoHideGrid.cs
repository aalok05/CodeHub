using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using CodeHub.Helpers;
using UICompositionAnimations;
using UICompositionAnimations.Enums;

namespace CodeHub.Controls
{
    public class AutoHideGrid : Grid, IDisposable
    {
        /// <summary>
        /// Gets the duration of the animation that shows/hides the control when the ScrollViewer is scrolled up/down
        /// </summary>
        private const int AnimationDuration = 300;

        public AutoHideGrid()
        {
            // UI adjustments
            Canvas.SetZIndex(this, 1);

            // Set the RenderTransform for later use
            this.RenderTransform = new TranslateTransform();
        }

        private Storyboard _PendingStoriboard;

        /// <summary>
        /// The animation that is currently playing or the last one that was executed
        /// </summary>
        private Storyboard PendingStoryboard
        {
            get { return _PendingStoriboard; }
            set
            {
                _PendingStoriboard?.Stop();
                _PendingStoriboard = value;
            }
        }

        private bool _IsVisible = true;

        /// <summary>
        /// Indicates whether or not the AutoHideCanvas is currently shown
        /// </summary>
        public bool IsVisible
        {
            get { return _IsVisible; }
            set
            {
                if (IsVisible == value) return;
                PendingStoryboard = value
                    ? this.GetXAMLTransformSlideStoryboard(TranslationAxis.Y, null, 0, AnimationDuration, EasingFunctionNames.Linear)
                    : this.GetXAMLTransformSlideStoryboard(TranslationAxis.Y, null, -48, AnimationDuration, EasingFunctionNames.Linear);
                PendingStoryboard.Begin();
                _IsVisible = value;
            }
        }

        private ScrollViewer RelatedScrollViewer;

        /// <summary>
        /// Binds the AutoHideCanvas to the ScrollViewer contained inside a given DependencyObject
        /// </summary>
        /// <param name="parentObject">The object that contains the ScrollViewer</param>
        public void InitializeScrollViewer(DependencyObject parentObject)
        {
            RelatedScrollViewer = parentObject.FindChild<ScrollViewer>();
            if (RelatedScrollViewer == null)
            {
                throw new ArgumentException("The DependencyObject doesn't contain a ScrollViewer");
            }
            RelatedScrollViewer.ViewChanged += RelatedScrollViewer_ViewChanged;
        }

        public void Dispose()
        {
            if (RelatedScrollViewer != null)
            {
                RelatedScrollViewer.ViewChanged -= RelatedScrollViewer_ViewChanged;
                RelatedScrollViewer = null;
            }
        }

        /// <summary>
        /// The previous VerticalOffset of the related ScrollViewer
        /// </summary>
        private double BackupVerticalOffset;

        private double LastFinalVerticalOffset;

        private DateTime? LastPartialScrollTime;

        //Updates the Visibility when the user scrolls up or down on the ScrollViewer
        void RelatedScrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            if (!IsViewChangedEnabled) return;

            // Partial scroll
            if (e.IsIntermediate)
            {
                // Skip duplicates
                if (BackupVerticalOffset.EqualsWithDelta(RelatedScrollViewer.VerticalOffset, 0.01)) return;

                // Only update the visual state if the last change happened more than 100ms ago
                if (LastPartialScrollTime == null) LastPartialScrollTime = DateTime.Now;
                double delta = DateTime.Now.Subtract(LastPartialScrollTime.Value).TotalMilliseconds;
                if (delta > 100)
                {
                    LastPartialScrollTime = DateTime.Now;
                }
                else
                {
                    BackupVerticalOffset = RelatedScrollViewer.VerticalOffset;
                    return;
                }

                // Skip the update if the vertical offset difference is less than 0.1
                if (RelatedScrollViewer.VerticalOffset.EqualsWithDelta(BackupVerticalOffset)) return;

                // Update the visual state
                if (RelatedScrollViewer.VerticalOffset > BackupVerticalOffset)
                {
                    IsVisible = false;
                }
                else if (RelatedScrollViewer.VerticalOffset < BackupVerticalOffset || RelatedScrollViewer.VerticalOffset.EqualsWithDelta(0, 0.01))
                {
                    IsVisible = true;
                }
            }
            else
            {
                // Skip duplicates
                if (LastFinalVerticalOffset.EqualsWithDelta(RelatedScrollViewer.VerticalOffset, 0.01)) return;

                // Update the visual state when a final scroll gesture happens
                if (LastFinalVerticalOffset > RelatedScrollViewer.VerticalOffset || RelatedScrollViewer.VerticalOffset.EqualsWithDelta(0, 0.01))
                {
                    IsVisible = true;
                }
                else if (LastFinalVerticalOffset < RelatedScrollViewer.VerticalOffset)
                {
                    IsVisible = false;
                }
                LastFinalVerticalOffset = RelatedScrollViewer.VerticalOffset;
            }

            // Save the current vertical offset
            BackupVerticalOffset = RelatedScrollViewer.VerticalOffset;
        }

        private bool _IsViewChangedEnabled = true;

        /// <summary>
        /// <para>Gets or sets the value that indicates whether or not the control</para>
        /// <para>will hide/show when the user scrolls on the ScrollViewer</para>
        /// </summary>
        public bool IsViewChangedEnabled
        {
            get { return _IsViewChangedEnabled; }
            set
            {
                if (value) BackupVerticalOffset = RelatedScrollViewer.VerticalOffset;
                this._IsViewChangedEnabled = value;
            }
        }
    }
}
