using System.Windows.Controls;
using Nebula.Core.Extensions;
using Nebula.ViewModel;

namespace Nebula.View.Views
{
    public partial class OnlineSessionView : UserControl
    {
        public OnlineSessionView()
        {
            InitializeComponent();
            if (DataContext is OnlineSessionViewModel viewModel)
            {
                viewModel.Messages.CollectionChanged += (_, _) =>
                {
                    MessageList.UpdateLayout();
                    MessageList.GetChildOfType<ScrollViewer>()?.ScrollToBottom();
                };
            }
        }
    }
}