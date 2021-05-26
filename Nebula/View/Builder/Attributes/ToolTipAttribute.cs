using System;
using System.Windows;

namespace Nebula.View.Builder.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ToolTipAttribute : BasePropertySetterAttribute
    {
        public ToolTipAttribute(string toolTipKey)
        {
            ToolTipKey = toolTipKey;
        }

        public          string ToolTipKey                      { get; }
        public override void   Apply(FrameworkElement element) => element.ToolTip = NebulaClient.GetLang(ToolTipKey);
    }
}