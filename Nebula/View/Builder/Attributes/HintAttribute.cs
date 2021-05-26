using System;
using System.Windows;
using HandyControl.Controls;

namespace Nebula.View.Builder.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class HintAttribute : BasePropertySetterAttribute
    {
        public HintAttribute(string hintKey)
        {
            HintKey = hintKey;
        }

        public string HintKey { get; }

        public override void Apply(FrameworkElement element) => InfoElement.SetPlaceholder(element, NebulaClient.GetLang(HintKey));
    }
}