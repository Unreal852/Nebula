using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using HandyControl.Controls;
using SharpToolbox.Reflection.Fast;
using MessageBox = System.Windows.MessageBox;

namespace Nebula.View.Views
{
    public partial class SettingsView : UserControl
    {
        public SettingsView()
        {
            InitializeComponent();
        }
    }
    
    // All the below is to have a custom property grid behaviour but subclassing it break the renderer
    // See : https://github.com/HandyOrg/HandyControl/issues/812
    // Todo

    [TemplatePart(Name = "PART_ItemsControl", Type = typeof(ItemsControl))]
    [TemplatePart(Name = "PART_SearchBar", Type = typeof(SearchBar))]
    public class PropertyGridEx : PropertyGrid
    {
        public PropertyGridEx()
        {
            DataView = FastAccessor.GetField<PropertyGrid, ICollectionView>("_dataView", BindingFlags.Instance | BindingFlags.NonPublic);
        }

        public FastFieldInfo<PropertyGrid, ICollectionView> DataView { get; }
    }

    public class PropertyItemEx : PropertyItem
    {
        public static readonly DependencyProperty OrderProperty =
            DependencyProperty.Register(nameof(Order), typeof(int), typeof(PropertyItem), new PropertyMetadata(0));

        public int Order
        {
            get => (int) GetValue(OrderProperty);
            set => SetValue(OrderProperty, value);
        }
    }

    public class PropertyResolverEx : PropertyResolver
    {
    }
}