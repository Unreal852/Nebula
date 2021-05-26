using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Nebula.Utils.Collections;

namespace Nebula.Media
{
    public class MediasCollection : PagesCollection<IMediaInfo>
    {
        public MediasCollection(ICollection<IMediaInfo> elements = null, int maxElementsPerPage = 10) : base(elements, maxElementsPerPage)
        {
            Elements.CollectionChanged += OnCollectionChanged;
        }

        public TimeSpan TotalDuration { get; private set; }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            TotalDuration = TimeSpan.Zero;
            foreach (IMediaInfo mediaInfo in Elements)
                TotalDuration += mediaInfo.Duration;
        }
    }
}