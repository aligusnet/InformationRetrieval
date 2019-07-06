using System;
using System.Collections;
using System.Collections.Generic;
using ProtoBuf;

using Corpus;

namespace NaturalLanguageTools.Indexing
{
    /// <summary>
    /// Compressed Postings List.
    /// Stores documents ids as ranges.
    /// </summary>
    [ProtoContract]
    public class RangePostingsList : IEnumerable<DocumentId>
    {
        private const int DefaultCapacity = 8;

        [ProtoMember(1)]
        private readonly IList<DocumentIdRangeBlock> list = new List<DocumentIdRangeBlock>(DefaultCapacity);

        [ProtoMember(2)]
        public int DocumentsCount { get; private set; } = 0;

        public void Add(DocumentId id)
        {
            DocumentIdRangeBlock block;

            if (list.Count == 0 || list[^1].BlockId != id.BlockId)
            {
                block = new DocumentIdRangeBlock(id.BlockId);
                list.Add(block);
            }
            else
            {
                block = list[^1];
            }

            if (block.Add(id.LocalId)) DocumentsCount++;
        }

        public void Add(uint id)
        {
            Add(new DocumentId(id));
        }

        public IEnumerator<DocumentId> GetEnumerator()
        {
            foreach (var block in list)
            {
                foreach (var r in block.Ranges)
                {
                    var range = DocumentIdRange.Decompose(r);
                    for (ushort i = 0; i < range.Length; ++i)
                    {
                        yield return new DocumentId(block.BlockId, (ushort)(range.Start + i));
                    }
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }

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

    [ProtoContract]
    public class DocumentIdRangeBlock
    {
        private const int DefaultCapacity = 8;

        [ProtoMember(1)]
        public ushort BlockId { get; }

        [ProtoMember(2)]
        public IList<uint> Ranges { get; }

        // for protobuf deserialization
        private DocumentIdRangeBlock() : this(0)
        {
        }

        public DocumentIdRangeBlock(ushort id)
        {
            BlockId = id;
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
}
