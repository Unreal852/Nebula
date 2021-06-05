using System.Windows;

namespace Nebula.View.Builder
{
    public class PropertyBindingInfo
    {
        public PropertyBindingInfo(DependencyProperty property, string propertyInfoName)
        {
            Property = property;
            PropertyInfoName = propertyInfoName;
        }

        public DependencyProperty Property         { get; }
        public string             PropertyInfoName { get; }
    }
}