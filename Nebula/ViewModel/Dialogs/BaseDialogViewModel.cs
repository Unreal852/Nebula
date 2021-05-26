using System.Windows.Input;
using HandyControl.Controls;
using Nebula.MVVM;

namespace Nebula.ViewModel.Dialogs
{
    public abstract class BaseDialogViewModel : BaseViewModel
    {
        protected BaseDialogViewModel()
        {
            
        }
        
        public ICommand CloseDialogCommand { get; }

        protected void Close()
        {
            
        }
    }
}