using System;
using System.Collections.Generic;

namespace Nebula.MVVM
{
    public class ViewModelLink
    {
        public ViewModelLink(Guid guid)
        {
            Guid = guid;
        }

        private Guid                 Guid        { get; }
        private List<Action<object>> Subscribers { get; } = new();

        public void Subscribe(Action<object> action) => Subscribers.Add(action);
        public void Send(object @object)             => Subscribers.ForEach(action => action(@object));
    }
}