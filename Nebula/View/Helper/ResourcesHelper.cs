using System.Windows;

namespace Nebula.View.Helper
{
    public static class ResourcesHelper
    {
        public static T GetResource<T>(string resourceName) where T : class
        {
            return Application.Current.TryFindResource(resourceName) as T;
        }
    }
}