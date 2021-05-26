using System;
using System.Diagnostics;
using System.Windows.Input;
using HandyControl.Controls;
using HandyControl.Tools;
using HandyControl.Tools.Command;
using Nebula.MVVM;

namespace Nebula.ViewModel
{
    public class SideBarViewModel : BaseViewModel
    {
        public static SideBarViewModel Instance { get; private set; }

        public SideBarViewModel()
        {
            Instance = this;
            NavigateCommand = new RelayCommand(MainWindowViewModel.Instance.Navigate);
        }

        public ICommand NavigateCommand { get; }
    }
}