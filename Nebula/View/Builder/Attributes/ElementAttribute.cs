using System;

namespace Nebula.View.Builder.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ElementAttribute : Attribute
    {
        public ElementAttribute(Type type, string dependencyPropertyName = "")
        {
            ElementType = type;
            PropertyName = dependencyPropertyName;
        }

        public Type   ElementType  { get; }
        public string PropertyName { get; }
        public bool   HasProperty  => !string.IsNullOrWhiteSpace(PropertyName);
    }
}