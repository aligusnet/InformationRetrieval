using System;
using System.Collections.Generic;
using System.Linq;

using Corpus;
using InformationRetrieval.Utility;
using System.Collections;

namespace InformationRetrieval.Indexing.PostingsList
{
    /// <summary>
    /// Compress postings lists using varints
    /// </summary>
    public class VarintPostingsList : IReadOnlyCollection<DocumentId>
    {
        private byte[] data;
        private int length;
        private uint prevInserted;

        public VarintPostingsList() : this(0) { }

        public VarintPostingsList(int capacity)
        {
            data = capacity > 0 ? new byte[capacity] : Array.Empty<byte>();
            length = 0;
            prevInserted = 0;
            Count = 0;
        }

        public VarintPostingsList(byte[] buffer) : this(buffer, buffer.Length) { }

        public VarintPostingsList(byte[] buffer, int length)
        {
            data = buffer;
            this.length = length;
            Count = VarintEncoder.GetIntegerCount(buffer.AsSpan(0, length));
            prevInserted = this.LastOrDefault();
        }

        public int Count { get; private set; }

        public void Add(uint id)
        {
            if (Count > 0 && prevInserted == id)
            {
                return;
            }

            if (length + VarintEncoder.BufferLength >= data.Length)
            {
                Resize(Math.Max(length + VarintEncoder.BufferLength, data.Length * 2));
            }

            if (prevInserted > id)
            {
                throw new ArgumentException($"DocumentIds are expected to be in non-decreasing order: {prevInserted} > {id}");
            }

            uint docIdGap = id - prevInserted;
            length += VarintEncoder.Encode(docIdGap, data.AsSpan(length));
            prevInserted = id;
            ++Count;
        }

        public void Add(DocumentId docId)
            => Add(docId.Id);

        public IEnumerator<DocumentId> GetEnumerator()
        {
            ulong prevId = 0;

            foreach (ulong gap in VarintEncoder.Decode(data, 0, length))
            {
                prevId += gap;
                yield return new DocumentId((uint)prevId);
            }
        }

        private uint LastOrDefault()
        {
            int lastPos = VarintEncoder.FindLast(data.AsSpan(0, length));
            if (lastPos >= 0)
            {
                return (uint)VarintEncoder.Decode(data.AsSpan(lastPos));
            }
            else
            {
                return 0;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() 
            => GetEnumerator();

        public ReadOnlySpan<byte> GetReadOnlySpan() 
            => data.AsSpan(0, length);

        private void Resize(int newSize) 
            => Array.Resize(ref data, newSize);
    }
}
