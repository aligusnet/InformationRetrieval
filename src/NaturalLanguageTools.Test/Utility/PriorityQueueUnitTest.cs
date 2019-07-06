using System;
using System.Collections.Generic;
using System.Linq;

using Xunit;

using NaturalLanguageTools.Utility;

namespace NaturalLanguageTools.Test.Utility
{
    public class HeapUnitTests
    {
        [Fact]
        public void HeapCommonFunctionsTest()
        {
            Assert.Equal(0, PriorityQueue<int>.Parent(1));
            Assert.Equal(0, PriorityQueue<int>.Parent(2));
            Assert.Equal(1, PriorityQueue<int>.Parent(3));
            Assert.Equal(1, PriorityQueue<int>.Parent(4));
            Assert.Equal(2, PriorityQueue<int>.Parent(5));
            Assert.Equal(2, PriorityQueue<int>.Parent(6));

            Assert.Equal(1, PriorityQueue<int>.Left(0));
            Assert.Equal(2, PriorityQueue<int>.Right(0));
            Assert.Equal(3, PriorityQueue<int>.Left(1));
            Assert.Equal(4, PriorityQueue<int>.Right(1));
            Assert.Equal(5, PriorityQueue<int>.Left(2));
            Assert.Equal(6, PriorityQueue<int>.Right(2));
            Assert.Equal(7, PriorityQueue<int>.Left(3));
            Assert.Equal(8, PriorityQueue<int>.Right(3));
            Assert.Equal(9, PriorityQueue<int>.Left(4));
            Assert.Equal(10, PriorityQueue<int>.Right(4));
        }

        [Fact]
        public void SortUsingPriorityQueueTest()
        {
            int numElements = 10;
            var rnd = new Random();
            var values = Enumerable.Range(0, numElements).OrderBy(x => rnd.Next()).ToArray();
            var expectedValues = Enumerable.Range(0, numElements).ToArray();

            var minHeapComparer = Comparer<int>.Create((x, y) => y.CompareTo(x));

            var heap = new PriorityQueue<int>(numElements, minHeapComparer);

            foreach (var n in values)
            {
                heap.Push(n);
            }

            for (int i = 0; i < numElements; ++i)
            {
                values[i] = heap.Pop();
            }

            Assert.Equal(expectedValues, values);
        }
    }
}
