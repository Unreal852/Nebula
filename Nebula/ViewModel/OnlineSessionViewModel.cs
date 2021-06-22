using LiteMVVM;
using LiteMVVM.Navigation;

namespace Nebula.ViewModel
{
    public class OnlineSessionViewModel : BaseViewModel, INavigable
    {
        private bool _isConnected;

        public OnlineSessionViewModel()
        {
            _isConnected = true; // Temporary to see view design
        }

        public bool IsConnected
        {
            get => _isConnected;
            set => Set(ref _isConnected, value);
        }

        public async void OnNavigated(object param)
        {
            if (param is string str)
            {
                // For discord join
            }
            else
            {
            }
        }
    }
}