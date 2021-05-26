using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using HandyControl.Controls;
using Nebula.MVVM;
using Nebula.View.Builder.Attributes;
using TextBox = HandyControl.Controls.TextBox;

namespace Nebula.View.Builder
{
    public static class ViewBuilder
    {
        private static readonly BindingFlags PropertyBindingFlags =
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.SetProperty;

        private static readonly Dictionary<Type, DependencyProperty> DependencyProperties = new();

        public static void RegisterDependencyPropertyForType(Type type, DependencyProperty dependencyProperty)
        {
            if (DependencyProperties.ContainsKey(type))
                return;
            DependencyProperties.Add(type, dependencyProperty);
        }

        public static FrameworkElement BuildDialogViewFromViewModel<T>() where T : BaseViewModel
        {
            throw new NotImplementedException();
        }

        public static ViewCache BuildViewCacheFromViewModel<T>() where T : BaseViewModel, new()
        {
            Type viewModelType = typeof(T);
            PropertyInfo[] properties = viewModelType.GetProperties(PropertyBindingFlags);
            SimpleStackPanel containerPanel = new SimpleStackPanel {Orientation = Orientation.Vertical};
            ViewCache viewCache = new ViewCache(viewModelType, containerPanel);
            foreach (PropertyInfo propertyInfo in properties)
            {
                ElementAttribute viewElementAttribute = propertyInfo.GetCustomAttribute<ElementAttribute>();
                if (viewElementAttribute == null)
                    continue;
                FrameworkElement frameworkElement = Activator.CreateInstance(viewElementAttribute.ElementType) as FrameworkElement;
                if (frameworkElement == null)
                    continue;
                foreach (BasePropertySetterAttribute propertySetter in propertyInfo.GetCustomAttributes<BasePropertySetterAttribute>())
                    propertySetter.Apply(frameworkElement);
                viewCache.AddElement(new ViewElementInfo(frameworkElement, GetDependencyProperty(viewElementAttribute.ElementType,
                    viewElementAttribute.PropertyName), propertyInfo.Name));
            }

            return viewCache;
        }

        public static DependencyProperty GetDependencyProperty(Type type, string name)
        {
            FieldInfo fieldInfo = type.GetField(name, BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
            return fieldInfo?.GetValue(null) as DependencyProperty;
        }
    }
}