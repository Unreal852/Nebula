using System.Windows.Input;
using HandyControl.Interactivity;
using LiteMVVM;
using LiteMVVM.Command;

namespace Nebula.ViewModel.Dialogs
{
    public abstract class BaseDialogViewModel : BaseViewModel
    {
        protected BaseDialogViewModel()
        {
            ConfirmCommand = new RelayCommand<object>(OnConfirm);
        }

        public ICommand ConfirmCommand { get; }

        protected void Close() => ControlCommands.Close.Execute(null, null);

        protected abstract void OnConfirm(object param);
    }
}