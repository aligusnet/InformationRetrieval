﻿using System;
using System.Collections;
using System.Collections.Generic;

namespace InformationRetrieval.Utility
{
    /// <summary>
    /// Respresents a list of lists as one continues collection.
    /// </summary>
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
    public class ListChain<T> : IEnumerable<T>, IReadOnlyCollection<T>
    {
        private readonly List<IReadOnlyCollection<T>> chain;

        public ListChain(): this(0) { }

        public ListChain(int capacity)
        {
            chain = new List<IReadOnlyCollection<T>>(capacity);
            Count = 0;
        }

        public void Add(IReadOnlyCollection<T> node)
        {
            chain.Add(node);
            Count += node.Count;
        }

        public void Sort(Comparison<IReadOnlyCollection<T>> comparison) =>
            chain.Sort(comparison);

        public int Count { get; private set; }

        public void Clear()
        {
            Count = 0;
            chain.Clear();
        }

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

        public IReadOnlyList<IReadOnlyCollection<T>> Chains => chain;

        IEnumerator IEnumerable.GetEnumerator() =>
            GetEnumerator();
    }
}
