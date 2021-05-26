using System;
using System.Collections.Generic;
using Nebula.Utils;

namespace Nebula.MVVM
{
    public static class ViewModelLinkManager
    {
        private static Dictionary<Guid, ViewModelLink> Links { get; } = new();

        public static ViewModelLink RegisterLink(string name)
        {
            Guid guid = GuidUtils.Create(GuidUtils.UrlNamespace, name);
            if (!Links.ContainsKey(guid))
            {
                ViewModelLink link = new ViewModelLink(guid);
                Links.Add(guid, link);
            }

            return Links[guid];
        }
    }
}