using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;

namespace NaturalLanguageTools.Utility
{
    public class TemporaryBuffer<T> : IEnumerable<T>, IDisposable where T : IComparable<T>
    {
        private T[] buffer;
        private bool disposed = false;

        public TemporaryBuffer(int size)
        {
            buffer = ArrayPool<T>.Shared.Rent(size);
            Count = 0;
        }

        public int Count { get; private set; }

        public T Last()
        {
            return buffer[Count - 1];
        }

        public void Add(T ch)
        {
            if (Count >= buffer.Length)
            {
                Resize(buffer.Length * 2);
            }

            buffer[Count] = ch;
            ++Count;
        }

        public void Insert(int index, T ch)
        {
            if (Count == buffer.Length)
            {
                Resize(buffer.Length*2);
            }

            for (int i = Count; i > index; --i)
            {
                buffer[i] = buffer[i - 1];
            }

            buffer[index] = ch;
            ++Count;
        }

        public bool EndsWith(T ch)
        {
            if (Count < 1)
            {
                return false;
            }

            return buffer[Count - 1].CompareTo(ch) == 0;
        }

        public bool EndsWith(T[] chs)
        {
            if (Count < chs.Length)
            {
                return false;
            }

            int offset = Count - chs.Length;

            for (int i = 0; i < chs.Length; ++i)
            {
                if (buffer[i + offset].CompareTo(chs[i]) != 0)
                {
                    return false;
                }
            }

            return true;
        }

        public void Discard()
        {
            Count = 0;
        }

        public void DiscardLast()
        {
            --Count;
        }

        public void Commit(ICollection<T> target)
        {
            for (int i = 0; i < Count; ++i)
            {
                target.Add(buffer[i]);
            }

            Count = 0;
        }

        public void Apply(Action<T[], int> action)
        {
            action(buffer, Count);
        }

        public TResult Apply<TResult>(Func<T[], int, TResult> func)
        {
            return func(buffer, Count);
        }

        public void Dispose()
        {
            if (!disposed)
            {
                ArrayPool<T>.Shared.Return(buffer);
                disposed = true;
            }

            GC.SuppressFinalize(true);
        }

        private void Resize(int newSize)
        {
            var tmp = buffer;
            buffer = ArrayPool<T>.Shared.Rent(newSize);
            Array.Copy(tmp, 0, buffer, 0, tmp.Length);
            ArrayPool<T>.Shared.Return(tmp);
        }

        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < Count; ++i)
            {
                yield return buffer[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        ~TemporaryBuffer()
        {
            Dispose();
        }
    }
}
