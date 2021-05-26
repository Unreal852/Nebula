using System;
using System.Windows;
using HandyControl.Controls;

namespace Nebula.View.Builder.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class TitleAttribute : BasePropertySetterAttribute
    {
        public TitleAttribute(string titleKey)
        {
            TitleKey = titleKey;
        }

        public          string TitleKey                        { get; }
        public override void   Apply(FrameworkElement element) => TitleElement.SetTitle(element, NebulaClient.GetLang(TitleKey));
    }
}