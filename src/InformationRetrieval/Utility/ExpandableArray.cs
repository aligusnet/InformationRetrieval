using System;
using System.Collections;
using System.Collections.Generic;

namespace InformationRetrieval.Utility
{
    /// <summary>
    /// A simple wrapper around System.Array 
    /// intended to be almost as fast as System.Array
    /// but expandable.
    /// It exposes internal array unlike System.Generic.List.
    /// </summary>
    /// <typeparam name="T">The type of the elements of the array.</typeparam>
    public class ExpandableArray<T> : IReadOnlyCollection<T>
    {
        private T[] buffer;

        public ExpandableArray() : this(0) { }

        public ExpandableArray(int capacity)
        {
            Length = 0;
            buffer = capacity > 0 ? new T[capacity] : Array.Empty<T>();
        }

        /// <summary>
        /// Creates ExpandableArray from buffer.
        /// ExpandableArray owns the buffer and can freely move it etc.
        /// </summary>
        /// <param name="buffer">The buffer</param>
        public ExpandableArray(T[] buffer) : this (buffer, buffer.Length) { }

        /// <summary>
        /// Creates ExpandableArray from buffer.
        /// ExpandableArray owns the buffer and can freely move it etc.
        /// </summary>
        /// <param name="buffer">The buffer</param>
        /// <param name="length">The length</param>
        public ExpandableArray(T[] buffer, int length)
        {
            Length = length;
            this.buffer = buffer;
        }

        public int Length { get; private set; }
         
        public T[] Buffer { get => buffer; }

        public int Count => Length;

        public void Add(T v)
        {
            if (Length >= buffer.Length)
            {
                Resize(Math.Max(8, buffer.Length * 2));
            }

            buffer[Length] = v;
            ++Length;
        }

        public void Add(T[] values)
        {
            if (Length + values.Length > buffer.Length)
            {
                Resize(Math.Max(Length + values.Length, buffer.Length * 2));
            }

            Array.Copy(values, 0, buffer, Length, values.Length);
            Length += values.Length;
        }

        public void Add(Span<T> values)
        {
            if (Length + values.Length > buffer.Length)
            {
                Resize(Math.Max(Length + values.Length, buffer.Length * 2));
            }

            values.CopyTo(buffer.AsSpan(startIndex: Length));
            Length += values.Length;
        }

        public ref T this[int index] => ref buffer[index];

        public ReadOnlySpan<T> GetReadOnlySpan() 
            => buffer.AsSpan(0, Length);

        public void Resize(int newSize)
        {
            Array.Resize(ref buffer, newSize);
        }

        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < Length; ++i)
            {
                yield return buffer[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
