using System.Collections;
using System.Collections.Generic;

namespace InformationRetrieval.Utility
{
    public class ReadOnlyCollection<T> : IReadOnlyCollection<T>
    {
        private readonly IEnumerable<T> enumerable;

        public ReadOnlyCollection(int count, IEnumerable<T> enumerable)
        {
            this.Count = count;
            this.enumerable = enumerable;
        }

        public int Count { get; }

        public IEnumerator<T> GetEnumerator() 
            => enumerable.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => enumerable.GetEnumerator();
    }
}
