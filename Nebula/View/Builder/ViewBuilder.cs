using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using HandyControl.Controls;
using LiteMVVM;
using Nebula.View.Builder.Attributes;
using Nebula.View.Views.Dialogs;
using Nebula.ViewModel.Dialogs;

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

        public static ViewCache BuildDialogViewCacheFromViewModel<T>() where T : BaseDialogViewModel, new()
        {
            BaseDialogView view = new BaseDialogView();
            ViewCache cache = BuildViewCacheFromViewModel<T>();
            cache.Container = view;
            cache.Parent.SetValue(Grid.RowProperty, 0);
            view.Container.Children.Add(cache.Parent);
            return cache;
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