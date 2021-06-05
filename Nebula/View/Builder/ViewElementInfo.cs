using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace Nebula.View.Builder
{
    public class ViewElementInfo
    {
        public ViewElementInfo(FrameworkElement element, params PropertyBindingInfo[] properties)
        {
            Element = element;
            Properties = properties;
        }

        public FrameworkElement      Element    { get; }
        public PropertyBindingInfo[] Properties { get; }

        public void SetBinding(object context)
        {
            if (Properties is not {Length: > 0})
                return;
            foreach (PropertyBindingInfo property in Properties)
            {
                Element.SetBinding(property.Property, new Binding
                {
                    Path = new PropertyPath(property.PropertyInfoName),
                    Source = context
                });
            }
        }
    }
}