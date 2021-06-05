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
        /*
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
            PlaylistImportDialogView view = new PlaylistImportDialogView();
            ViewCache cache = BuildViewCacheFromViewModel<T>();
            cache.Container = view;
            cache.Parent.SetValue(Grid.RowProperty, 1);
            cache.Parent.Margin = new Thickness(5, 0, 5, 0);
            view.Container.Children.Add(cache.Parent);
            return cache;
        }

        public static ViewCache BuildViewCacheFromViewModel<T>() where T : BaseViewModel, new()
        {
            Type viewModelType = typeof(T);
            Dictionary<string, PropertyInfo> properties = GetProperties(viewModelType, PropertyBindingFlags);
            SimpleStackPanel containerPanel = new SimpleStackPanel {Orientation = Orientation.Vertical};
            ViewCache viewCache = new ViewCache(viewModelType, containerPanel);
            foreach (PropertyInfo propertyInfo in properties.Values)
            {
                ElementAttribute viewElementAttribute = propertyInfo.GetCustomAttribute<ElementAttribute>();
                if (viewElementAttribute == null)
                    continue;
                FrameworkElement frameworkElement = Activator.CreateInstance(viewElementAttribute.ElementType) as FrameworkElement;
                if (frameworkElement == null)
                    continue;
                foreach (BasePropertySetterAttribute propertySetter in propertyInfo.GetCustomAttributes<BasePropertySetterAttribute>())
                    propertySetter.Apply(frameworkElement);
                List<PropertyBindingInfo> bindings = new List<PropertyBindingInfo>
                {
                    new(GetDependencyProperty(viewElementAttribute.ElementType, viewElementAttribute.PropertyName), propertyInfo.Name)
                };

                foreach (BindAttribute attribute in propertyInfo.GetCustomAttributes<BindAttribute>())
                {
                    if(string.IsNullOrWhiteSpace(attribute.DependencyProperty) || string.IsNullOrWhiteSpace(attribute.TargetName))
                        continue;
                    DependencyProperty dep = GetDependencyProperty(viewElementAttribute.ElementType, attribute.DependencyProperty);
                    if(dep == null)
                        continue;
                }

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

        private static Dictionary<string, PropertyInfo> GetProperties(Type type, BindingFlags bindingFlags)
        {
            Dictionary<string, PropertyInfo> propertyInfos = new();
            foreach (PropertyInfo property in type.GetProperties(bindingFlags))
                propertyInfos.Add(property.Name, property);
            return propertyInfos;
        }
*/
    }
}