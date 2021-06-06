using System;
using HandyControl.Tools.Extension;

namespace Nebula.ViewModel.Dialogs
{
    public class MessageDialogViewModel : BaseDialogViewModel, IDialogResultable<bool>
    {
        private MessageDialog _dialogType;
        private string        _message;

        public string Message
        {
            get => _message;
            set => Set(ref _message, value);
        }

        public MessageDialog DialogType
        {
            get => _dialogType;
            set
            {
                _dialogType = value;
                CancelButtonVisible = ConfirmButtonVisible = true;
                switch (DialogType)
                {
                    case MessageDialog.CancelConfirm:
                    {
                        CancelButtonText = NebulaClient.GetLang("dialog_cancel");
                        ConfirmButtonText = NebulaClient.GetLang("dialog_confirm");
                        break;
                    }
                    case MessageDialog.NoYes:
                    {
                        CancelButtonText = NebulaClient.GetLang("dialog_no");
                        ConfirmButtonText = NebulaClient.GetLang("dialog_yes");
                        ConfirmButtonText = NebulaClient.GetLang("dialog_yes");
                        break;
                    }
                    case MessageDialog.Ok:
                    {
                        ConfirmButtonVisible = false;
                        CancelButtonText = NebulaClient.GetLang("dialog_ok");
                        break;
                    }
                }
            }
        }

        public bool   Result      { get; set; }
        public Action CloseAction { get; set; }

        protected override void OnConfirm()
        {
            Result = true;
            Close();
        }
    }

    public enum MessageDialog
    {
        CancelConfirm,
        NoYes,
        Ok
    }
}