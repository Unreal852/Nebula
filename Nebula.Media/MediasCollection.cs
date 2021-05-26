using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Nebula.Media;
using Nebula.Utils.Collections;

namespace Nebula.Media
{
    public class MediasCollection<T> : PagesCollection<T> where T : IMediaInfo
    {
        public MediasCollection(ICollection<T> elements = null, int maxElementsPerPage = 10) : base(elements, maxElementsPerPage)
        {
            Elements.CollectionChanged += OnCollectionChanged;
        }

        public TimeSpan TotalDuration { get; private set; }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                {
                    if (e.NewItems != null)
                    {
                        foreach (T newMedia in e.NewItems)
                            TotalDuration += newMedia.Duration;
                    }

                    break;
                }
                case NotifyCollectionChangedAction.Remove:
                {
                    if (e.OldItems != null)
                    {
                        foreach (T newMedia in e.OldItems)
                            TotalDuration -= newMedia.Duration;
                    }

                    break;
                }
                case NotifyCollectionChangedAction.Replace:
                {
                    if (e.OldItems != null)
                    {
                        foreach (T newMedia in e.OldItems)
                            TotalDuration -= newMedia.Duration;
                    }

                    if (e.NewItems != null)
                    {
                        foreach (T newMedia in e.NewItems)
                            TotalDuration += newMedia.Duration;
                    }

                    break;
                }
                case NotifyCollectionChangedAction.Reset:
                {
                    TotalDuration = TimeSpan.Zero;
                    foreach (T mediaInfo in Elements)
                        TotalDuration += mediaInfo.Duration;
                    break;
                }
            }
        }
    }
}