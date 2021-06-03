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
    }
}