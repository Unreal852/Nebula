using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using HandyControl.Tools;

namespace Nebula.Core
{
    public class ObservableCollectionEx<T> : ObservableCollection<T>
    {
        /// <summary>
        ///     Gets or sets a property that determines if we are delaying notifications on updates.
        /// </summary>
        public bool DisableOnCollectionChangedNotification { get; set; }


        /// <summary>
        ///     Add a range of IEnumerable items to the observable collection and optionally delay notification until the operation is complete.
        /// </summary>
        /// <param name="items"></param>
        /// <param name="delayCollectionChangedNotification">Value indicating whether delay notification will be turned on/off</param>
        public void AddRange(IEnumerable<T> items, bool delayCollectionChangedNotification = true)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            DoDispatchedAction(() =>
            {
                DisableOnCollectionChangedNotification = delayCollectionChangedNotification;
                var enumerable = items as T[] ?? items.ToArray();
                if (enumerable.Any())
                    try
                    {
                        foreach (var item in enumerable)
                            Add(item);
                    }
                    finally
                    {
                        DisableOnCollectionChangedNotification = false;
                        OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                    }
            });
        }

        /// <summary>
        ///     Clear the items in the ObservableCollection and optionally delay notification until the operation is complete.
        /// </summary>
        public void ClearItems(bool delayCollectionChangedNotification = true)
        {
            // Do we have anything to remove?
            if (!this.Any())
                return;

            DoDispatchedAction(() =>
            {
                try
                {
                    DisableOnCollectionChangedNotification = delayCollectionChangedNotification;
                    Clear();
                }
                finally
                {
                    DisableOnCollectionChangedNotification = false;
                    OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                }
            });
        }

        /// <summary>
        ///     Override the virtual OnCollectionChanged() method on the ObservableCollection class.
        /// </summary>
        /// <param name="e">Event arguments</param>
        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            DoDispatchedAction(() =>
            {
                if (!DisableOnCollectionChangedNotification)
                    base.OnCollectionChanged(e);
            });
        }

        /// <summary>
        ///     Makes sure 'action' is executed on the thread that owns the object. Otherwise, things will go boom.
        /// </summary>
        /// <param name="action">The action which should be executed</param>
        private static void DoDispatchedAction(Action action)
        {
            DispatcherHelper.RunOnMainThread(action);
        }
    }
}