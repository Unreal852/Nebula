using System.Windows.Input;
using System.Windows.Media;
using HandyControl.Controls;
using LiteMVVM;
using LiteMVVM.Command;

namespace Nebula.ViewModel.Dialogs
{
    public abstract class BaseDialogViewModel : BaseViewModel
    {
        private string          _dialogTitle;
        private string          _cancelButtonText;
        private string          _confirmButtonText;
        private bool            _cancelButtonVisible;
        private bool            _confirmButtonVisible;
        private SolidColorBrush _titleBrush;

        protected BaseDialogViewModel()
        {
            ConfirmCommand = new RelayCommand(OnConfirm);
            CancelButtonText = NebulaClient.GetLang("dialog_cancel");
            ConfirmButtonText = NebulaClient.GetLang("dialog_confirm");
            CancelButtonVisible = ConfirmButtonVisible = true;
        }

        public ICommand ConfirmCommand { get; }
        public Dialog   Dialog         { get; set; }

        public string Title
        {
            get => _dialogTitle;
            set => Set(ref _dialogTitle, value);
        }

        public string CancelButtonText
        {
            get => _cancelButtonText;
            set => Set(ref _cancelButtonText, value);
        }

        public string ConfirmButtonText
        {
            get => _confirmButtonText;
            set => Set(ref _confirmButtonText, value);
        }

        public bool CancelButtonVisible
        {
            get => _cancelButtonVisible;
            set => Set(ref _cancelButtonVisible, value);
        }

        public bool ConfirmButtonVisible
        {
            get => _confirmButtonVisible;
            set => Set(ref _confirmButtonVisible, value);
        }

        public SolidColorBrush TitleBrush
        {
            get => _titleBrush;
            set => Set(ref _titleBrush, value);
        }

        protected void Close() => Dialog.Close();

        protected abstract void OnConfirm();
    }
}