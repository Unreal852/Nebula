using System;
using System.Windows;

namespace Nebula.View.Builder.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class)]
    public class SizeAttribute : BasePropertySetterAttribute
    {
        public SizeAttribute(double width = -1, double height = -1)
        {
            Width = width;
            Height = height;
        }

        public double Width  { get; }
        public double Height { get; }

        public override void Apply(FrameworkElement element)
        {
            if (Width > -1)
                element.Width = Width;
            if (Height > -1)
                element.Height = Height;
        }
    }
}