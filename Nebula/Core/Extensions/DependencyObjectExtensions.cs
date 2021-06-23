using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Nebula.Core.Extensions
{
    /// <summary>
    /// Provide extensions for <see cref="DependencyObject"/>
    /// </summary>
    public static class DependencyObjectExtensions
    {
        /// <summary>
        /// Find the child of the specified type
        /// </summary>
        /// <param name="depObj">The DependencyObject</param>
        /// <typeparam name="T">Object Type</typeparam>
        /// <returns>Object</returns>
        public static T GetChildOfType<T>(this DependencyObject depObj) where T : DependencyObject
        {
            if (depObj == null)
                return null;
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                T result = child as T ?? child.GetChildOfType<T>();
                if (result != null)
                    return result;
            }

            return null;
        }

        /// <summary>
        /// Remove the specified child
        /// </summary>
        /// <param name="parent">Parent</param>
        /// <param name="child">Child to remove</param>
        public static void RemoveChild(this DependencyObject parent, UIElement child)
        {
            switch (parent)
            {
                case Panel panel:
                    panel.Children.Remove(child);
                    return;
                case Decorator decorator:
                {
                    if (decorator.Child == child)
                        decorator.Child = null;
                    return;
                }
                case ContentPresenter contentPresenter:
                {
                    if (Equals(contentPresenter.Content, child))
                        contentPresenter.Content = null;
                    return;
                }
                case ContentControl contentControl:
                {
                    if (Equals(contentControl.Content, child))
                        contentControl.Content = null;
                    return;
                }
            }
        }
    }
}