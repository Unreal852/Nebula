using System.Windows.Controls;
using HandyControl.Data;
using Nebula.ViewModel;

namespace Nebula.View.Views
{
    public partial class PlaylistView : UserControl
    {
        public PlaylistView()
        {
            InitializeComponent();
        }

        private void OnPageUpdated(object sender, FunctionEventArgs<int> e)
        {
            if (DataContext is PlaylistViewModel vm)
                vm.CurrentPage = e.Info;
        }
    }
}