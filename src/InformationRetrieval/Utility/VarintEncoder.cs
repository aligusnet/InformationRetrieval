using System;
using System.Collections.Generic;

namespace InformationRetrieval.Utility
{
    public static class VarintEncoder
    {
        public const int BufferLength = 10;

        private const byte LastByteMask = 0x80;

        public static int Encode(ulong n, Span<byte> output)
        {
            Span<byte> buffer = stackalloc byte[BufferLength];
            int pos = BufferLength;
            do
            {
                buffer[--pos] = (byte)(n & 0x7f);
                n >>= 7;
            }
            while (n != 0);

            buffer[BufferLength - 1] |= LastByteMask;

            buffer.Slice(pos, BufferLength - pos).CopyTo(output);

            return BufferLength - pos;
        }

        public static IEnumerable<ulong> Decode(byte[] data, int start, int length)
        {
            int end = start + length;
            ulong n = 0;
            for (int i = start; i < end; ++i)
            {
                if (IsLastByte(data[i]))
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

        /// <summary>
        /// Decode teh first number from the buffer
        /// </summary>
        /// <param name="buffer">The buffer</param>
        /// <returns>Decoded number</returns>
        public static ulong Decode(ReadOnlySpan<byte> buffer)
        {
            ulong n = 0;
            for (int i = 0; i < buffer.Length; ++i)
            {
                if (IsLastByte(buffer[i]))
                {
                    n = (n << 7) + (ulong)(buffer[i] & 0x7f);
                    return n;
                }
                else
                {
                    n = (n << 7) + buffer[i];
                }
            }

            throw new Exception("Failed to decode varint: last byte is missing");
        }

        public static int GetIntegerCount(ReadOnlySpan<byte> buffer)
        {
            int count = 0;

            for (int i = 0; i < buffer.Length; ++i)
            {
                if (IsLastByte(buffer[i]))
                {
                    ++count;
                }
            }

            return count;
        }

        /// <summary>
        /// Return length of the first number
        /// </summary>
        /// <param name="buffer">the buffer</param>
        /// <returns>Length of the first byte if found, -1 otherwise</returns>
        public static int FindFirst(ReadOnlySpan<byte> buffer)
        {
            for (int i = 0; i < buffer.Length; ++i)
            {
                if (IsLastByte(buffer[i]))
                {
                    return i + 1;
                }
            }

            return -1;
        }

        /// <summary>
        /// return positiong of the first byte of the last number
        /// </summary>
        /// <param name="buffer">the buffer</param>
        /// <returns>Position of the last number if found, -1 otherwise</returns>
        public static int FindLast(ReadOnlySpan<byte> buffer)
        {
            int lastBytesCount = 0;
            for (int i = buffer.Length; i > 0; --i)
            {
                if (IsLastByte(buffer[i-1]))
                {
                    if(++lastBytesCount == 2)
                    {
                        return i;
                    }
                }
            }

            return -1;
        }

        private static bool IsLastByte(byte b) 
            => (b & LastByteMask) == LastByteMask;
    }
}
