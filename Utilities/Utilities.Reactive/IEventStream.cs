using System;

namespace Utilities.Reactive
{
    partial interface IEventStream
    {
        void Push<TEvent>(TEvent @event);
        IObservable<TEvent> Of<TEvent>();
    }
}