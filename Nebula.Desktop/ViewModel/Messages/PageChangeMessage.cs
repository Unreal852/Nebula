using CommunityToolkit.Mvvm.Messaging.Messages;

namespace Nebula.Desktop.ViewModel.Messages;

public sealed class PageChangeMessage : ValueChangedMessage<ViewModelPageBase>
{
    public PageChangeMessage(ViewModelPageBase value) : base(value)
    {
    }
}