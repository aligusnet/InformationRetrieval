using System.Collections.Generic;

namespace NaturalLanguageTools.Utility
{
    public class PriorityQueue<T>
    {
        private readonly IComparer<T> comparer;
        private readonly T[] heap;
        private int length;

        public PriorityQueue(int capacity, IComparer<T> comparer)
        {
            this.comparer = comparer;
            this.heap = new T[capacity];
            this.length = 0;
        }

        public void Push(T value)
        {
            heap[length] = value;
            int i = length;
            while (HeapPropertyHolds(i) == false)
            {
                int p = Parent(i);
                Swap(i, p);
                i = p;
            }
            length++;
        }

        public T Pop()
        {
            T value = heap[0];

            length--;
            heap[0] = heap[length];
            Heapify(0);

            return value;
        }

        private void Heapify(int i)
        {
            int l = Left(i);
            int r = Right(i);
            int largest = i;

            if (l < length && comparer.Compare(heap[l], heap[largest]) > 0)
            {
                largest = l;
            }

            if (r < length && comparer.Compare(heap[r], heap[largest]) > 0)
            {
                largest = r;
            }

            if (i != largest)
            {
                Swap(i, largest);
                Heapify(largest);
            }
        }

        private bool HeapPropertyHolds(int i) => 
            i == 0 || comparer.Compare(heap[Parent(i)], heap[i]) >= 0;

        private void Swap(int i, int j)
        {
            T tmp = heap[i];
            heap[i] = heap[j];
            heap[j] = tmp;
        }

        internal static int Parent(int i) => (i - 1) >> 1;

        internal static int Left(int i) => (i << 1) + 1;

        internal static int Right(int i) => (i << 1) + 2;
    }
}
