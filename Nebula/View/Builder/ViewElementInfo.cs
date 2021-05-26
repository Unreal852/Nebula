using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace Nebula.View.Builder
{
    public class ViewElementInfo
    {
        public ViewElementInfo(FrameworkElement element, DependencyProperty dependencyProperty, string propertyName)
        {
            Element = element;
            DependencyProperty = dependencyProperty;
            PropertyName = propertyName;
        }

        public FrameworkElement   Element            { get; }
        public DependencyProperty DependencyProperty { get; }
        public string             PropertyName       { get; }

        public void SetBinding(object context)
        {
            Element.SetBinding(DependencyProperty, new Binding
            {
                Path = new PropertyPath(PropertyName),
                Source = context
            });
        }
    }
}