using System;
using System.Windows;

namespace Nebula.View.Builder.Attributes
{
    public abstract class BasePropertySetterAttribute : Attribute
    {
        public BasePropertySetterAttribute()
        {
        }

        public abstract void Apply(FrameworkElement element);
    }
}