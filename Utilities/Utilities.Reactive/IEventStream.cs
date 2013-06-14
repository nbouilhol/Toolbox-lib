using System;

namespace Utilities.Reactive
{
    internal partial interface IEventStream
    {
        void Push<TEvent>(TEvent @event);

        IObservable<TEvent> Of<TEvent>();
    }
}