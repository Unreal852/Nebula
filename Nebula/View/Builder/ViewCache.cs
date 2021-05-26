using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Nebula.MVVM;

namespace Nebula.View.Builder
{
    public class ViewCache
    {
        public ViewCache(Type viewType, Panel parent)
        {
            ViewType = viewType;
            Parent = parent;
        }

        public  Type                  ViewType { get; }
        public  Panel                 Parent   { get; }
        private List<ViewElementInfo> Elements { get; } = new();

        public IEnumerable<FrameworkElement> GetFrameworkElements() => Elements.Select(elementInfo => elementInfo.Element);

        public void AddElement(ViewElementInfo elementInfo)
        {
            Elements.Add(elementInfo);
            Parent.Children.Add(elementInfo.Element);
        }

        public void PrepareFor(BaseViewModel viewModel)
        {
            foreach (ViewElementInfo viewElementInfo in Elements)
                viewElementInfo.SetBinding(viewModel);
        }
    }
}