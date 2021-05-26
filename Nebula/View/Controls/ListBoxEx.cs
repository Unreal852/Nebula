using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Nebula.View.Controls
{
    public class ListBoxEx : ListBox
    {
        private ScrollViewer _scrollViewer;

        public ScrollViewer ScrollViewer => _scrollViewer ??= GetScrollViewer();

        private ScrollViewer GetScrollViewer()
        {
            Decorator border = VisualTreeHelper.GetChild(this, 0) as Decorator;
            ScrollViewer scrollViewer = border.Child as ScrollViewer;
            return scrollViewer;
        }

        public static T GetDescendantByType<T>(Visual element) where T:class
        {
            if (element == null)
            {
                return default(T);
            }
            if (element.GetType() == typeof(T))
            {
                return element as T;
            }
            T foundElement = null;
            if (element is FrameworkElement)
            {
                (element as FrameworkElement).ApplyTemplate();
            }
            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
            {
                var visual = VisualTreeHelper.GetChild(element, i) as Visual;
                foundElement = GetDescendantByType<T>(visual);
                if (foundElement != null)
                {
                    break;
                }
            }
            return foundElement;
        }
    }
}