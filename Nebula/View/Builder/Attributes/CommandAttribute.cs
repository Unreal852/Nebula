using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Nebula.View.Builder.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class CommandAttribute : BasePropertySetterAttribute
    {
        public override void Apply(FrameworkElement element)
        {

        }
    }
}