using System.Collections.Generic;
using ReactiveUI;

namespace Utilities.Reactive.Extensions
{
    public static class EnumerableExtension
    {
        public static ReactiveCollection<T> Replace<T>(this ReactiveCollection<T> reactiveCollection, IEnumerable<T> collectionToAdd)
        {
            if (reactiveCollection == null) return new ReactiveCollection<T>(collectionToAdd);
            reactiveCollection.Clear();
            foreach (T elemntToAdd in collectionToAdd) reactiveCollection.Add(elemntToAdd);
            return reactiveCollection;
        }
    }
}