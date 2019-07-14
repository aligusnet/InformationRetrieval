using System;
using System.Collections.Generic;

namespace InformationRetrieval.Utility
{
    public static class VarintEncoder
    {
        public static int Encode(ulong n, Span<byte> output)
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

            buffer.Slice(pos, BufferLength - pos).CopyTo(output);

            return BufferLength - pos;
        }

        public static IEnumerable<ulong> Decode(byte[] data, int start, int length)
        {
            int end = start + length;
            ulong n = 0;
            for (int i = start; i < end; ++i)
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
    }
}
