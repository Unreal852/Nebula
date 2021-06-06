using System;
using System.Collections.Generic;
using Nebula.Core.Providers.Youtube;
using Nebula.Media;

namespace Nebula.Core.Providers
{
    public class ProvidersManager
    {
        public ProvidersManager()
        {
            RegisterProvider(new YoutubeMediaProvider());
        }

        private List<IMediasProvider> Providers      { get; } = new();
        public  int                   ProvidersCount => Providers.Count;

        public void RegisterProvider(IMediasProvider provider)
        {
            if (Providers.Contains(provider))
                return;
            Providers.Add(provider);
        }

        public void UnregisterProvider(IMediasProvider provider)
        {
            if (!Providers.Contains(provider))
                return;
            Providers.Remove(provider);
        }

        public T FindProviderByType<T>()
        {
            foreach (IMediasProvider provider in Providers)
            {
                if (provider is T type)
                    return type;
            }

            return default;
        }

        public IMediasProvider FindProviderByName(string name, bool ignoreCase = true)
        {
            foreach (IMediasProvider provider in Providers)
            {
                if (String.Equals(provider.Name, name, ignoreCase ? StringComparison.CurrentCultureIgnoreCase : StringComparison.CurrentCulture))
                    return provider;
            }

            return default;
        }

        public async IAsyncEnumerable<IMediaInfo> SearchMedias(string query, params object[] args)
        {
            foreach (IMediasProvider mediasProvider in Providers)
            {
                await foreach (IMediaInfo mediaInfo in mediasProvider.SearchMedias(query, args))
                    yield return mediaInfo;
            }
        }
    }
}