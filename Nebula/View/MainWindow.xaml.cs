using System.Windows;
using System.Windows.Controls;
using System.Windows.Shell;

namespace Nebula.View
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            SetTitlebarElementsClickable();
        }

        private void SetTitlebarElementsClickable()
        {
            foreach (IInputElement element in TitleBar.Children)
                WindowChrome.SetIsHitTestVisibleInChrome(element, true);
            foreach (IInputElement element in TitleBarButtons.Children)
                WindowChrome.SetIsHitTestVisibleInChrome(element, true);
        }

        private void OnControlBoxClick(object sender, RoutedEventArgs e)
        {
            if (sender is not Button button)
                return;
            switch (button.Tag.ToString())
            {
                case "-":
                    WindowState = WindowState.Minimized;
                    break;
                case "+" when WindowState == WindowState.Maximized:
                    WindowState = WindowState.Normal;
                    break;
                case "+" when WindowState == WindowState.Normal:
                    WindowState = WindowState.Maximized;
                    break;
                case "x" when NebulaClient.Settings.General.CloseToTray:
                {
                    Visibility = Visibility.Collapsed;
                    break;
                }
                case "x" when !NebulaClient.Settings.General.CloseToTray:
                {
                    Close();
                    break;
                }
                
            }
        }
    }
}