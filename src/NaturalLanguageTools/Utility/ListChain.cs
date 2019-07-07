using System;
using System.Collections;
using System.Collections.Generic;

namespace NaturalLanguageTools.Utility
{
    /// <summary>
    /// Respresents a list of lists as one continues collection.
    /// </summary>
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
    public class ListChain<T> : IEnumerable<T>, IReadOnlyCollection<T>
    {
        private readonly List<IList<T>> chain;

        public ListChain(): this(0) { }

        public ListChain(int capacity)
        {
            chain = new List<IList<T>>(capacity);
            Count = 0;
        }

        public void Add(IList<T> node)
        {
            chain.Add(node);
            Count += node.Count;
        }

        public void Sort(Comparison<IList<T>> comparison) =>
            chain.Sort(comparison);

        public int Count { get; private set; }

        public IEnumerator<T> GetEnumerator()
        {
            foreach (var node in chain)
            {
                foreach (var item in node)
                {
                    yield return item;
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator() =>
            GetEnumerator();
    }
}
