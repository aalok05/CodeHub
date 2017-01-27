using System.Collections;
using System.Linq;
using System.Runtime.CompilerServices;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using JetBrains.Annotations;

namespace CodeHub.Helpers
{
    /// <summary>
    /// A collection of various extension methods
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Casts the given object into the Type in input via a direct cast and returns it
        /// </summary>
        /// <typeparam name="T">The target Type</typeparam>
        /// <param name="target">The object to cast into the given Type</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T To<T>(this object target) => (T)target;

        /// <summary>
        /// Returns the first element of a specific type in the visual tree of a DependencyObject
        /// </summary>
        /// <typeparam name="T">The type of the element to find</typeparam>
        /// <param name="parent">The object that contains the UIElement to find</param>
        public static T FindChild<T>([NotNull] this DependencyObject parent) where T : UIElement
        {
            if (parent is T) return parent.To<T>();
            int children = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < children; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);
                if (!(child is T))
                {
                    T tChild = FindChild<T>(child);
                    if (tChild != null) return tChild;
                }
                else return child as T;
            }
            return null;
        }

        /// <summary>
        /// Scrolls an input ListView to the top, if possible
        /// </summary>
        /// <param name="listView">The target list to scroll</param>
        public static bool ScrollToTheTop([NotNull] this ListView listView)
        {
            ScrollViewer scrollViewer = listView.FindChild<ScrollViewer>();
            if (scrollViewer == null) return false;
            scrollViewer.ChangeView(null, 0, null, false);
            return true;
        }

        /// <summary>
        /// Counts the number of items in an IEnumerable sequence
        /// </summary>
        /// <param name="enumerable">The sequence to count</param>
        public static int Count([NotNull] this IEnumerable enumerable) => Enumerable.Count(enumerable.Cast<object>());
    }
}
