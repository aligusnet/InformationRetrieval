using System;
using System.Collections;
using System.Collections.Generic;
using ProtoBuf;

using DocumentStorage;

namespace NaturalLanguageTools.Indexing
{
    [ProtoContract]
    public struct DocumentIdRange
    {
        [ProtoMember(1)]
        public ushort Start { get; }
        public int End => Start + Length;
        [ProtoMember(2)]
        public ushort Length { get; set; }

        public DocumentIdRange(ushort localId)
        {
            Start = localId;
            Length = 1;
        }
    }

    [ProtoContract]
    public class DocumentIdRangeCollection
    {
        [ProtoMember(1)]
        public ushort CollectionId { get; }

        [ProtoMember(2)]
        public IList<DocumentIdRange> Ranges { get; }

        // for protobuf deserialization
        private DocumentIdRangeCollection() : this(0)
        {
        }

        public DocumentIdRangeCollection(ushort id)
        {
            CollectionId = id;
            Ranges = new List<DocumentIdRange>();
        }

        public bool Add(ushort localId)
        {
            bool isAdded = false;

            if (Ranges.Count == 0)
            {
                Ranges.Add(new DocumentIdRange(localId));
                isAdded = true;
            }
            else
            {
                Index lastIndex = ^1;

                var lastRange = Ranges[lastIndex];
                if (lastRange.End == localId)
                {
                    lastRange.Length += 1;
                    Ranges[lastIndex] = lastRange;
                    isAdded = true;
                }
                else if (lastRange.End < localId)
                {
                    Ranges.Add(new DocumentIdRange(localId));
                    isAdded = true;
                }
            }

            return isAdded;
        }
    }

    [ProtoContract]
    public class DocumentIdRangeCollectionList : IEnumerable<DocumentId>
    {
        [ProtoMember(1)]
        private readonly IList<DocumentIdRangeCollection> list = new List<DocumentIdRangeCollection>();

        [ProtoMember(2)]
        public int DocumentsCount { get; private set; } = 0;

        public void Add(DocumentId id)
        {
            DocumentIdRangeCollection collection;

            if (list.Count == 0 || list[^1].CollectionId != id.CollectionId)
            {
                collection = new DocumentIdRangeCollection(id.CollectionId);
                list.Add(collection);
            }
            else
            {
                collection = list[^1];
            }

            if ( collection.Add(id.LocalId) ) DocumentsCount++;
        }

        public IEnumerator<DocumentId> GetEnumerator()
        {
            foreach (var collection in list)
            {
                foreach (var range in collection.Ranges)
                {
                    for (ushort i = 0; i < range.Length; ++i)
                    {
                        yield return new DocumentId(collection.CollectionId, (ushort)(range.Start + i));
                    }
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
