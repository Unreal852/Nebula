using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using HandyControl.Controls;
using HandyControl.Tools;
using LiteMVVM.Command;
using Nebula.View.Helper;
using TextBox = System.Windows.Controls.TextBox;

namespace Nebula.View.Controls
{
    public class ColorPickerPropertyEditor : PropertyEditorBase
    {
        public override FrameworkElement CreateElement(PropertyItem propertyItem)
        {
            var button = new Button() {HorizontalAlignment = HorizontalAlignment.Left};
            button.Click += (_, _) =>
            {
                var picker = SingleOpenHelper.CreateControl<ColorPicker>();
                picker.SelectedBrush = button.Content as SolidColorBrush;
                var window = new PopupWindow {PopupElement = picker};
                picker.Canceled += (_, _) => { window.Close(); };
                picker.Confirmed += (_, _) =>
                {
                    button.Content = picker.SelectedBrush;
                    window.Close();
                };
                window.Show(button, false);
            };
            //var colorPicker = new ColorPicker {IsEnabled = !propertyItem.IsReadOnly};
            return button;
        }

        public override DependencyProperty GetDependencyProperty()
        {
            return ContentControl.ContentProperty;
        }
    }
}