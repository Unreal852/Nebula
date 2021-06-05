using System.Windows.Input;
using HandyControl.Controls;
using HandyControl.Interactivity;
using HandyControl.Tools.Extension;
using LiteMVVM;
using LiteMVVM.Command;

namespace Nebula.ViewModel.Dialogs
{
    public abstract class BaseDialogViewModel : BaseViewModel
    {
        private string _dialogTitle;

        protected BaseDialogViewModel()
        {
            ConfirmCommand = new RelayCommand(OnConfirm);
        }

        public ICommand ConfirmCommand { get; }
        public Dialog   Dialog         { get; set; }

        public string Title
        {
            get => _dialogTitle;
            set => Set(ref _dialogTitle, value);
        }

        protected void Close() => Dialog.Close();

        protected abstract void OnConfirm();
    }
}