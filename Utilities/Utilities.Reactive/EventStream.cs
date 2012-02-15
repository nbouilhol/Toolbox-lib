using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reactive.Subjects;
using Utilities.Functional;
using Utilities.Extensions;

namespace Utilities.Reactive
{
    partial class EventStream : IEventStream
    {
        private ConcurrentDictionary<Type, Subject<dynamic>> subjects = new ConcurrentDictionary<Type, Subject<dynamic>>();

        public void Push<TEvent>(TEvent @event)
        {
            if (@event == null) throw new ArgumentNullException("@event", "Parameter cannot be null.");

            Type eventType = @event.GetType();

            subjects.Keys.Where(subjectEventType => subjectEventType.IsAssignableFrom(eventType))
                .Select(subjectEventType => subjects.TryGetValue(subjectEventType))
                .ForEach(subject => subject.OnNext(@event));
        }

        public IObservable<TEvent> Of<TEvent>()
        {
            return subjects.GetOrAdd(typeof(TEvent), type => new Subject<TEvent>() as Subject<dynamic>) as IObservable<TEvent>;
        }
    }
}
