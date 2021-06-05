using System;
using System.Windows;
using System.Windows.Controls;

namespace Nebula.View.Builder.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class)]
    public class ContentAttribute : BasePropertySetterAttribute
    {
        public ContentAttribute(string contentKey)
        {
            ContentKey = contentKey;
        }

        public string ContentKey { get; }

        public override void Apply(FrameworkElement element)
        {
            if (element is ContentControl content)
                content.Content = NebulaClient.GetLang(ContentKey);
        }
    }
}