using System.Collections.Generic;
using Nebula.Core.Providers.Youtube;
using Nebula.Model;

namespace Nebula.Core.Providers
{
    public class ProvidersManager
    {
        public ProvidersManager()
        {
            RegisterProvider(new YoutubeMediaProvider());
        }

        private Dictionary<string, IMediasProvider> Providers      { get; } = new();
        public  int                                 ProvidersCount => Providers.Count;

        public void RegisterProvider(IMediasProvider provider)
        {
            if (Providers.ContainsValue(provider))
                return;
            Providers.Add(provider.Name, provider);
        }

        public void UnregisterProvider(IMediasProvider provider)
        {
            if (!Providers.ContainsValue(provider))
                return;
            Providers.Remove(provider.Name);
        }

        public T FindProviderByType<T>()
        {
            foreach (IMediasProvider provider in Providers.Values)
                if (provider is T type)
                    return type;
            return default;
        }

        public IMediasProvider GetProvider(string name)
        {
            return Providers.ContainsKey(name) ? Providers[name] : default;
        }

        public async IAsyncEnumerable<MediaInfo> SearchMedias(string query, params object[] args)
        {
            foreach (IMediasProvider mediasProvider in Providers.Values)
            await foreach (MediaInfo mediaInfo in mediasProvider.SearchMedias(query, args))
                yield return mediaInfo;
        }

        public async IAsyncEnumerable<Playlist> SearchPlaylists(string query, params object[] args)
        {
            foreach (IMediasProvider mediasProvider in Providers.Values)
            await foreach (Playlist playlist in mediasProvider.SearchPlaylists(query, args))
                yield return playlist;
        }
    }
}