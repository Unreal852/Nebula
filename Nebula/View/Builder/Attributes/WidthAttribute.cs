using System;
using System.Windows;
using HandyControl.Controls;

namespace Nebula.View.Builder.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class WidthAttribute : BasePropertySetterAttribute
    {
        public WidthAttribute(double width)
        {
            Width = width;
        }

        public          double Width                           { get; }
        public override void   Apply(FrameworkElement element) => element.Width = Width;
    }
}