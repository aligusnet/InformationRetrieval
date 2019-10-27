using System;
using System.Collections;
using System.Collections.Generic;
using ProtoBuf;

using Corpus;

namespace InformationRetrieval.Indexing.PostingsList
{
    /// <summary>
    /// Compressed Postings List.
    /// Stores documents ids as ranges.
    /// </summary>
    [ProtoContract]
    public class RangePostingsList : IReadOnlyCollection<DocumentId>
    {
        private const int DefaultCapacity = 16;

        // every odd number is DocumentId starting the range 
        // and the next number is length of the range.
        [ProtoMember(1)]
        public IList<uint> Ranges { get; }

        [ProtoMember(2)]
        public int Count { get; private set; }

        public RangePostingsList() : this(0, new List<uint>(DefaultCapacity)) { }

        public RangePostingsList(int count, IList<uint> postings)
        {
            this.Count = count;
            this.Ranges = postings;
        }

        public void Add(DocumentId id)
        {
            Add(id.Id);
        }

        public void Add(uint id)
        {
            if (Ranges.Count > 0 && Ranges[^2] + Ranges[^1] == id)
            {
                Ranges[^1] = Ranges[^1] + 1;
                Count++;
            }
            else if (Ranges.Count == 0 || Ranges[^2] + Ranges[^1] < id)
            {
                Ranges.Add(id);
                Ranges.Add(1);
                Count++;
            }
        }

        public IEnumerator<DocumentId> GetEnumerator()
        {
            for (int i = 1; i < Ranges.Count; i += 2)
            {
                uint startId = Ranges[i - 1];
                uint length = Ranges[i];
                for (uint j = 0; j < length; ++j)
                {
                    yield return new DocumentId(startId + j);
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
