using System.Windows.Input;
using HandyControl.Interactivity;
using Nebula.MVVM;
using Nebula.MVVM.Commands;

namespace Nebula.ViewModel.Dialogs
{
    public abstract class BaseDialogViewModel : BaseViewModel
    {
        protected BaseDialogViewModel()
        {
            ConfirmCommand = new RelayCommand(OnConfirm);
        }

        public ICommand ConfirmCommand { get; }

        protected void Close() => ControlCommands.Close.Execute(null, null);

        protected abstract void OnConfirm(object param);
    }
}