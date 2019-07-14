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
            prevInserted = this.LastOrDefault().Id;
        }

        public int Count { get; private set; }

        public void Add(DocumentId docId)
        {
            if (length + VarintEncoder.BufferLength >= data.Length)
            {
                Resize(Math.Max(length + VarintEncoder.BufferLength, data.Length * 2));
            }

            if (prevInserted > docId.Id)
            {
                throw new ArgumentException("DocumentIds are expected to be in non-decreasing order");
            }

            uint docIdGap = docId.Id - prevInserted;
            length += VarintEncoder.Encode(docIdGap, data.AsSpan(length));
            prevInserted = docId.Id;
            ++Count;
        }

        public IEnumerator<DocumentId> GetEnumerator()
        {
            ulong prevId = 0;

            foreach (ulong gap in VarintEncoder.Decode(data, 0, length))
            {
                prevId += gap;
                yield return new DocumentId((uint)prevId);
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
