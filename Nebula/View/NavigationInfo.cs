using System;
using System.Windows.Controls;

namespace Nebula.View
{
    public class NavigationInfo
    {
        public static NavigationInfo Create(Type type, object param)           => new(type, param);
        public static NavigationInfo Create(UserControl control, object param) => new(control, param);

        public NavigationInfo(Type type, object param = null)
        {
            Type = type;
            Parameter = param;
        }

        public NavigationInfo(UserControl control, object param = null) : this(control.GetType(), param)
        {
            Control = control;
        }

        [NotNull]   public Type        Type           { get; }
        [CanBeNull] public UserControl Control        { get; }
        [CanBeNull] public object      Parameter      { get; }
        public             bool        NavigateIfSame { get; set; } = false;
    }
}