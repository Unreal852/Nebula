using System;
using System.Windows.Controls;

namespace Nebula.View
{
    public class NavigationInfo
    {
        public static NavigationInfo Create(Type type, object param, bool navigateIfSame)           => new(type, param, navigateIfSame);
        public static NavigationInfo Create(UserControl control, object param, bool navigateIfSame) => new(control, param, navigateIfSame);

        public NavigationInfo(Type type, object param = null, bool navigateIfSame = false)
        {
            Type = type;
            Parameter = param;
            NavigateIfSame = navigateIfSame;
        }

        public NavigationInfo(UserControl control, object param = null, bool navigateIfSame = false) : this(control.GetType(), param, navigateIfSame)
        {
            Control = control;
        }

        [NotNull]   public Type        Type           { get; }
        [CanBeNull] public UserControl Control        { get; }
        [CanBeNull] public object      Parameter      { get; }
        public             bool        NavigateIfSame { get; }
    }
}