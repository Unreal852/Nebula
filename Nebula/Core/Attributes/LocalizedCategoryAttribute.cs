using System.ComponentModel;

namespace Nebula.Core.Attributes
{
    public class LocalizedCategoryAttribute : CategoryAttribute
    {
        public LocalizedCategoryAttribute(string value) : base(value)
        {
            
        }
        
        protected override string GetLocalizedString(string value)
        {
            return NebulaClient.GetLang(value);
        }
    }
}