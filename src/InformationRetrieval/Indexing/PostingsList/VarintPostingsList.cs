using System;
using System.Collections.Generic;

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

        public VarintPostingsList()
        {
            data = new byte[32];
            length = 0;
            prevInserted = 0;
            Count = 0;
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
        {
            return GetEnumerator();
        }

        private void Resize(int newSize)
        {
            Array.Resize(ref data, newSize);
        }
    }
}
