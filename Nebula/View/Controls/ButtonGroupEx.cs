using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using HandyControl.Controls;

namespace Nebula.View.Controls
{
    public class ButtonGroupEx : ItemsControl
    {
        public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register(nameof(Orientation), typeof(Orientation), typeof(ButtonGroupEx),
            new PropertyMetadata((object) Orientation.Horizontal));

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            switch (item)
            {
                case Button _:
                case RadioButton _:
                    return true;
                default:
                    return item is ToggleButton;
            }
        }

        public Orientation Orientation
        {
            get => (Orientation) GetValue(ButtonGroup.OrientationProperty);
            set => SetValue(ButtonGroup.OrientationProperty, value);
        }

        protected override void OnVisualChildrenChanged(
            DependencyObject visualAdded,
            DependencyObject visualRemoved)
        {
            base.OnVisualChildrenChanged(visualAdded, visualRemoved);
            int count = Items.Count;
            // This fix the cast exception. This default control does not handle ItemTemplate and try to cast the model to a ButtonBase
            if (count <= 0 || Items[0] is not ButtonBase)
                return;
            for (int index = 0; index < count; ++index)
            {
                ButtonBase buttonBase = (ButtonBase) Items[index];
                buttonBase.Style = ItemContainerStyleSelector?.SelectStyle(buttonBase, this);
            }
        }
    }
}