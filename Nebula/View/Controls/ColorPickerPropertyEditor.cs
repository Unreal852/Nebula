using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using HandyControl.Controls;
using HandyControl.Tools;

namespace Nebula.View.Controls
{
    public class ColorPickerPropertyEditor : PropertyEditorBase
    {
        private static readonly SolidColorBrush WhiteBrush = new(Colors.White);
        private static readonly SolidColorBrush BlackBrush = new(Colors.Black);

        public override FrameworkElement CreateElement(PropertyItem propertyItem)
        {
            var button = new Button {HorizontalAlignment = HorizontalAlignment.Left};
            button.Loaded += OnButtonLoaded;
            button.Click += OnButtonClick;
            return button;
        }

        public override DependencyProperty GetDependencyProperty()
        {
            return ContentControl.ContentProperty;
        }

        private static void OnButtonLoaded(object sender, RoutedEventArgs e)
        {
            var button = (Button) sender;
            if (button.Content is SolidColorBrush brush)
            {
                button.Background = brush;
                button.Foreground = GetBrightness(brush.Color) < 0.55 ? WhiteBrush : BlackBrush;
            }

            button.Loaded -= OnButtonLoaded;
        }

        private static void OnButtonClick(object sender, RoutedEventArgs e)
        {
            var button = (Button) sender;
            var picker = SingleOpenHelper.CreateControl<ColorPicker>();
            var window = new PopupWindow {PopupElement = picker};
            picker.SelectedBrush = button.Content as SolidColorBrush;
            picker.Canceled += PickerCanceled;
            picker.Confirmed += PickerConfirmed;

            void PickerCanceled(object sender, EventArgs e)
            {
                window.Close();
                picker.Canceled -= PickerCanceled;
            }

            void PickerConfirmed(object sender, EventArgs e)
            {
                button.Content = picker.SelectedBrush;
                button.Background = picker.SelectedBrush;
                button.Foreground = new SolidColorBrush(GetBrightness(picker.SelectedBrush.Color) < 0.55 ? Colors.White : Colors.Black);
                picker.Confirmed -= PickerConfirmed;
            }

            window.Show(button, false);
        }

        private static float GetBrightness(Color c)
        {
            return (c.R * 0.299f + c.G * 0.587f + c.B * 0.114f) / 256f;
        }
    }
}