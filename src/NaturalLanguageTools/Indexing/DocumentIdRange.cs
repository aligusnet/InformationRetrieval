using System;
using System.Collections;
using System.Collections.Generic;
using ProtoBuf;

using DocumentStorage;

namespace NaturalLanguageTools.Indexing
{
    static class DocumentIdRange
    {
        private const int offset = 16;

        public static uint Make(ushort localId)
        {
            return ((uint)localId << offset) + 1;
        }

        public static uint End(uint range)
        {
            return (range >> offset) + (ushort)range;
        }

        public static (ushort Start, ushort Length) Decompose(uint range)
        {
            return ((ushort)(range >> offset), (ushort)range);
        }
    }

    [ProtoContract(Name = "RangeList")]
    public class DocumentIdRangeCollection
    {
        private const int DefaultCapacity = 8;

        [ProtoMember(1)]
        public ushort CollectionId { get; }

        [ProtoMember(2)]
        public IList<uint> Ranges { get; }

        // for protobuf deserialization
        private DocumentIdRangeCollection() : this(0)
        {
        }

        public DocumentIdRangeCollection(ushort id)
        {
            CollectionId = id;
            Ranges = new List<uint>(DefaultCapacity);
        }

        public bool Add(ushort localId)
        {
            bool isAdded = false;

            if (Ranges.Count == 0)
            {
                Ranges.Add(DocumentIdRange.Make(localId));
                isAdded = true;
            }
            else
            {
                Index lastIndex = ^1;

                var lastRange = Ranges[lastIndex];
                var nextLocalId = DocumentIdRange.End(lastRange);
                if (nextLocalId == localId)
                {
                    lastRange += 1;
                    Ranges[lastIndex] = lastRange;
                    isAdded = true;
                }
                else if (nextLocalId < localId)
                {
                    Ranges.Add(DocumentIdRange.Make(localId));
                    isAdded = true;
                }
            }

            return isAdded;
        }
    }

    [ProtoContract(Name = "CollectionList")]
    public class DocumentIdRangeCollectionList : IEnumerable<DocumentId>
    {
        private const int DefaultCapacity = 8;

        [ProtoMember(1)]
        private readonly IList<DocumentIdRangeCollection> list = new List<DocumentIdRangeCollection>(DefaultCapacity);

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

        public void Add(uint id)
        {
            Add(new DocumentId(id));
        }

        public IEnumerator<DocumentId> GetEnumerator()
        {
            foreach (var collection in list)
            {
                foreach (var r in collection.Ranges)
                {
                    var range = DocumentIdRange.Decompose(r);
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
