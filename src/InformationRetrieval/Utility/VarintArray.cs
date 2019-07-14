using System;
using System.Collections;
using System.Collections.Generic;

namespace InformationRetrieval.Utility
{
    /// <summary>
    /// Memory-effectively stores small non-negative integers
    /// </summary>
    public class VarintArray : IReadOnlyCollection<ulong>
    {
        private readonly ExpandableArray<byte> data;

        public static int GetIntegerCount(ReadOnlySpan<byte> buffer)
        {
            int count = 0;

            for (int i = 0; i < buffer.Length; ++i)
            {
                if ((buffer[i] & 0x80) == 0x80)
                {
                    ++count;
                }
            }

            return count;
        }

        public VarintArray()
        {
            data = new ExpandableArray<byte>();
            Count = 0;
        }

        public VarintArray(byte[] buffer) : this(buffer, GetIntegerCount(buffer.AsSpan())) { }

        public VarintArray(byte[] buffer, int count)
        {
            data = new ExpandableArray<byte>(buffer);
            Count = count;
        }

        public int Count { get; private set; }

        public void Add(ulong n)
        {
            const int BufferLength = 10;
            Span<byte> buffer = stackalloc byte[BufferLength];
            int pos = BufferLength;
            do
            {
                buffer[--pos] = (byte)(n & 0x7f);
                n >>= 7;
            }
            while (n != 0);

            buffer[BufferLength - 1] |= 0x80;

            data.Add(buffer.Slice(pos, BufferLength - pos));
            ++Count;
        }

        public ReadOnlySpan<byte> GetReadOnlySpan() 
            => data.GetReadOnlySpan();

        public IEnumerator<ulong> GetEnumerator()
        {
            ulong n = 0;
            for (int i = 0; i < data.Length; ++i)
            {
                if ((data[i] & 0x80) == 0x80)
                {
                    n = (n << 7) + (ulong)(data[i] & 0x7f);
                    yield return n;
                    n = 0;
                }
                else
                {
                    n = (n << 7) + data[i];
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
