using System;

namespace Nebula.View.Builder.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class BindAttribute : Attribute
    {
        public BindAttribute(string dependencyProperty, string targetName, bool isCommand)
        {
            DependencyProperty = dependencyProperty;
            TargetName = targetName;
            IsCommand = isCommand;
        }

        public string DependencyProperty { get; }
        public string TargetName         { get; }
        public bool   IsCommand          { get; }
    }
}