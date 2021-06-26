using System.ComponentModel;

namespace Nebula.Core.Attributes
{
    public class LocalizedDisplayNameAttribute : DisplayNameAttribute
    {
        public LocalizedDisplayNameAttribute(string value) : base(value)
        {
            
        }
        
        public override string DisplayName => NebulaClient.GetLang(DisplayNameValue);
    }
}