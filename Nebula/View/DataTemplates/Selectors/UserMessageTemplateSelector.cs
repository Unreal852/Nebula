using System.Windows;
using System.Windows.Controls;
using Nebula.Model;
using Nebula.View.Helper;

namespace Nebula.View.DataTemplates.Selectors
{
    public class UserMessageTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is UserMessage message)
            {
                return message.Message.MessageType switch
                {
                    1 => ResourcesHelper.GetResource<DataTemplate>("MessageMediaItemTemplate"),
                    _ => ResourcesHelper.GetResource<DataTemplate>("MessageItemTemplate"),
                };
            }

            return ResourcesHelper.GetResource<DataTemplate>("MessageItemTemplate");
        }
    }
}